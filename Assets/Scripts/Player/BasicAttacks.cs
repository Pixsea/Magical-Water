using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttacks : MonoBehaviour
{
    private GameObject bm;  //Battle manager
    private Phase battlePhase;

    // Animator for attacking
    [SerializeField]
    private GameObject pSprite;  // tell the animator script to attack

    [SerializeField]
    private GameObject projectile;
    [SerializeField]
    private GameObject rapid_projectile;
    [SerializeField]
    private GameObject spread_projectile;
    [SerializeField]
    private GameObject spray_projectile;
    [SerializeField]
    private GameObject precision_projectile;


    // Keys used for multi-directional shooting/Fire3
    [SerializeField]
    private string Attackkey;
    [SerializeField]
    private string Upkey;
    [SerializeField]
    private string Leftkey;
    [SerializeField]
    private string Downkey;
    [SerializeField]
    private string Rightkey;
    [SerializeField]
    private string Switchkey;
    [SerializeField]
    private string Guardkey;  // So you can't shoot while guarding

    // Return true when held down
    private bool Attackpress;
    private bool Uppress;
    private bool Leftpress;
    private bool Downpress;
    private bool Rightpress;
    private bool Guardpress;

    //return true on frame pushed down
    private bool Switch;
    private bool Switch2;  // Used so that it rememebers when a key is pressed down if an update frame happens without a fixed update

    private Vector2 direction;
    private int mode;
    private float cooldown;

    public List<GameObject> bullets = new List<GameObject>();  // List used to destroy all bullets


    // Start is called before the first frame update
    void Start()
    {
        bm = GameObject.Find("BattleManager");

        mode = 0;
        cooldown = 0f;
        Switch2 = false;
    }

    // Update is called once per frame
    void Update()
    {
        Attackpress = Input.GetKey(Attackkey);
        Uppress = Input.GetKey(Upkey);
        Leftpress = Input.GetKey(Leftkey);
        Downpress = Input.GetKey(Downkey);
        Rightpress = Input.GetKey(Rightkey);

        Switch = Input.GetKeyDown(Switchkey);

        if (Switch)
        {
            Switch2 = true;  // Remeber if true for fixed update, will get set back to false at first fixed update
        }

        Guardpress = Input.GetKey(Guardkey);
    }

    void FixedUpdate()
    {
        battlePhase = bm.GetComponent<BattleManager>().currPhase;

        // If not in combat, prevent attacking
        if (battlePhase != Phase.Combat)
        {
            Attackpress = false;
            Uppress = false;
            Leftpress = false;
            Downpress = false;
            Rightpress = false;
            Switch2 = false;
        }


        if (Switch2)
        {
            Switch2 = false;

            if (mode == 4)
            {
                mode = 0;
            }

            else
            {
                mode += 1;
            }
        }

        
        // Rotate to direction pressed
        if (Uppress | Leftpress | Downpress | Rightpress)
        {
            FreeRotate();
        }


        // Attacks in front of the player, should be used with Movement2
        if (Attackpress && (cooldown <= 0) && !Guardpress)
        {
            Attack(transform.rotation);
            pSprite.SendMessage("Attack");
        }


        // Attacks in direction pressed
        //if ((Uppress | Leftpress | Downpress | Rightpress) && (cooldown <= 0))
        //{
        //    FreeAttack();
        //    pSprite.SendMessage("Attack");
        //}

        cooldown--;
    }



    void Fire()
    {
        Quaternion temp = Quaternion.Euler(0, 0, 0);

        // Check direction
        if (BasicMovement.playerDirection == 1)
        {
            temp = Quaternion.Euler(0, 0, 0);
        }

        else if (BasicMovement.playerDirection == 2)
        {
            temp = Quaternion.Euler(0, 0, 315);
        }

        else if (BasicMovement.playerDirection == 3)
        {
            temp = Quaternion.Euler(0, 0, 270);
        }

        else if (BasicMovement.playerDirection == 4)
        {
            temp = Quaternion.Euler(0, 0, 225);
        }

        else if (BasicMovement.playerDirection == 5)
        {
            temp = Quaternion.Euler(0, 0, 180);
        }

        else if (BasicMovement.playerDirection == 6)
        {
            temp = Quaternion.Euler(0, 0, 135);
        }

        else if (BasicMovement.playerDirection == 7)
        {
            temp = Quaternion.Euler(0, 0, 90);
        }

        else if (BasicMovement.playerDirection == 8)
        {
            temp = Quaternion.Euler(0, 0, 45);
        }


        // Making bullet spawn at the correct rotation, see first link in google doc
        //Vector3 tempDirection = Quaternion.Euler(90, 0, 0) * temp;
        //Quaternion tempDirection2 = Quaternion.LookRotation(Vector3.forward, tempDirection);
        //GameObject bullet = Instantiate(projectile, transform.position, tempDirection2) as GameObject;

        GameObject bullet = Instantiate(projectile, transform.position, temp) as GameObject;
    }



    void Attack(Quaternion direction)
    {
        // Given an quaternion roation, attacks in that direction
        if (mode == 0)
        {
            BasicShot(direction);
        }

        else if (mode == 1)
        {
            RapidShot(direction);
        }

        else if (mode == 2)
        {
            SpreadShot(direction);
        }

        else if (mode == 3)
        {
            SprayShot(direction);
        }

        else if (mode == 4)
        {
            PrecisionShot(direction);
        }
    }



    void FreeAttack()
    {
        // Attacks in the direction pressed, can attack in an independent direction from what you're moving

        Quaternion temp = Quaternion.Euler(0, 0, 0);

        if (Uppress && !Downpress && !Leftpress && !Rightpress)
        {
            temp = Quaternion.Euler(0, 0, 0);
        }

        else if (Uppress && !Downpress && !Leftpress && Rightpress)
        {
            temp = Quaternion.Euler(0, 0, 315);
        }

        else if (!Uppress && !Downpress && !Leftpress && Rightpress)
        {
            temp = Quaternion.Euler(0, 0, 270);
        }

        else if (!Uppress && Downpress && !Leftpress && Rightpress)
        {
            temp = Quaternion.Euler(0, 0, 225);
        }

        else if (!Uppress && Downpress && !Leftpress && !Rightpress)
        {
            temp = Quaternion.Euler(0, 0, 180);
        }

        else if (!Uppress && Downpress && Leftpress && !Rightpress)
        {
            temp = Quaternion.Euler(0, 0, 135);
        }

        else if (!Uppress && !Downpress && Leftpress && !Rightpress)
        {
            temp = Quaternion.Euler(0, 0, 90);
        }

        else if (Uppress && !Downpress && Leftpress && !Rightpress)
        {
            temp = Quaternion.Euler(0, 0, 45);
        }

        
        Attack(temp);
    }



    void FreeRotate()
    {
        // Rotate to the direction pressed, can turn in an independent direction from what you're moving

        Quaternion temp = Quaternion.Euler(0, 0, 0);

        if (Uppress && !Downpress && !Leftpress && !Rightpress)
        {
            temp = Quaternion.Euler(0, 0, 0);
        }

        else if (Uppress && !Downpress && !Leftpress && Rightpress)
        {
            temp = Quaternion.Euler(0, 0, 315);
        }

        else if (!Uppress && !Downpress && !Leftpress && Rightpress)
        {
            temp = Quaternion.Euler(0, 0, 270);
        }

        else if (!Uppress && Downpress && !Leftpress && Rightpress)
        {
            temp = Quaternion.Euler(0, 0, 225);
        }

        else if (!Uppress && Downpress && !Leftpress && !Rightpress)
        {
            temp = Quaternion.Euler(0, 0, 180);
        }

        else if (!Uppress && Downpress && Leftpress && !Rightpress)
        {
            temp = Quaternion.Euler(0, 0, 135);
        }

        else if (!Uppress && !Downpress && Leftpress && !Rightpress)
        {
            temp = Quaternion.Euler(0, 0, 90);
        }

        else if (Uppress && !Downpress && Leftpress && !Rightpress)
        {
            temp = Quaternion.Euler(0, 0, 45);
        }

        transform.rotation = temp;
    }



    void Reset2()
    {
        mode = 0;
        cooldown = 0f;

        //foreach (GameObject bullet in bullets)
        //{
        //    Destroy(bullet);
        //}
    }



    ////////////////////
    // Start of Shots //
    ////////////////////



    void BasicShot(Quaternion direction)
    {
        GameObject bullet1 = Instantiate(projectile, transform.position, direction) as GameObject;
        GameObject bullet2 = Instantiate(projectile, transform.position, direction * Quaternion.Euler(0, 0, 7)) as GameObject;
        GameObject bullet3 = Instantiate(projectile, transform.position, direction * Quaternion.Euler(0, 0, -7)) as GameObject;

        bullets.Add(bullet1);
        bullets.Add(bullet2);
        bullets.Add(bullet3);

        Physics2D.IgnoreCollision(bullet1.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(bullet2.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(bullet3.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        cooldown = .6f / Time.fixedDeltaTime;
    }



    void RapidShot(Quaternion direction)
    {
        GameObject bullet = Instantiate(rapid_projectile, transform.position, direction) as GameObject;
        bullets.Add(bullet);
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        cooldown = .2f / Time.fixedDeltaTime;
    }



    void SpreadShot(Quaternion direction)
    {
        GameObject bullet1 = Instantiate(spread_projectile, transform.position, direction) as GameObject;
        GameObject bullet2 = Instantiate(spread_projectile, transform.position, direction * Quaternion.Euler(0, 0, 10)) as GameObject;
        GameObject bullet3 = Instantiate(spread_projectile, transform.position, direction * Quaternion.Euler(0, 0, -10)) as GameObject;
        GameObject bullet4 = Instantiate(spread_projectile, transform.position, direction * Quaternion.Euler(0, 0, 20)) as GameObject;
        GameObject bullet5 = Instantiate(spread_projectile, transform.position, direction * Quaternion.Euler(0, 0, -20)) as GameObject;
        GameObject bullet6 = Instantiate(spread_projectile, transform.position, direction * Quaternion.Euler(0, 0, 30)) as GameObject;
        GameObject bullet7 = Instantiate(spread_projectile, transform.position, direction * Quaternion.Euler(0, 0, -30)) as GameObject;

        bullets.Add(bullet1);
        bullets.Add(bullet2);
        bullets.Add(bullet3);
        bullets.Add(bullet4);
        bullets.Add(bullet5);
        bullets.Add(bullet6);
        bullets.Add(bullet7);

        Physics2D.IgnoreCollision(bullet1.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(bullet2.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(bullet3.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(bullet4.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(bullet5.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(bullet6.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(bullet7.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        cooldown = 1.25f / Time.fixedDeltaTime;
    }



    void SprayShot(Quaternion direction)
    {
        Quaternion temp = Quaternion.Euler(0, 0, Random.Range(-30.0f, 30.0f));
        GameObject bullet = Instantiate(spray_projectile, transform.position, direction * temp) as GameObject;
        bullets.Add(bullet);
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        cooldown = .1f / Time.fixedDeltaTime;
    }



    void PrecisionShot(Quaternion direction)
    {
        GameObject bullet = Instantiate(precision_projectile, transform.position, direction) as GameObject;
        bullets.Add(bullet);
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        cooldown = 2.5f / Time.fixedDeltaTime;
    }
}
