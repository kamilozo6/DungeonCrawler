using Assets.Scripts.Monsters;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MonsterBasic : MonoBehaviour
{
    protected enum State
    {
        Default,
        Chase,
        Attack,
        Death
    }

    protected enum AnimationStates
    {
        Idle,
        Walk,
        Attack,
        Death,
        Charge
    }
    // Properties
    public int damge;
    public float attackSpeed;
    public float moveSpeed;
    public float range;
    public int healthPoints;

    public Transform[] transforms;

    protected float timer = 0;
    protected string currentAnimationState;
    protected string lastAnimationState;
    protected Animator animator;
    protected AnimatorStateInfo stateInfo;
    protected const float deletionAfterDeathTime = 5;
    protected float deathTimer;

    protected State monsterState;

    protected const float xFraction = 0.5f;
    protected const float zFraction = 0.5f;
    protected Vector2Int destination;
    protected Vector3 direction;

    protected bool wasResetedInMove;

    protected Pathfinder pathfinder;
    protected List<Node> currentPath;

    protected bool isFollowingPlayer;

    // Animations states
    public static readonly string[] animationsStatesString = {"Idle",
    "Walk",
    "Attack",
    "Death",
    "Charge"};

    // Unity functions

    // Start is called before the first frame update
    void Start()
    {
        animator = transforms[0].GetComponent<Animator>();
        currentAnimationState = "Idle";
        lastAnimationState = "";
        monsterState = State.Default;
        deathTimer = 0;
        pathfinder = new Pathfinder(xFraction, zFraction, transform.position.y, 15);
        isFollowingPlayer = false;
        SphereCollider sphereCollider = GetComponent<SphereCollider>();
        sphereCollider.radius *= 2;
        moveSpeed += 2;
    }

    // Update is called once per frame
    void Update()
    {
        if (monsterState.Equals(State.Death))
        {
            deathTimer += Time.deltaTime;
            if (deathTimer >= deletionAfterDeathTime)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player") && !monsterState.Equals(State.Death))
        {
            isFollowingPlayer = false;

        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag.Equals("Player") && !monsterState.Equals(State.Death))
        {
            UpdateTimer();

            MakeMove(other);
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player") && !monsterState.Equals(State.Death))
        {
            animationSet(AnimationStates.Idle);
        }
    }

    // Functions

    public static readonly string[] monsterPrefabs = {"1Zombie",
    "2Skeleton",
    "3Spider",
    "4Dragon",
    "5Wizard",
    "6Golem",
    "7Troll",
    "8Goblin",
    "9Orc"};

    public static void SpawnRandomMonster(Vector3 spawnPosition)
    {
        int randomNumber = Random.Range(0, 9);
        GameObject monsterPrefab = Resources.Load("Monsters/" + monsterPrefabs[randomNumber]) as GameObject;
        Instantiate(monsterPrefab, spawnPosition, Quaternion.Euler(0, 0, 0));
    }

    protected void UpdateTimer()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
        }

        if (monsterState.Equals(State.Attack) &&
           ((timer <= 0) ||
            (timer + 2 < 1 / attackSpeed)))
        {
            monsterState = State.Default;
        }
    }

    protected void animationSet(AnimationStates animationStateToPlay)
    {
        // If monster is dead only animation that we can set is death
        if (monsterState.Equals(State.Death) &&
            !animationStateToPlay.Equals(AnimationStates.Death))
        {
            return;
        }

        string animationToPlay = animationsStatesString[(int)animationStateToPlay];
        stateInfo = animator.GetCurrentAnimatorStateInfo(0);

        if (animationToPlay.Equals(currentAnimationState))
        {
            // State does not changes
            return;
        }

        if (lastAnimationState != "")
        {
            animator.SetBool(lastAnimationState + "To" + currentAnimationState, false);
            lastAnimationState = "";
        }

        animator.SetBool(currentAnimationState + "To" + animationToPlay, true);
        lastAnimationState = currentAnimationState;
        currentAnimationState = animationToPlay;
    }

    public void GetHit(int value)
    {
        if (!monsterState.Equals(State.Death))
        {
            healthPoints -= value;
            if (healthPoints <= 0)
            {
                monsterState = State.Death;
                animationSet(AnimationStates.Death);
            }
        }
    }

    protected void CheckTimerAndAttack(Collider player)
    {
        transform.rotation = GetRotation(player);

        if (timer <= 0)
        {
            animationSet(AnimationStates.Attack);
            Attack(player);
            timer = 1 / attackSpeed;
        }
        else if (!monsterState.Equals(State.Attack))
        {
            animationSet(AnimationStates.Idle);
        }
    }

    protected virtual void Attack(Collider player)
    {
        monsterState = State.Attack;
        //TODO hit player

    }

    protected void SetDirection()
    {
        List<Node> path = currentPath;
        Vector2Int pos = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));
        Vector2Int dir = path[0].position - pos;

        // Move in x axis
        if (dir.Equals(Vector2Int.right))
        {
            direction = Vector3.right;
        }
        else if (dir.Equals(Vector2Int.left))
        {
            direction = Vector3.left;
        }
        // Move in z axis
        else if (dir.Equals(Vector2Int.up))
        {
            direction = Vector3.forward;
        }
        else if (dir.Equals(Vector2Int.down))
        {
            direction = Vector3.back;
        }
        else
        {
            throw new System.Exception();
        }
    }

    protected void SetDestination()
    {
        Vector3 tmpDestination = transform.position + direction;
        destination.x = Mathf.FloorToInt(tmpDestination.x);
        destination.y = Mathf.FloorToInt(tmpDestination.z);
    }

    protected bool CheckIfReachedDestination(float currentPos, int flooredPos, int destinationPart, float fraction, bool isHigher)
    {
        if (isHigher)
        {
            if (flooredPos.Equals(destinationPart) &&
                (currentPos - flooredPos > fraction))
            {
                return true;
            }
        }
        else
        {
            if (flooredPos.Equals(destinationPart) &&
                (currentPos - flooredPos < fraction))
            {
                return true;
            }
        }
        return false;
    }

    protected void CheckIfShouldStop()
    {
        Vector3 tmpPostition = transform.position;
        int xPos = Mathf.FloorToInt(tmpPostition.x);
        int zPos = Mathf.FloorToInt(tmpPostition.z);

        if (direction.Equals(Vector3.forward))
        {
            if (CheckIfReachedDestination(tmpPostition.z, zPos, destination.y, zFraction, true))
            {
                monsterState = State.Default;
                currentPath.RemoveAt(0);
            }
        }
        else if (direction.Equals(Vector3.back))
        {
            if (CheckIfReachedDestination(tmpPostition.z, zPos, destination.y, zFraction, false))
            {
                monsterState = State.Default;
                currentPath.RemoveAt(0);
            }
        }
        else if (direction.Equals(Vector3.right))
        {
            if (CheckIfReachedDestination(tmpPostition.x, xPos, destination.x, xFraction, true))
            {
                monsterState = State.Default;
                currentPath.RemoveAt(0);
            }
        }
        else if (direction.Equals(Vector3.left))
        {
            if (CheckIfReachedDestination(tmpPostition.x, xPos, destination.x, xFraction, false))
            {
                monsterState = State.Default;
                currentPath.RemoveAt(0);
            }
        }
    }

    protected void Move(Collider player)
    {
        if (monsterState.Equals(State.Default))
        {
            //SetNewPath(player);
            Vector2Int start = new Vector2Int(Mathf.FloorToInt(transform.position.x), Mathf.FloorToInt(transform.position.z));
            Vector2Int end = new Vector2Int(Mathf.FloorToInt(player.transform.position.x), Mathf.FloorToInt(player.transform.position.z));
            pathfinder.FindPath(start, end);
            currentPath = new List<Node>(pathfinder.GetPath());
            if (currentPath.Count != 0)
            {
                monsterState = State.Chase;
                SetDirection();
                SetDestination();
            }
            else
            {
                isFollowingPlayer = false;
                animationSet(AnimationStates.Idle);
            }
        }

        if (monsterState.Equals(State.Chase))
        {
            transform.rotation = GetRotation(player);
            transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
            animationSet(AnimationStates.Walk);
            CheckIfShouldStop();
        }
    }

    protected void SetNewPath(Collider player)
    {
        int bestInd = currentPath.Count - 1;
        Node bestNode = currentPath[bestInd];
        Vector2Int playerPos = new Vector2Int(Mathf.FloorToInt(player.transform.position.x), Mathf.FloorToInt(player.transform.position.z));

        for (int ind = bestInd - 1; ind >= 0; ind--)
        {
            Node currNode = currentPath[ind];
            Vector2Int distance = playerPos - currNode.position;
            Vector2Int bestDistance = playerPos - bestNode.position;
            if (Mathf.Abs(bestDistance.x) + Mathf.Abs(bestDistance.y) > Mathf.Abs(distance.x) + Mathf.Abs(distance.y))
            {
                bestInd = ind;
                bestNode = currNode;
            }

            if (bestDistance.Equals(Vector2Int.zero))
            {
                if (bestInd != currentPath.Count - 1)
                {
                    currentPath.RemoveRange(ind + 1, currentPath.Count - ind - 1);
                }
                return;
            }
        }

        Vector2Int newStartPos = bestNode.position;
        currentPath.RemoveRange(bestInd + 1, currentPath.Count - bestInd - 1);

        pathfinder.FindPath(newStartPos, playerPos);

        currentPath.AddRange(pathfinder.GetPath());
    }

    protected virtual void MakeMove(Collider player)
    {
        float distance = Vector3.Distance(transform.position, player.transform.position);
        RaycastHit raycastHit;
        Physics.Linecast(transform.position, player.transform.position, out raycastHit, 1);

        // if you entered monster chase sphere and it is won't see you for first time
        // it will do nothing
        if (!isFollowingPlayer && !raycastHit.transform.tag.Equals("Player"))
        {
            return;
        }
        else
        {
            isFollowingPlayer = true;
        }

        if (distance > range || monsterState.Equals(State.Chase) || !raycastHit.transform.tag.Equals("Player"))
        {
            Move(player);
        }
        else
        {
            CheckTimerAndAttack(player);
        }
    }

    protected Quaternion GetRotation(Collider other)
    {
        Quaternion targetRotation;
        if (monsterState.Equals(State.Chase))
        {
            targetRotation = Quaternion.LookRotation(direction);
            float oryginalX = transform.rotation.x;
            float oryginalZ = transform.rotation.z;

            Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, 1);
            finalRotation.x = oryginalX;
            finalRotation.z = oryginalZ;

            return finalRotation;
        }
        else
        {
            targetRotation = Quaternion.LookRotation(other.transform.position - transform.position);
            float oryginalX = transform.rotation.x;
            float oryginalZ = transform.rotation.z;

            Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, 1);
            finalRotation.x = oryginalX;
            finalRotation.z = oryginalZ;

            return finalRotation;
        }
    }
}
