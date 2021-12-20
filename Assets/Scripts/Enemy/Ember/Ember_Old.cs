using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ember_Old : MonoBehaviour
{
    private GameObject bm;  //Battle manager
    private Phase battlePhase;

    private float spawnX;
    private float spawnY;
    private float spawnRotation;

    private int currMove;  // 1 is charge flamethrower, 2 is fireballs
    [SerializeField]
    private int preferedMove;  // If not -1 (null), will always do the move corresponding to the given int

    // What state the enemy is in, 0 = aiming, 1 = charging, 2 = shooting, 3 = cooldown
    private int state;

    private GameObject player;

    //[SerializeField]
    //private GameObject boundary;

    [SerializeField]
    private GameObject projectile;
    [SerializeField]
    private GameObject explodingProjectile;
    [SerializeField]
    private GameObject aimingReticle;

    //private int health;

    [SerializeField]
    private float turnSpeed;
    [SerializeField]
    private float aimPeriod;
    [SerializeField]
    private float chargePeriod;
    [SerializeField]
    private float cooldownPeriod;

    private float speed;
    private Vector2 velocity;  //Vector controlling movement
    private Rigidbody2D rb;

    private bool atWall;  // When touches a wall, becomes true so doesn't move

    private float cooldown;
    private float angle;
    private Quaternion angle2;

    [SerializeField]
    private int flamesToShoot;  // How many flames it shoots in one volley
    private int flameCount;  //How many flames shot in a volley
    [SerializeField]
    private float flameDelay;  // Delay between flame shots

    [SerializeField]
    private int fireballsToShoot;  // How many flames it shoots in one volley
    private int fireballCount;  //How many flames shot in a volley
    [SerializeField]
    private float fireballDelay;  // Delay between flame shots

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

        turnSpeed = turnSpeed * Time.fixedDeltaTime;
        aimPeriod = aimPeriod / Time.fixedDeltaTime;
        chargePeriod = chargePeriod / Time.fixedDeltaTime;
        cooldownPeriod = cooldownPeriod / Time.fixedDeltaTime;

        rb = GetComponent<Rigidbody2D>();

        player = GameObject.FindGameObjectWithTag("Player");
        cooldown = aimPeriod;
        state = 0;

        flameCount = 0;
        flameDelay = flameDelay / Time.fixedDeltaTime;

        fireballCount = 0;
        fireballDelay = fireballDelay / Time.fixedDeltaTime;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        battlePhase = bm.GetComponent<BattleManager>().currPhase;


        speed = gameObject.GetComponent<EnemyStats>().speed;
        speed = speed * Time.fixedDeltaTime;

        if ((battlePhase != Phase.Combat))
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        else if ((battlePhase == Phase.Combat) && (currMove == 1))
        {
            rb.constraints = RigidbodyConstraints2D.None;
            StreamAttackUpdate();
        }

        else if ((battlePhase == Phase.Combat) && (currMove == 2))
        {
            rb.constraints = RigidbodyConstraints2D.None;
            MultiFireballAttackUpdate();
        }


        //health = gameObject.GetComponent<EnemyStats>().health;

        if (gameObject.GetComponent<EnemyStats>().health <= 0)
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

        atWall = false;

        cooldown = aimPeriod;
        state = 0;
        eAnimator.SetInteger("state", state);
        flameCount = 0;
        fireballCount = 0;
    }



    void Damage(int damage)
    {
        gameObject.GetComponent<EnemyStats>().health -= damage;
        gameObject.GetComponent<EnemyStats>().Flash("red");
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
    void StreamAttackUpdate()
    {
        //Debug.Log(Vector3.Distance(transform.position, player.transform.position));

        if (state == 0)
        {
            //if (atWall == false)
            //{
            //    Move();
            //}
            //else
            //{
            //    rb.velocity = Vector3.zero;
            //}

            // if distance is too great, don't move
            if (Vector3.Distance(transform.position, player.transform.position) < 3.5f)
            {
                Move();
            }
            else
            {
                rb.velocity = new Vector3(0, 0, 0);
            }

            Turn();
        }


        // switch state
        if (cooldown <= 0)
        {
            // Idle -> Charge
            if (state == 0)
            {
                cooldown = chargePeriod * 1.5f;
                state = 1;
                eAnimator.SetInteger("state", state);
                gameObject.GetComponent<EnemyStats>().Flicker("red", 5);

                MakeReticle(chargePeriod * 1.5f);

                // Stop movement
                rb.velocity = new Vector3(0, 0, 0);
            }

            // Charging -> Shooting
            else if (state == 1)
            {
                cooldown = flameDelay;
                state = 2;
                eAnimator.SetInteger("state", state);
                gameObject.GetComponent<EnemyStats>().StopFlicker();
                flameCount = 0;
            }

            // Shooting
            else if (state == 2)
            {
                // Continue Shooting
                if (flameCount < flamesToShoot)
                {
                    Attack();
                    flameCount += 1;
                    cooldown = flameDelay;
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
    void FireballAttackUpdate()
    {
        if (state == 0)
        {
            // if distance is too great, don't move
            if (Vector3.Distance(transform.position, player.transform.position) < 3.5f)
            {
                Move();
            }
            else if (Vector3.Distance(transform.position, player.transform.position) > 4.5f)
            {
                MoveAway();
            }
            else
            {
                rb.velocity = new Vector3(0, 0, 0);
            }

            Turn();
        }


        // switch state
        if (cooldown <= 0)
        {
            // Idle -> Charge
            if (state == 0)
            {
                cooldown = (chargePeriod / 2);
                state = 1;
                eAnimator.SetInteger("state", state);

                // Stop movement
                rb.velocity = new Vector3(0, 0, 0);
            }

            // Charging -> Shoot -> Cooldown (use the firing sprite for cooldown)
            else if (state == 1)
            {
                FireballAttack();
                cooldown = cooldownPeriod;
                state = 2;
                eAnimator.SetInteger("state", state);
            }

            // Cooldown -> Idle
            else if (state == 2)
            {
                cooldown = aimPeriod;
                state = 0;
                eAnimator.SetInteger("state", state);

                // Add random amount of time to fire rate to desync enemies
                float temp = Random.Range(0, 1f) / Time.fixedDeltaTime;
                cooldown += temp;
            }
        }

        cooldown--;
    }



    // Moves towards an edge, then moves side to side, aims straight and shoots in bursts
    void MultiFireballAttackUpdate()
    {
        if (state == 0)
        {
            if (fireballCount == 0)
            {
                // if distance is too great, is too close or too far, move so that is a certain distance away
                if (Vector3.Distance(transform.position, player.transform.position) < 3.5f)
                {
                    Move();
                }
                //else if (Vector3.Distance(transform.position, player.transform.position) > 4.5f)
                //{
                //    MoveAway();
                //}
                else
                {
                    rb.velocity = new Vector3(0, 0, 0);
                }
            }
                
            Turn();
        }


        // switch state
        if (cooldown <= 0)
        {
            // Idle -> Charge
            if (state == 0)
            {
                cooldown = (chargePeriod / 2f);
                state = 1;
                eAnimator.SetInteger("state", state);

                // Stop movement
                rb.velocity = new Vector3(0, 0, 0);
            }

            //// Charging -> Shoot -> Cooldown (use the firing sprite for cooldown)
            //else if (state == 1)
            //{
            //    FireballAttack();
            //    cooldown = cooldownPeriod;
            //    state = 2;
            //    eAnimator.SetInteger("state", state);
            //}

            // Charging -> Shooting
            else if (state == 1)
            {

                FireballAttack();
                cooldown = .15f / Time.fixedDeltaTime;
                state = 2;
                eAnimator.SetInteger("state", state);
            }

            // Shooting
            else if (state == 2)
            {
                // Continue Shooting
                if (fireballCount < fireballsToShoot)
                {
                    fireballCount += 1;
                    cooldown = fireballDelay;
                    state = 0;
                }

                // Shooting -> Cooldown
                else
                {
                    cooldown = cooldownPeriod;
                    state = 3;
                    //eAnimator.SetInteger("state", state);
                }
            }

            // Cooldown -> Idle
            else if (state == 3)
            {
                cooldown = aimPeriod;
                state = 0;
                fireballCount = 0;
                eAnimator.SetInteger("state", state);

                // Add random amount of time to fire rate to desync enemies
                float temp = Random.Range(0, 1f) / Time.fixedDeltaTime;
                cooldown += temp;
            }
        }

        cooldown--;
    }


    ///////////////////////////
    // End of Attack Updates //
    ///////////////////////////



    void Move()
    {
        Vector3 direction = player.transform.position - transform.position;
        angle = (Mathf.Atan2(direction.x, direction.y)) * (180 / Mathf.PI);
        angle = 0 - angle;
        angle2 = Quaternion.Euler(new Vector3(0, 0, angle));

        Vector3 velocity = angle2 * -Vector3.up * speed;
        //Debug.Log(velocity);

        // Draw raycast to detect wall collision
        RaycastHit2D hit;

        bool upWall = false;
        bool downWall = false;
        bool leftWall = false;
        bool rightWall = false;

        //Debug.DrawRay(transform.position, Vector2.up, Color.green);
        hit = Physics2D.Raycast(transform.position, Vector2.up, 1.5f, 1 << LayerMask.NameToLayer("Wall"));
        if (hit.collider != null)
        {
            upWall = true;

            if (velocity.y > 0)
            {
                // Give excess force going into a wall towards the other direction
                float temp = velocity.y;
                if ((velocity.x > 0) && !rightWall)
                {
                    velocity.x += temp / 3;
                }
                else if ((velocity.x < 0) && !leftWall)
                {
                    velocity.x -= temp / 3;
                }

                velocity.y = 0;
            }
        }

        //Debug.DrawRay(transform.position, -Vector3.up, Color.red);
        hit = Physics2D.Raycast(transform.position, -Vector2.up, 1.5f, 1 << LayerMask.NameToLayer("Wall"));
        if (hit.collider != null)
        {
            downWall = true;
            
            if (velocity.y < 0)
            {
                // Give excess force going into a wall towards the other direction
                float temp = -velocity.y;
                if ((velocity.x > 0) && !rightWall)
                {
                    velocity.x += temp / 3;
                }
                else if ((velocity.x < 0) && !leftWall)
                {
                    velocity.x -= temp / 3;
                }

                velocity.y = 0;
            }
        }

        //Debug.DrawRay(transform.position, Vector3.right, Color.blue);
        hit = Physics2D.Raycast(transform.position, Vector2.right, 1.5f, 1 << LayerMask.NameToLayer("Wall"));
        if (hit.collider != null)
        {
            rightWall = true;
            
            if (velocity.x > 0)
            {
                // Give excess force going into a wall towards the other direction
                float temp = velocity.x;
                if ((velocity.y > 0) && !upWall)
                {
                    velocity.y += temp / 3;
                }
                else if ((velocity.y < 0) && !downWall)
                {
                    velocity.y -= temp / 3;
                }

                velocity.x = 0;
            }
        }

        //Debug.DrawRay(transform.position, -Vector3.right, Color.yellow);
        hit = Physics2D.Raycast(transform.position, -Vector2.right, 1.5f, 1 << LayerMask.NameToLayer("Wall"));
        if (hit.collider != null)
        {
            leftWall = true;

            if (velocity.x < 0)
            {
                // Give excess force going into a wall towards the other direction
                float temp = -velocity.x;
                if ((velocity.y > 0) && !upWall)
                    {
                    velocity.y += temp / 3;
                }
                else if ((velocity.y < 0) && !downWall)
                {
                    velocity.y -= temp / 3;
                }

                velocity.x = 0;
            }
        }

        rb.velocity = velocity;
    }



    void MoveAway()
    {
        Vector3 direction = player.transform.position - transform.position;
        angle = (Mathf.Atan2(direction.x, direction.y)) * (180 / Mathf.PI);
        angle = 0 - angle;
        angle2 = Quaternion.Euler(new Vector3(0, 0, angle));

        Vector3 velocity = angle2 * Vector3.up * speed;
        //Debug.Log(velocity);

        // Draw raycast to detect wall collision
        RaycastHit2D hit;

        bool upWall = false;
        bool downWall = false;
        bool leftWall = false;
        bool rightWall = false;

        //Debug.DrawRay(transform.position, Vector2.up, Color.green);
        hit = Physics2D.Raycast(transform.position, Vector2.up, 1.5f, 1 << LayerMask.NameToLayer("Wall"));
        if (hit.collider != null)
        {
            upWall = true;

            if (velocity.y > 0)
            {
                // Give excess force going into a wall towards the other direction
                float temp = velocity.y;
                if ((velocity.x > 0) && !rightWall)
                {
                    velocity.x += temp / 3;
                }
                else if ((velocity.x < 0) && !leftWall)
                {
                    velocity.x -= temp / 3;
                }

                velocity.y = 0;
            }
        }

        //Debug.DrawRay(transform.position, -Vector3.up, Color.red);
        hit = Physics2D.Raycast(transform.position, -Vector2.up, 1.5f, 1 << LayerMask.NameToLayer("Wall"));
        if (hit.collider != null)
        {
            downWall = true;

            if (velocity.y < 0)
            {
                // Give excess force going into a wall towards the other direction
                float temp = -velocity.y;
                if ((velocity.x > 0) && !rightWall)
                {
                    velocity.x += temp / 3;
                }
                else if ((velocity.x < 0) && !leftWall)
                {
                    velocity.x -= temp / 3;
                }

                velocity.y = 0;
            }
        }

        //Debug.DrawRay(transform.position, Vector3.right, Color.blue);
        hit = Physics2D.Raycast(transform.position, Vector2.right, 1.5f, 1 << LayerMask.NameToLayer("Wall"));
        if (hit.collider != null)
        {
            rightWall = true;

            if (velocity.x > 0)
            {
                // Give excess force going into a wall towards the other direction
                float temp = velocity.x;
                if ((velocity.y > 0) && !upWall)
                {
                    velocity.y += temp / 3;
                }
                else if ((velocity.y < 0) && !downWall)
                {
                    velocity.y -= temp / 3;
                }

                velocity.x = 0;
            }
        }

        //Debug.DrawRay(transform.position, -Vector3.right, Color.yellow);
        hit = Physics2D.Raycast(transform.position, -Vector2.right, 1.5f, 1 << LayerMask.NameToLayer("Wall"));
        if (hit.collider != null)
        {
            leftWall = true;

            if (velocity.x < 0)
            {
                // Give excess force going into a wall towards the other direction
                float temp = -velocity.x;
                if ((velocity.y > 0) && !upWall)
                {
                    velocity.y += temp / 3;
                }
                else if ((velocity.y < 0) && !downWall)
                {
                    velocity.y -= temp / 3;
                }

                velocity.x = 0;
            }
        }

        rb.velocity = velocity;
    }



    void Turn()
    {
        Vector3 direction = player.transform.position - transform.position;
        angle = (Mathf.Atan2(direction.x, direction.y)) * (180 / Mathf.PI);
        angle = 0 - angle;
        angle2 = Quaternion.Euler(new Vector3(0, 0, angle));

        transform.rotation = Quaternion.RotateTowards(transform.rotation, angle2, turnSpeed);
    }



    void TurnSlow()
    {
        Vector3 direction = player.transform.position - transform.position;
        angle = (Mathf.Atan2(direction.x, direction.y)) * (180 / Mathf.PI);
        angle = 0 - angle;
        angle2 = Quaternion.Euler(new Vector3(0, 0, angle));

        transform.rotation = Quaternion.RotateTowards(transform.rotation, angle2, turnSpeed / 8);
    }



    void Attack()
    {
        Quaternion temp = Quaternion.Euler(0, 0, Random.Range(-7.5f, 7.5f));
        GameObject bullet = Instantiate(projectile, transform.position, transform.rotation * temp) as GameObject;
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        eAnimator.SetTrigger("attack");
    }



    void FireballAttack()
    {
        GameObject bullet = Instantiate(explodingProjectile, transform.position, transform.rotation) as GameObject;
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        bullet.GetComponent<ExplodingFireball>().initialSpeed = (Vector3.Distance(transform.position, player.transform.position)) * 2.5f; 

        //eAnimator.SetTrigger("attack");
    }



    void MakeReticle(float duration)
    {
        GameObject reticle = Instantiate(aimingReticle, transform.position, transform.rotation) as GameObject;
        reticle.GetComponent<TempSprite>().totalTime = duration;
    }
}
