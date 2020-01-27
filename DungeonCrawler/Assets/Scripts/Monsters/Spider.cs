using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spider : MonsterBasic
{
    public float poisonProbability;

    protected override void Attack(Collider player)
    {
        base.Attack(player);
        if (Random.Range(0.0f, 1.0f) <= poisonProbability)
        {
            // TODO apply effect to player
        }
    }
}
