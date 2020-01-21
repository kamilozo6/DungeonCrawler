using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum State
{
    Default,
    Chase,
    Attack,
    Death
}

enum AnimationStates
{
    Idle,
    Walk,
    Attack,
    Death
}

public class MonsterBasic : MonoBehaviour
{
    // Properties
    public int damge;
    public float attackSpeed;
    public float moveSpeed;
    public float range;
    public int healthPoints;

    public Transform[] transforms;

    private float timer = 0;
    private string currentAnimationState;
    private string lastAnimationState;
    private Animator animator;
    private AnimatorStateInfo stateInfo;
    private const float deletionAfterDeathTime = 5;
    private float deathTimer;

    State monsterState;

    // Animations states
    public static readonly string[] animationsStatesString = {"Idle",
    "Walk",
    "Attack",
    "Death"};

    // Unity functions

    // Start is called before the first frame update
    void Start()
    {
        animator = transforms[0].GetComponent<Animator>();
        currentAnimationState = "Idle";
        lastAnimationState = "";
        monsterState = State.Default;
        deathTimer = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(monsterState.Equals(State.Death))
        {
            deathTimer += Time.deltaTime;
            if(deathTimer >= deletionAfterDeathTime)
            {
                Destroy(gameObject);
            }
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player") && !monsterState.Equals(State.Death))
        {
            monsterState = State.Chase;
        }
    }

    void OnTriggerStay(Collider other)
    {
        if (other.tag.Equals("Player") && !monsterState.Equals(State.Death))
        {
            transform.rotation = GetRotation(other);

            if (timer>0)
            {
                timer -= Time.deltaTime;
            }

            float distance = Vector3.Distance(transform.position, other.transform.position);
            if (distance > range)
            {
                transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
                animationSet(AnimationStates.Walk);
            }
            else
            {
                if (timer <= 0)
                {
                    animationSet(AnimationStates.Attack);
                    Attack(other);
                    timer = 1 / attackSpeed;
                }
                else if (timer + 2 < 1 / attackSpeed)
                {
                    animationSet(AnimationStates.Idle);
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player") && !monsterState.Equals(State.Death))
        {
            monsterState = State.Default;
            animationSet(AnimationStates.Idle);
        }
    }

    // Functions


    private void animationSet(AnimationStates animationStateToPlay)
    {
        // If monster is dead only animation that we can set is death
        if(monsterState.Equals(State.Death) && 
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

    protected virtual void Attack(Collider player)
    {
    }

    void Chase()
    {

    }

    void Move()
    {

    }

    void MakeMove()
    {

    }

    protected Quaternion GetRotation(Collider other)
    {
        Quaternion targetRotation = Quaternion.LookRotation(other.transform.position - transform.position);
        float oryginalX = transform.rotation.x;
        float oryginalZ = transform.rotation.z;

        Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, 5.0f * Time.deltaTime);
        finalRotation.x = oryginalX;
        finalRotation.z = oryginalZ;

        return finalRotation;
    }
}
