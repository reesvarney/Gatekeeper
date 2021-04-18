using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class playerMovement : MonoBehaviour
{
    public float speed = 3f;
    private Rigidbody2D myBody;
    private Animator anim;
    private Vector3 origScale;    

    void Awake()
    {
        myBody = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        origScale = transform.localScale;
        myBody.freezeRotation = true;
        anim.SetBool("isMoving", false);
    }

    void ChangeDirection(int direction)
    {
        Vector3 tempScale = transform.localScale;
        tempScale.x = direction * origScale.x;
        transform.localScale = tempScale;
    }


    void PlayerMove()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        if (h == 0)
        {
            anim.SetBool("isMoving", false);
        }
        else
        {
            anim.SetBool("isMoving", true);
            if (h > 0)
            {
                ChangeDirection(-1);
            }
            else if (h < 0)
            {
                ChangeDirection(1);
            }
        };
        myBody.velocity = new Vector2(h * speed, v * speed);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        PlayerMove();
    }
}
