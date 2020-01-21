using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wizard : MonsterBasic
{
    public GameObject fireballPrefab;

    protected override void Attack(Collider player)
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
