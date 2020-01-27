using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Troll : MonsterBasic
{
    public float slowProbability;

    protected override void Attack(Collider player)
    {
        base.Attack(player);
        if(Random.Range(0.0f,1.0f) <= slowProbability)
        {
            // TODO apply effect to player
        }
    }
}
