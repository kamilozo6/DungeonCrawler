using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public float moveSpeed;
    public float fireProbability;
    public bool canApplyEffect;

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.forward * moveSpeed * Time.deltaTime);
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            // TODO damage player
            if (Random.Range(0.0f, 1.0f) <= fireProbability && canApplyEffect)
            {
                // TODO apply effect to player
                print("XD");
            }
        }

        Destroy(gameObject);
    }
}
