using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    [SerializeField]
    private float speed = 10;
    private Vector2 velocity;  //Vector controlling movement


    private Rigidbody2D rb;
    private BasicAttacks ba;

    private float horizontalInput;
    private float verticalInput;


    // For animations
    [SerializeField]
    //private Animator animator;



    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        ba = GetComponent<BasicAttacks>();
    }

    // Update is called once per frame
    void Update()
    {
        horizontalInput = Input.GetAxisRaw("Horizontal");
        verticalInput = Input.GetAxisRaw("Vertical");

        //animator.SetFloat("Horizontal", rb.velocity.x);
        //animator.SetFloat("Vertical", rb.velocity.y);
        //animator.SetFloat("Speed", rb.velocity.sqrMagnitude);
    }

    private void FixedUpdate()
    {
        Movement();
    }

    void Movement()
    {
        velocity = new Vector2(horizontalInput * speed, verticalInput * speed);
        rb.velocity = velocity;



        if (horizontalInput != 0 || verticalInput != 0)
        {
            // If not in attack cooldown, rotate towards the direction moving
            if (ba.canAttack)
            {
                float angle = Mathf.Atan2(velocity.y, velocity.x) * Mathf.Rad2Deg;
                Quaternion newDirection = Quaternion.Euler(new Vector3(0, 0, angle - 90));
                gameObject.transform.rotation = Quaternion.RotateTowards(gameObject.transform.rotation, newDirection, 30);
            }
        }
    }
}
