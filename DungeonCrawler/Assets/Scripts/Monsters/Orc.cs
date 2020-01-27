using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orc : MonsterBasic
{
    public float chargeProbability;
    public float rangeOfCharge;

    private bool isInCharge;
    private bool isChargeAttack;

    protected override void MakeMove(Collider player)
    {
        float randomNum = Random.Range(0.0f, 1.0f);
        float distance = Vector3.Distance(transform.position, player.transform.position);
        RaycastHit raycastHit;
        Physics.Linecast(transform.position, player.transform.position, out raycastHit, 1);

        // Melee attack
        if (distance <= range && raycastHit.transform.tag.Equals("Player"))
        {
            isChargeAttack = false;
            CheckTimerAndAttack(player);
        }
        // Range attack
        else if (randomNum <= chargeProbability && distance <= rangeOfCharge && raycastHit.transform.tag.Equals("Player"))
        {
            isChargeAttack = true;
            CheckTimerAndAttack(player);
        }
        else
        {
            Move(player);
        }
    }
}
