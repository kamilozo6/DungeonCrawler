using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float moveSpeed;
    private float length;

    void Start()
    {
        length = 100;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Translate(Vector3.up * moveSpeed * Time.deltaTime);
        length -= moveSpeed * Time.deltaTime;
        if(length<=0)
        {
            Destroy(gameObject);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.tag.Equals("Player"))
        {
            // TODO damage player
        }

        Destroy(gameObject);
    }
}
