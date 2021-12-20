using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicBullet : Bullet
{
    //[SerializeField]
    //private int power;
    //[SerializeField]
    //private float speed;
    //[SerializeField]
    //private float timer;
    //[SerializeField]
    //private string type;

    private Vector2 velocity;

    private GameObject parent;

    // Start is called before the first frame update
    void Start()
    {
        // Set to one if not modified
        if (powerMultiplier == 0f)
        {
            powerMultiplier = 1f;
        }

        if (speedMultiplier == 0f)
        {
            speedMultiplier = 1f;
        }


        timer = timer / Time.fixedDeltaTime;
        speed = speed * Time.fixedDeltaTime;

        parent = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        //if (timer <= 0)
        //{
        //    DestroyObject(gameObject);
        //}

        //timer--;

        //velocity = new Vector2(0.0f, speed);
        //transform.Translate(velocity * Time.deltaTime);
    }

    void FixedUpdate()
    {
        if (timer <= 0)
        {
            Dead();
        }

        timer--;

        velocity = new Vector2(0.0f, speed);
        //transform.Translate(velocity * Time.deltaTime);
        transform.Translate(velocity);
        //rb.velocity = velocity;
    }



    void OnCollisionEnter2D(Collision2D hitObj)
    {
        if (hitObj.gameObject.tag == "Enemy")
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), hitObj.gameObject.GetComponent<Collider2D>());
            //hitObj.gameObject.SendMessage("Damage", power);
            hitObj.gameObject.GetComponent<Enemy>().Damage(power);
            Dead();
        }

        else if ((hitObj.gameObject.tag == "Player") | (hitObj.gameObject.tag == "Bullet") | (hitObj.gameObject.tag == "Edge"))
        {
            Physics2D.IgnoreCollision(GetComponent<Collider2D>(), hitObj.gameObject.GetComponent<Collider2D>());
        }

        else
        {
            Dead();
        }
    }



    // Enemies are triggers so hurt them here
    void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.gameObject.tag == "Enemy")
        {
            //obj.gameObject.SendMessage("Damage", power);
            obj.gameObject.GetComponent<Enemy>().Damage(power);
            Dead();
        }
    }



    // When a bullet hits a wall
    public override void HitWall()
    {
        Dead();
    }



    // When hits an enemy/wall, remove self from bullet list and destroy self
    public override void Dead()
    {
        DestroyObject(gameObject);
    }
}
