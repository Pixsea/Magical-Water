using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ember2 : MonoBehaviour
{
    private GameObject bm;  //Battle manager
    private Phase battlePhase;

    private float spawnX;
    private float spawnY;
    private float spawnRotation;

    private int currMove;
    [SerializeField]
    private int preferedMove;  //If not -1 (null), will always do the move corresponding to the given int

    private GameObject player;

    [SerializeField]
    private GameObject boundary;

    [SerializeField]
    private GameObject projectile;

    private int health;

    [SerializeField]
    private float turnSpeed;
    [SerializeField]
    private float aimPeriod;
    [SerializeField]
    private float chargePeriod;
    [SerializeField]
    private float cooldownPeriod;

    private float speed;
    private Vector3 velocity;  //Vector controlling movement
    private Rigidbody2D rb;
    private Rigidbody2D brb;  // Boundary rigid body

    private float cooldown;
    private float angle;
    private Quaternion angle2;

    // What state the enemy is in, 0 = aiming, 1 = charging, 2 = shooting, 3 = cooldown
    [HideInInspector]
    public int state;

    [SerializeField]
    private int fireballsToShoot;  // How many fireballs it shoots in one volley
    private int fireballCount;  //How many fireballs shot in a volley
    [SerializeField]
    private float firballDelay;  // Delay between fireball shots

    // Use to trigger attack animation
    [SerializeField]
    private Animator eAnimator;



    // Start is called before the first frame update
    void Start()
    {
        bm = GameObject.Find("Manager");

        spawnX = transform.position.x;
        spawnY = transform.position.y;
        spawnRotation = transform.rotation.eulerAngles.z;

        speed = gameObject.GetComponent<EnemyStats>().speed;

        turnSpeed = turnSpeed * Time.fixedDeltaTime;
        speed = speed * Time.fixedDeltaTime * .01f;
        aimPeriod = aimPeriod / Time.fixedDeltaTime;
        chargePeriod = chargePeriod / Time.fixedDeltaTime;
        cooldownPeriod = cooldownPeriod / Time.fixedDeltaTime;

        rb = GetComponent<Rigidbody2D>();
        brb = boundary.GetComponent<Rigidbody2D>();

        player = GameObject.FindGameObjectWithTag("Player");
        cooldown = aimPeriod;
        state = 0;

        fireballCount = 0;
        firballDelay = firballDelay / Time.fixedDeltaTime;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        transform.position = boundary.transform.position;

        battlePhase = bm.GetComponent<BattleManager>().currPhase;

        if ((battlePhase != Phase.Combat))
        {
            //rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        else if ((battlePhase == Phase.Combat) && (currMove == 1))
        {
            //rb.constraints = RigidbodyConstraints2D.None;
            AttackUpdate();
        }

        else if ((battlePhase == Phase.Combat) && (currMove == 2))
        {
            //rb.constraints = RigidbodyConstraints2D.None;
            SweepAttackUpdate();
        }


        health = gameObject.GetComponent<EnemyStats>().health;

        if (health <= 0)
        {
            bm.GetComponent<BattleManager>().enemies.Remove(gameObject);
            Dead();
        }
    }



    void Setup()
    {
        ChooseMove();
    }



    void Reset()
    {
        transform.position = new Vector3(spawnX, spawnY);
        transform.rotation = Quaternion.Euler(0, 0, spawnRotation);

        boundary.transform.position = new Vector3(spawnX, spawnY);
        boundary.transform.rotation = Quaternion.Euler(0, 0, spawnRotation);

        cooldown = aimPeriod;
        state = 0;
        eAnimator.SetInteger("state", state);
        fireballCount = 0;
    }



    void Damage(int damage)
    {
        health -= damage;
        gameObject.GetComponent<EnemyStats>().health = health;
    }

    void Dead()
    {
        DestroyObject(gameObject);
    }



    void ChooseMove()
    {
        if (preferedMove != -1)
        {
            currMove = preferedMove;
        }

        else
        {
            float temp = Random.Range(-1.0f, 1.0f);

            if (temp <= 0)
            {
                currMove = 1;
            }
            else
            {
                currMove = 2;
            }
        }
    }



    // Moves side to shot aiming at the player and shooting periodically
    void AttackUpdate()
    {
        velocity = Vector3.zero;

        if (state == 0)
        {
            Move();
            Turn();
        }
        //Debug.Log(velocity);
        //brb.velocity = velocity;
        //Debug.Log(brb.velocity);

        if (state == 1)
        {
            //TurnSlow();
            //Turn();
        }


        // switch state
        if (cooldown <= 0)
        {
            // Idle -> Charge
            if (state == 0)
            {
                cooldown = chargePeriod;
                state = 1;
                eAnimator.SetInteger("state", state);
            }

            // Charging -> Shooting
            else if (state == 1)
            {
                cooldown = firballDelay;
                state = 2;
                eAnimator.SetInteger("state", state);
                fireballCount = 0;
            }

            // Shooting
            else if (state == 2)
            {
                // Continue Shooting
                if (fireballCount < fireballsToShoot)
                {
                    Attack();
                    fireballCount += 1;
                    cooldown = firballDelay;
                }

                // Shooting -> Cooldown
                else
                {
                    cooldown = cooldownPeriod;
                    state = 3;
                    eAnimator.SetInteger("state", state);
                }
            }

            // Cooldown -> Aiming
            else if (state == 3)
            {
                cooldown = aimPeriod;
                state = 0;
                eAnimator.SetInteger("state", state);
            }
        }

        cooldown--;
    }



    // Moves towards an edge, then moves side to side, aims straight and shoots in bursts
    void SweepAttackUpdate()
    {
    }



    void Move()
    {
        Vector3 direction = player.transform.position - transform.position;
        angle = (Mathf.Atan2(direction.x, direction.y)) * (180 / Mathf.PI);
        angle = 0 - angle;
        angle2 = Quaternion.Euler(new Vector3(0, 0, angle));

        //Debug.Log(angle2 * transform.forward * speed);
        //velocity = angle2 * -Vector3.up * speed;
        brb.velocity = angle2 * -Vector3.up * speed;
        //Debug.Log(brb.velocity);
    }



    void Turn()
    {
        Vector3 direction = player.transform.position - transform.position;
        angle = (Mathf.Atan2(direction.x, direction.y)) * (180 / Mathf.PI);
        angle = 0 - angle;
        angle2 = Quaternion.Euler(new Vector3(0, 0, angle));

        //Debug.Log(angle);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, angle2, turnSpeed);
    }



    void TurnSlow()
    {
        Vector3 direction = player.transform.position - transform.position;
        angle = (Mathf.Atan2(direction.x, direction.y)) * (180 / Mathf.PI);
        angle = 0 - angle;
        angle2 = Quaternion.Euler(new Vector3(0, 0, angle));

        //Debug.Log(angle);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, angle2, turnSpeed / 4);
    }



    void Attack()
    {
        Quaternion temp = Quaternion.Euler(0, 0, Random.Range(-15.0f, 15.0f));
        GameObject bullet = Instantiate(projectile, transform.position, transform.rotation * temp) as GameObject;
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        //bullets.Add(bullet);

        eAnimator.SetTrigger("attack");
    }
}
