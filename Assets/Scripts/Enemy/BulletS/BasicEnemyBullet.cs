using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemyBullet : Bullet
{
    //[SerializeField]
    //private int power;
    //private int shieldPower;
    //[SerializeField]
    //private float speed;
    //[SerializeField]
    //private float timer;
    //[SerializeField]
    //private string type;

    private Vector2 velocity;

    // Start is called before the first frame update
    void Start()
    {
        // Get all the values from the bullet stat script
        //power = Mathf.RoundToInt(gameObject.GetComponent<BulletStats>().power * gameObject.GetComponent<BulletStats>().powerMultiplier);
        //shieldPower = gameObject.GetComponent<BulletStats>().shieldPower;
        //speed = gameObject.GetComponent<BulletStats>().speed * gameObject.GetComponent<BulletStats>().speedMultiplier;
        //timer = gameObject.GetComponent<BulletStats>().timer;
        //type = gameObject.GetComponent<BulletStats>().type;


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
        speed = speed * Time.fixedDeltaTime * speedMultiplier;

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
    }



    //int hitPlayer()
    //{
    //    return power;
    //}


    // From when bullets told the player they were damaged instead of the player checking if a bullet touched them
    // Changed becuase bullets having a collider could push the player

    //void OnCollisionEnter2D(Collision2D hitObj)
    //{
    //    if (hitObj.gameObject.tag == "Player")
    //    {
    //        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), hitObj.gameObject.GetComponent<Collider2D>());
    //        hitObj.gameObject.SendMessage("Damage", power);
    //        DestroyObject(gameObject);
    //    }

    //    else if (hitObj.gameObject.tag == "Enemy")
    //    {
    //        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), hitObj.gameObject.GetComponent<Collider2D>());
    //    }

    //    if (hitObj.gameObject.tag == "Bullet")
    //    {
    //        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), hitObj.gameObject.GetComponent<Collider2D>());
    //    }

    //    else
    //    {
    //        DestroyObject(gameObject);
    //    }

    //    if (hitObj.gameObject.tag != "Player")
    //    {
    //        Dead();
    //    }
    //}



    // Destroy self if hits collider, for when bullets checked if they hit something
    //void OnTriggerEnter2D(Collider2D obj)
    //{
    //    //if ((obj.gameObject.tag != "Bullet") && (obj.gameObject.tag != "Edge") && (obj.gameObject.tag != "Enemy"))
    //    //{
    //    //    Collided();
    //    //}
    //}



    // When a bullet hits a player
    public override void HitPlayer(GameObject player)
    {
        player.GetComponent<PlayerStats>().ApplyDamage((int) Mathf.Ceil(power * powerMultiplier));
        Dead();
    }

    // When a bullet hits a shield
    public override void HitShield(GameObject shield)
    {
        shield.GetComponent<Guard>().ApplyShieldDamage((int) Mathf.Ceil(shieldPower * powerMultiplier));
        Dead();
    }

    // When a bullet hits a wall
    public override void HitWall()
    {
        Dead();
    }



    public override void Dead()
    {
        DestroyObject(gameObject);
    }
}
