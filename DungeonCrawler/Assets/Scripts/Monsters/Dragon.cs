using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dragon : MonsterBasic
{
    public float rangeAttackProbability;
    public float rangeOfRangeAttack;
    public GameObject fireballPrefab;

    private bool isRangeAttack;

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
        if (distance <= range && raycastHit.transform.tag.Equals("Player") && !monsterState.Equals(State.Chase)) 
        {
            isRangeAttack = false;
            CheckTimerAndAttack(player);
        }
        // Range attack
        else if (randomNum <= rangeAttackProbability && distance <= rangeOfRangeAttack && raycastHit.transform.tag.Equals("Player") && !monsterState.Equals(State.Chase))
        {
            isRangeAttack = true;
            CheckTimerAndAttack(player);
        }
        else
        {
            Move(player);
        }
    }

    protected override void Attack(Collider player)
    {
        monsterState = State.Attack;
        if (isRangeAttack)
        {
            ShootFireball(player);
        }
        else
        {
            base.Attack(player);
        }
    }

    private void ShootFireball(Collider player)
    {
        Instantiate(fireballPrefab, GetFireballPosition(), GetFireballRotation(player));
    }

    private Quaternion GetFireballRotation(Collider player)
    {
        Quaternion targetRotation = Quaternion.LookRotation(player.transform.position - transform.position);

        float yRotation = targetRotation.eulerAngles.y;

        Quaternion finalRotation = Quaternion.Euler(0, yRotation, 0);

        return finalRotation;
    }

    private Vector3 GetFireballPosition()
    {
        Vector3 arrowPosition = transform.position;
        arrowPosition.y += 1.0f;
        return arrowPosition;
    }
}
