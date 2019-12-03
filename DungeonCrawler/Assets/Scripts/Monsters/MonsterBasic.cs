using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum State
{
    Default,
    Chase,
    Attack,
}

enum AnimationStates
{
    Idle,
    Walk,
    Attack
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

    State monsterState;

    // Animations states
    public static readonly string[] animationsStatesString = {"Idle",
    "Walk",
    "Attack"};

    // Unity functions

    // Start is called before the first frame update
    void Start()
    {
        animator = transforms[0].GetComponent<Animator>();
        currentAnimationState = "Idle";
        lastAnimationState = "";
    }

    // Update is called once per frame
    void Update()
    {

    }
    void OnTriggerStay(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            Quaternion targetRotation = Quaternion.LookRotation(other.transform.position - transform.position);
            float oryginalX = transform.rotation.x;
            float oryginalZ = transform.rotation.z;

            Quaternion finalRotation = Quaternion.Slerp(transform.rotation, targetRotation, 5.0f * Time.deltaTime);
            finalRotation.x = oryginalX;
            finalRotation.z = oryginalZ;
            transform.rotation = finalRotation;

            if(timer>0)
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
                    timer = 1 / attackSpeed;
                }
                else if (timer + 1 < 1 / attackSpeed)
                {
                    animationSet(AnimationStates.Idle);
                }
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            animationSet((int)AnimationStates.Idle);
        }
    }

    // Functions


    private void animationSet(AnimationStates animationStateToPlay)
    {
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

void Attack()
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
}
