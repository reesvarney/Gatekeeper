using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class magicParticle : MonoBehaviour
{
    public float playerFollowDistance = 5f;
    public float movementSpeed = 50f;
    public int value = 10;
    public Transform playerTransform;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector2.Distance(transform.position, playerTransform.position) < playerFollowDistance){
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            Vector2 force = direction * movementSpeed * Time.deltaTime;
            rb.AddForce(force);
        }
    }
}
