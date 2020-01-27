using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orc : MonsterBasic
{
    public float chargeProbability;
    public float rangeOfCharge;
    public float chargeSpeed;
    public float chargeLenght;
    public float waitAfterCharge;

    private bool isInCharge;
    private float chargeTimer;
    private float chargeLenghtLeft;

    void OnTriggerExit(Collider other)
    {
        if (other.tag.Equals("Player") && !monsterState.Equals(State.Death))
        {
            isInCharge = false;
            animationSet(AnimationStates.Idle);
        }
    }

    protected override void MakeMove(Collider player)
    {
        float randomNum = Random.Range(0.0f, 1.0f);
        float distance = Vector3.Distance(transform.position, player.transform.position);
        RaycastHit raycastHit;
        Physics.Linecast(transform.position, player.transform.position, out raycastHit, 1);

        if (!isFollowingPlayer && !raycastHit.transform.tag.Equals("Player"))
        {
            return;
        }
        else
        {
            isFollowingPlayer = true;
        }

        // Melee attack
        if (distance <= range && raycastHit.transform.tag.Equals("Player") && !isInCharge && !monsterState.Equals(State.Chase))
        {
            CheckTimerAndAttack(player);
        }
        // Charge attack
        else if ((randomNum <= chargeProbability && distance <= rangeOfCharge && raycastHit.transform.tag.Equals("Player") && !monsterState.Equals(State.Chase)) || isInCharge)
        {
            Charge(player);
        }
        else
        {
            Move(player);
        }
    }

    private void Charge(Collider player)
    {
        if(!isInCharge)
        {
            isInCharge = true;
            transform.rotation = GetRotation(player);
            chargeLenghtLeft = chargeLenght;
            animationSet(AnimationStates.Charge);
        }

        if (chargeTimer > 0)
        {
            chargeTimer -= Time.deltaTime;
            if (chargeTimer <= 0)
            {
                isInCharge = false;
            }
        }
        else
        {
            transform.Translate(Vector3.forward);
            Vector3 pos = transform.position;
            transform.Translate(Vector3.back);
            Collider[] hitColliders = Physics.OverlapSphere(pos, 0.5f);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                // TODO if no floor return true
                if (hitColliders[i].tag.Equals("Player") || hitColliders[i].tag.Equals("Obstacle"))
                {
                    chargeTimer = waitAfterCharge;
                    animationSet(AnimationStates.Idle);
                    return;
                }
            }
            transform.Translate(Vector3.forward * chargeSpeed * Time.deltaTime);
            chargeLenghtLeft -= chargeSpeed * Time.deltaTime;

            if(chargeLenghtLeft <=0)
            {
                chargeTimer = waitAfterCharge;
                animationSet(AnimationStates.Idle);
                return;
            }
        }
    }
}
