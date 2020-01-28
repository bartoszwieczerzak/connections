using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ship : MonoBehaviour
{
    public int crew = 0;
    public float speed = 2f;

    public Planet source;
    public Planet target;

    public Rigidbody rb;

    private bool isAttacking = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isAttacking)
        {
            Vector3 heading = target.transform.position - source.transform.position;

            rb.velocity = heading * speed;

            isAttacking = false;
        }
    }

    public void Attack()
    {
        if (source && target && crew > 0)
        {
            isAttacking = true;
        }
    }
}
