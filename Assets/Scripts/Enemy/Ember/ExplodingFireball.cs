using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplodingFireball : Bullet
{
    private GameObject player;

    //private int power;
    //private int shieldPower;
    //private float speed;
    //private float timer;
    private Vector2 velocity;

    [SerializeField]
    private float blastRadius;
    [SerializeField]
    private float explosionTime;  // How long the explosion lasts
    //[SerializeField]
    //private float explosionMaxSize;  // How big to increase the scale to for the sprite
    
    [HideInInspector]
    public float initialSpeed;  // The initial speed of the fireball based on the distance between the enemy and the player

    private bool exploding;

    // Use to trigger attack animation
    [SerializeField]
    private Animator eAnimator;



    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");

        // get all the values from the bullet stat script
        //power = gameObject.GetComponent<BulletStats>().power;
        //shieldPower = gameObject.GetComponent<BulletStats>().shieldPower;
        //speed = gameObject.GetComponent<BulletStats>().speed;
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
        explosionTime = explosionTime / Time.fixedDeltaTime;

        if (initialSpeed < 8)
        {
            initialSpeed = 8f;
        }
        speed = initialSpeed * Time.fixedDeltaTime;

        exploding = false;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        Vector3 direction = player.transform.position - transform.position;
        Debug.DrawRay(transform.position, Vector3.Normalize(direction) * blastRadius, Color.blue);

        if ((timer <= 0) && (exploding == false))
        {
            Explode();
        }
        else if (timer <= 0)
        {
            Dead();
        }

        timer--;

        // Only move if it's not exploding
        if (exploding == false)
        {
            velocity = new Vector2(0.0f, speed);
            transform.Translate(velocity);

            //slow down over time
            //speed -= .003f;
            if (speed > .1)
            {
                speed = speed * .95f;
            }

            else if ( speed > 0)
            {
                speed = speed - .003f;
            }
            
            else if (speed <= 0)
            {
                speed = 0;
            }
        }

        //else
        //{
        //    if (transform.localScale.x < explosionMaxSize)
        //    {
        //        transform.localScale += new Vector3(5, 5, 0);
        //    }
        //}
    }



    // When a bullet hits a player
    public override void HitPlayer(GameObject player)
    {
        //player.GetComponent<PlayerStats>().ApplyDamage(power);
        Explode();
    }

    // When a bullet hits a shield
    public override void HitShield(GameObject shield)
    {
        //shield.GetComponent<Guard>().ApplyShieldDamage(power);
        Explode();
    }

    // When a bullet hits a wall
    public override void HitWall()
    {
        Explode();
    }



    public override void Dead()
    {
        DestroyObject(gameObject);
    }



    void Explode()
    {
        eAnimator.SetTrigger("attack");
        exploding = true;
        timer = explosionTime;
        GetComponent<Collider2D>().enabled = false;

        Vector3 direction = player.transform.position - transform.position;
        //float angle = (Mathf.Atan2(direction.x, direction.y)) * (180 / Mathf.PI);
        //angle = 0 - angle;
        //Quaternion angle2 = Quaternion.Euler(new Vector3(0, 0, angle));

        //Debug.Log(direction);

        Debug.DrawRay(transform.position, Vector3.Normalize(direction) * blastRadius, Color.blue);
        RaycastHit2D hit = Physics2D.Raycast(transform.position, direction, blastRadius, 1 << LayerMask.NameToLayer("Shield"));
        if (hit.collider != null)
        {
            if (hit.collider.tag == "Shield")
            {
                player.GetComponent<Guard>().ApplyShieldDamage((int)Mathf.Ceil(shieldPower * powerMultiplier));
            }
        }

        // If it didn't hit a shield, look for player
        else
        {
            hit = Physics2D.Raycast(transform.position, direction, blastRadius, 1 << LayerMask.NameToLayer("Player"));
            if (hit.collider != null)
            {
                if (hit.collider.tag == "Player")
                {
                    player.GetComponent<PlayerStats>().ApplyDamage((int)Mathf.Ceil(power * powerMultiplier));
                }
            }
        }
    }
}
