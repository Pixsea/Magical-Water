using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boundary : MonoBehaviour
{
    public GameObject parent;

    // Start is called before the first frame update
    void Start()
    {
        //GameObject player = GameObject.FindGameObjectWithTag("Player");
        //Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), player.GetComponent<CircleCollider2D>());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = parent.transform.position;

        //Rigidbody2D rb = GetComponent<Rigidbody2D>();

        //if (parent.GetComponent<Ember2>().state != 0)
        //{
        //    Debug.Log("TEST");
        //    rb.constraints = RigidbodyConstraints2D.FreezeAll;
        //}
        //else
        //{
        //    rb.constraints = RigidbodyConstraints2D.None;
        //}
        
    }



    //void OnCollisionEnter2D(Collision2D obj)
    //{
    //    if (obj.gameObject.tag != "Wall")
    //    {
    //        Debug.Log("TEST");
    //        Physics2D.IgnoreCollision(GetComponent<CircleCollider2D>(), obj.gameObject.GetComponent<CircleCollider2D>());
    //    }
    //}



    void OnTriggerEnter2D(Collider2D obj)
    {
        // If touching a wall don't move
        if (obj.gameObject.tag == "Wall")
        {
            parent.SendMessage("HitWall");
        }
    }
}
