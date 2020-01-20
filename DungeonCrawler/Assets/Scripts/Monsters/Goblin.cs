using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : MonsterBasic
{
    public GameObject arrowPrefab;

    protected override void Attack(Collider player)
    {
        Instantiate(arrowPrefab, GetArrowPosition(), GetArrowRotation(player));
    }

    private Quaternion GetArrowRotation(Collider player)
    {
        Quaternion targetRotation = Quaternion.LookRotation(player.transform.position - transform.position);

        float yRotation = targetRotation.eulerAngles.y;

        Quaternion finalRotation = Quaternion.Euler(90, 0, -yRotation);

        return finalRotation;
    }

    private Vector3 GetArrowPosition()
    {
        Vector3 arrowPosition = transform.position;
        arrowPosition.y += 1.0f;
        return arrowPosition;
    }
}
