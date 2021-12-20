using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary2 : MonoBehaviour
{
    public GameObject parent;
    private Rigidbody2D rb;

    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), player.GetComponent<CircleCollider2D>());
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (parent.GetComponent<Ember2>().state != 0)
        {
            //rb.isKinematic = true;
            //rb.constraints = RigidbodyConstraints2D.FreezeAll;
            //rb.velocity = Vector3.zero;
            //rb.angularVelocity = 0;
        }
        else
        {
            //rb.isKinematic = false;
            //rb.constraints = RigidbodyConstraints2D.None;
        }

    }



    void OnCollisionEnter2D(Collision2D obj)
    {
        if (obj.gameObject.tag != "Wall")
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), obj.gameObject.GetComponent<Collider2D>(), true);
        }

        else
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), obj.gameObject.GetComponent<Collider2D>(), false);
        }
    }



    //void OnTriggerEnter2D(Collider2D obj)
    //{
    //    // If touching a wall don't move
    //    if (obj.gameObject.tag == "Wall")
    //    {
    //        parent.SendMessage("HitWall");
    //    }
    //}
}
