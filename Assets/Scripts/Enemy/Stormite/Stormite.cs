using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stormite : Enemy
{

    // Projectiles
    [SerializeField]
    private GameObject projectile;
    [SerializeField]
    private GameObject overcastProjectile;

    private float bulletSpeedMultiplier;  // for when attacks need different speed bullet


    // Time periods
    [SerializeField]
    private float turnSpeed;
    [SerializeField]
    private float aimPeriod;
    [SerializeField]
    private float chargePeriod;
    [SerializeField]
    private float cooldownPeriod;
    [SerializeField]
    private float overcastPeriod;

    // For burst attacks
    [SerializeField]
    private int bulletsToShoot;  // How many bullets it shoots in one volley
    private int bulletCount;  //How many bullets shot in a volley
    [SerializeField]
    private float bulletDelay;  // Delay between bullet shots

    // For movement
    //[SerializeField]
    //private float speed;
    private Vector2 velocity;  //Vector controlling movement

    private float angle;
    private Quaternion angle2;

    // For sweeping attacks
    [SerializeField]
    private string axis;
    [SerializeField]
    private string startDir;
    private string currDir;
    [SerializeField]
    private string setupDir; // Which way to go initially if doing the sweep attack
    private bool setupMove;  // Determines if its moving towards and edge first to attack before going side to side, will turn false the moment it hits a wall, used for sweep attack

    // For the evasive movement
    private float movementTimer;
    private bool isMoving;
    [SerializeField]
    private float movementDelay;
    [SerializeField]
    private int totalMoves;
    private int movesLeft;
    private Vector2 targetLocation;
    private float targetRotation;

    // Boundary for random locations to move to
    [SerializeField]
    private float minX;
    [SerializeField]
    private float maxX;
    [SerializeField]
    private float minY;
    [SerializeField]
    private float maxY;

    // For overcast attack
    [SerializeField]
    private float cloudRotation;
    [SerializeField]
    private float minYCloud;
    [SerializeField]
    private float maxYCloud;



    // Start is called before the first frame update
    public override void EnemySetup()
    {
        turnSpeed = turnSpeed * Time.fixedDeltaTime;
        aimPeriod = aimPeriod / Time.fixedDeltaTime;
        chargePeriod = chargePeriod / Time.fixedDeltaTime;
        cooldownPeriod = cooldownPeriod / Time.fixedDeltaTime;
        overcastPeriod = overcastPeriod / Time.fixedDeltaTime;

        bulletCount = 0;
        bulletDelay = bulletDelay / Time.fixedDeltaTime;

        timer = aimPeriod;
        state = 0;

        movementDelay = movementDelay / Time.fixedDeltaTime;
        movementTimer = movementDelay;
        isMoving = true;
        movesLeft = totalMoves + 1; // The first movement needs one extra
        targetLocation = new Vector2(spawnX, spawnY);
        targetRotation = spawnRotation;

        bulletSpeedMultiplier = 1f;
    }

    // Update is called once per frame
    void Update()
    {

    }

    void FixedUpdate()
    {
        battlePhase = bm.GetComponent<BattleManager>().currPhase;
        ModifyStats();

        if ((battlePhase != Phase.Combat))
        {
            rb.constraints = RigidbodyConstraints2D.FreezeAll;
        }

        else if ((battlePhase == Phase.Combat) && (currMove == 1))
        {
            rb.constraints = RigidbodyConstraints2D.None;
            SweepAttackUpdate();
        }

        else if ((battlePhase == Phase.Combat) && (currMove == 2))
        {
            rb.constraints = RigidbodyConstraints2D.None;
            SweepSingleAttackUpdate();
        }

        else if ((battlePhase == Phase.Combat) && (currMove == 3))
        {
            rb.constraints = RigidbodyConstraints2D.None;
            EvasiveAttackUpdate();
        }

        else if ((battlePhase == Phase.Combat) && (currMove == 4))
        {
            rb.constraints = RigidbodyConstraints2D.None;
            EvasiveSingleAttackUpdate();
        }

        else if ((battlePhase == Phase.Combat) && (currMove == 5))
        {
            rb.constraints = RigidbodyConstraints2D.None;
            SupportAttackUpdate();
        }

        else if ((battlePhase == Phase.Combat) && (currMove == 6))
        {
            rb.constraints = RigidbodyConstraints2D.None;
            OvercastAttackUpdate();
        }

        if (health <= 0)
        {
            bm.GetComponent<BattleManager>().enemies.Remove(gameObject);
            Dead();
        }

        ColorUpdate();
    }



    public override void Setup()
    {
        currDir = "U";

        ChooseMove();


        // Set the current direction to be the prefered direction, if two are put, pick randomly
        if (startDir == "L")
        {
            currDir = "L";
        }

        else if (startDir == "R")
        {
            currDir = "R";
        }

        else if (startDir == "U")
        {
            currDir = "U";
        }

        else if (startDir == "D")
        {
            currDir = "D";
        }

        else if (axis == "LR")
        {
            float temp = Random.Range(-1.0f, 1.0f);

            if (temp >= 0)
            {
                currDir = "L";
            }
            else
            {
                currDir = "R";
            }
        }

        else if (axis == "UD")
        {
            float temp = Random.Range(-1.0f, 1.0f);

            if (temp >= 0)
            {
                currDir = "U";
            }
            else
            {
                currDir = "D";
            }
        }
    }



    public override void Reset()
    {
        transform.position = new Vector3(spawnX, spawnY);
        transform.rotation = Quaternion.Euler(0, 0, spawnRotation);
        timer = aimPeriod;
        state = 0;
        movementTimer = movementDelay;
        isMoving = true;
        movesLeft = totalMoves + 1; // The first movement needs one extra
        targetLocation = new Vector2(spawnX, spawnY);
        bulletSpeedMultiplier = 1f;
        eAnimator.SetInteger("state", state);
    }



    void ChooseMove()
    {
        if (preferedMove != -1)
        {
            currMove = preferedMove;
        }

        else
        {
            List<int> moves = new List<int>();
            moves.Add(1); // Sweep
            moves.Add(2); // Sweep single
            moves.Add(3); // Evasive
            moves.Add(4); // Evasive single
            moves.Add(6); // Overcast attack

            currMove = moves[Random.Range(0, moves.Count)];
        }

        // Basic Attacks
        if ((currMove == 1) | (currMove == 2))
        {
            setupMove = false;
        }
        // Evasive totalMoves set
        else if (currMove == 3)
        {
            movesLeft = totalMoves;
        }
        else if (currMove == 4)
        {
            movesLeft = 1;
        }
        // Support Attack
        else if (currMove == 5)
        {
            setupMove = true;
        }
        else if (currMove == 6)
        {
            isMoving = false;
        }
    }



    //////////////////////////////
    // Start of attack updatess //
    //////////////////////////////



    // Moves side to shot aiming at the player and shooting periodically
    void SweepAttackUpdate()
    {
        Move();
        Turn();


        // switch state
        if (timer <= 0)
        {
            // Idle -> Charge
            if (state == 0)
            {
                timer = chargePeriod;
                state = 1;
                eAnimator.SetInteger("state", state);

                // Stop movement
                //rb.velocity = new Vector3(0, 0, 0);
            }

            // Charging -> Shooting
            else if (state == 1)
            {
                timer = 0;
                state = 2;
                eAnimator.SetInteger("state", state);
                bulletCount = 0;
            }

            // Shooting
            else if (state == 2)
            {
                // Continue Shooting
                if (bulletCount < bulletsToShoot)
                {
                    Attack();
                    bulletCount += 1;
                    timer = bulletDelay;
                }

                // Shooting -> Aiming
                else
                {
                    timer = aimPeriod;
                    state = 0;
                    eAnimator.SetInteger("state", state);

                    // Add random amount of time to fire rate to desync enemies
                    float temp = Random.Range(0, 1f) / Time.fixedDeltaTime;
                    timer += temp;
                }
            }
        }

        timer--;
    }



    // Moves side to shot aiming at the player and shooting single shots periodically
    void SweepSingleAttackUpdate()
    {
        Move();
        Turn();


        // switch state
        if (timer <= 0)
        {
            // Idle -> Shoot -> Idle
            if (state == 0)
            {
                timer = aimPeriod * 2.5f;
                eAnimator.SetInteger("state", state);

                bulletSpeedMultiplier = .5f;
                Attack();
            }
        }

        timer--;
    }



    // Moves towards an edge, then moves side to side, aims straight and shoots in bursts
    void SupportAttackUpdate()
    {
        Move();

        // Face away from the wall
        if (setupDir == "U")
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else if (setupDir == "D")
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if (setupDir == "L")
        {
            transform.rotation = Quaternion.Euler(0, 0, 270);
        }
        else if (setupDir == "R")
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }



        // switch state
        if (timer <= 0)
        {
            // Idle -> Charge
            if (state == 0)
            {
                timer = chargePeriod;
                state = 1;
                eAnimator.SetInteger("state", state);

                // Stop movement
                rb.velocity = new Vector3(0, 0, 0);
            }

            // Charging -> Shooting
            else if (state == 1)
            {
                timer = 0;
                state = 2;
                eAnimator.SetInteger("state", state);
                bulletCount = 0;
            }

            // Shooting
            else if (state == 2)
            {
                // Continue Shooting
                if (bulletCount < bulletsToShoot)
                {
                    Attack();
                    bulletCount += 1;
                    timer = bulletDelay;
                }

                // Shooting -> Aiming
                else
                {
                    timer = aimPeriod;
                    state = 0;
                    eAnimator.SetInteger("state", state);
                }
            }
        }


        // Single shot
        // switch state
        //if (timer <= 0)
        //{
        //    // Idle
        //    if (state == 0)
        //    {
        //        timer = chargePeriod;
        //        state = 1;
        //        eAnimator.SetInteger("state", state);
        //    }

        //    // Charging
        //    else if (state == 1)
        //    {
        //        timer = aimPeriod;
        //        state = 0;
        //        eAnimator.SetInteger("state", state);
        //        Attack();
        //    }
        //}

        timer--;
    }



    // Moves randomly a few times then shoots
    void EvasiveAttackUpdate()
    {
        if ((state == 0) && (movementTimer >= (movementDelay * (2 / 3))) && isMoving)
        {
            MoveTo();
        }
        Turn();

        // decrease movements and move, unless its 0 left, then start doing the shooting part by setting isMoving to false
        if (movementTimer <= 0)
        {
            movesLeft -= 1;
            movementTimer = movementDelay;

            if (movesLeft <= 0)
            {
                isMoving = false;
            }
            else
            {
                ChoosePosition();
            }
        }

        // switch state
        if (timer <= 0)
        {
            // Moving -> Charge
            if (state == 0)
            {
                timer = chargePeriod * .5f;
                state = 1;
                eAnimator.SetInteger("state", state);

                // Stop movement
                rb.velocity = new Vector3(0, 0, 0);
            }

            // Charging -> Shooting
            else if (state == 1)
            {
                timer = 0;
                state = 2;
                eAnimator.SetInteger("state", state);
                bulletCount = 0;
            }

            // Shooting
            else if (state == 2)
            {
                // Continue Shooting
                if (bulletCount < bulletsToShoot)
                {
                    Attack();
                    bulletCount += 1;
                    timer = bulletDelay;
                }

                // Shooting -> Cooldown
                else
                {
                    timer = cooldownPeriod;
                    state = 3;
                    eAnimator.SetInteger("state", state);

                    //movementTimer = movementDelay;
                    ////movementTimer = movementDelay * 1.5f;
                    //movesLeft = totalMoves;
                    //isMoving = true;
                    //CheckDirections();

                    //eAnimator.SetInteger("state", state);
                }
            }

            // Cooldown -> Moving
            else if (state == 3)
            {
                timer = aimPeriod * .25f;
                state = 0;

                movementTimer = movementDelay;
                //movementTimer = movementDelay * 1.5f;
                movesLeft = totalMoves;
                isMoving = true;
                ChoosePosition();

                eAnimator.SetInteger("state", state);
            }
        }

        if (isMoving == false)
        {
            timer--;
            Turn();
        }
        else
        {
            movementTimer--;
            //TurnFast();
        }
    }



    // Moves randomly once then shoots a single shot
    void EvasiveSingleAttackUpdate()
    {
        if ((state == 0) && (movementTimer >= (movementDelay * (2 / 3))) && isMoving)
        {
            MoveTo();
        }
        Turn();

        // decrease movements and move, unless its 0 left, then start doing the shooting part by setting isMoving to false
        if (movementTimer <= 0)
        {
            movesLeft -= 1;
            movementTimer = movementDelay;

            if (movesLeft <= 0)
            {
                isMoving = false;
            }
            else
            {
                ChoosePosition();
            }
        }

        // switch state
        if (timer <= 0)
        {
            // Idle -> Shoot -> Cooldown
            if (state == 0)
            {
                timer = cooldownPeriod * .5f;
                state = 3;
                eAnimator.SetInteger("state", state);
                bulletSpeedMultiplier = .5f;
                Attack();

                // Stop movement
                rb.velocity = new Vector3(0, 0, 0);
            }

            // Cooldown -> Moving
            else if (state == 3)
            {
                timer = aimPeriod * .25f;
                state = 0;

                movementTimer = movementDelay;
                //movementTimer = movementDelay * 1.5f;
                movesLeft = 1;
                isMoving = true;
                ChoosePosition();

                eAnimator.SetInteger("state", state);
            }
        }

        if (isMoving == false)
        {
            timer--;
            Turn();
        }
        else
        {
            movementTimer--;
            //TurnFast();
        }
    }



    // Makes clouds that slide in, while moving randomly
    void OvercastAttackUpdate()
    {
        // 
        MoveTo();
        Turn();

        // decrease movements and move, unless its 0 left, then start doing the shooting part by setting isMoving to false
        if (movementTimer <= 0)
        {
            movesLeft -= 1;
            movementTimer = movementDelay * 2.5f;

            ChoosePosition();
        }


        // switch state
        if (timer <= 0)
        {
            if (isMoving == true)
            {
                MakeCloud();
                timer = overcastPeriod;
            }

            // Idle -> Charge
            else if (state == 0)
            {
                timer = chargePeriod;
                state = 1;
                eAnimator.SetInteger("state", state);

                // Stop movement
                rb.velocity = new Vector3(0, 0, 0);
            }

            // Charging -> Idle
            else if (state == 1)
            {
                timer = overcastPeriod;
                state = 0;
                eAnimator.SetInteger("state", state);
                eAnimator.SetTrigger("attack");
                isMoving = true;
            }
        }


        if (isMoving == true)
        {
            movementTimer--;
            //TurnFast();
        }

        timer--;
    }



    ////////////////////////////
    // End of attack updatess //
    ////////////////////////////



    void Move()
    {
        velocity = new Vector2(0.0f, 0.0f);

        // If not setting up to move to an edge, move according to current direction
        if (setupMove == false)
        {
            if (currDir == "U")
            {
                velocity = new Vector2(0.0f, speed);
            }

            else if (currDir == "D")
            {
                velocity = new Vector2(0.0f, -speed);
            }

            else if (currDir == "L")
            {
                velocity = new Vector2(-speed, 0.0f);
            }

            else if (currDir == "R")
            {
                velocity = new Vector2(speed, 0.0f);
            }

            rb.velocity = velocity * Time.fixedDeltaTime;
        }

        // If setting up, move in the setup direction
        else
        {
            if (setupDir == "U")
            {
                velocity = new Vector2(0.0f, speed);
            }

            else if (setupDir == "D")
            {
                velocity = new Vector2(0.0f, -speed);
            }

            else if (setupDir == "L")
            {
                velocity = new Vector2(-speed, 0.0f);
            }

            else if (setupDir == "R")
            {
                velocity = new Vector2(speed, 0.0f);
            }

            rb.velocity = velocity * Time.fixedDeltaTime;
        }
    }



    void MoveTo()
    {
        transform.position = Vector2.MoveTowards(transform.position, targetLocation, (speed * Time.fixedDeltaTime) / 45);
    }



    void CheckDirections()
    {
        List<Vector3> directions = new List<Vector3>();
        List<float> rotations = new List<float>();

        // Draw raycast to detect wall collision
        RaycastHit2D hit;

        // North
        //Debug.DrawRay(transform.position, Vector2.up * 2, Color.green);
        hit = Physics2D.Raycast(transform.position, Vector2.up, 2.4f, 1 << LayerMask.NameToLayer("Wall"));
        if (hit.collider == null)
        {
            directions.Add(new Vector3(transform.position.x, transform.position.y + 2, 0));
        }

        // South
        //Debug.DrawRay(transform.position, -Vector2.up * 2, Color.green);
        hit = Physics2D.Raycast(transform.position, -Vector2.up, 2.4f, 1 << LayerMask.NameToLayer("Wall"));
        if (hit.collider == null)
        {
            directions.Add(new Vector3(transform.position.x, transform.position.y - 2, 180));
        }

        // East
        //Debug.DrawRay(transform.position, Vector2.right * 2, Color.green);
        hit = Physics2D.Raycast(transform.position, Vector2.right, 2.4f, 1 << LayerMask.NameToLayer("Wall"));
        if (hit.collider == null)
        {
            directions.Add(new Vector3(transform.position.x + 2, transform.position.y, 270));
        }

        // West
        //Debug.DrawRay(transform.position, -Vector2.right * 2, Color.green);
        hit = Physics2D.Raycast(transform.position, -Vector2.right, 2.4f, 1 << LayerMask.NameToLayer("Wall"));
        if (hit.collider == null)
        {
            directions.Add(new Vector3(transform.position.x - 2, transform.position.y, 90));
        }

        //string temp = "";
        //foreach (string str in directions)
        //{
        //    temp += str; //maybe also + '\n' to put them on their own line.
        //}

        //float tempx = 0;
        //float tempy = 0;
        //float tempRot = 0;

        Vector3 tempVect = directions[Random.Range(0, directions.Count)];
        targetLocation = new Vector2(tempVect.x, tempVect.y);
        targetRotation = tempVect.z;

        //targetLocation = directions[Random.Range(0, directions.Count)];

        //Debug.Log(directions[Random.Range(0, directions.Count)]);
    }



    void ChoosePosition()
    {
        targetLocation = new Vector2(Random.Range(minX, maxX), Random.Range(minY, maxY));
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



    void TurnFast()
    {
        //Vector3 direction = player.transform.position - transform.position;
        //angle = (Mathf.Atan2(direction.x, direction.y)) * (180 / Mathf.PI);
        //angle = 0 - angle;
        angle2 = Quaternion.Euler(new Vector3(0, 0, targetRotation));

        //Debug.Log(targetRotation);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, angle2, turnSpeed * 3);
    }



    void Attack()
    {
        GameObject bullet = Instantiate(projectile, transform.position, transform.rotation) as GameObject;
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        bullet.GetComponent<Bullet>().speedMultiplier = bulletSpeedMultiplier;

        //bullets.Add(bullet);

        eAnimator.SetTrigger("attack");
    }



    // Has a random angle applied
    void RandomAttack()
    {
        Quaternion temp = Quaternion.Euler(0, 0, Random.Range(-10.0f, 10.0f));
        GameObject bullet = Instantiate(projectile, transform.position, transform.rotation * temp) as GameObject;
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        bullet.GetComponent<Bullet>().speedMultiplier = bulletSpeedMultiplier;

        //bullets.Add(bullet);

        eAnimator.SetTrigger("attack");
    }



    void SpreadAttack()
    {
        GameObject bullet1 = Instantiate(projectile, transform.position, transform.rotation) as GameObject;
        GameObject bullet2 = Instantiate(projectile, transform.position, transform.rotation * Quaternion.Euler(0, 0, 7)) as GameObject;
        GameObject bullet3 = Instantiate(projectile, transform.position, transform.rotation * Quaternion.Euler(0, 0, -7)) as GameObject;

        Physics2D.IgnoreCollision(bullet1.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(bullet2.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(bullet3.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        bullet1.GetComponent<Bullet>().speedMultiplier = bulletSpeedMultiplier;
        bullet2.GetComponent<Bullet>().speedMultiplier = bulletSpeedMultiplier;
        bullet3.GetComponent<Bullet>().speedMultiplier = bulletSpeedMultiplier;

        //bullets.Add(bullet);

        eAnimator.SetTrigger("attack");
    }



    void MakeCloud()
    {
        Quaternion temp = Quaternion.Euler(0, 0, cloudRotation);

        Vector2 tempPos = new Vector2(Random.Range(10, 13), Random.Range(minYCloud, maxYCloud));

        GameObject bullet = Instantiate(overcastProjectile, tempPos, temp) as GameObject;

        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        bullet.GetComponent<Bullet>().speedMultiplier = bulletSpeedMultiplier;
    }



    //void Contact(object[] args)
    //{
    //    string contactEdge = (string)args[0];

    //    // If it hits a wall, and not setting up, bounce off walls

    //    if (setupMove == false)
    //    {
    //        if (contactEdge == "N")
    //        {
    //            currDir = "D";
    //        }

    //        else if (contactEdge == "S")
    //        {
    //            currDir = "U";
    //        }

    //        else if (contactEdge == "E")
    //        {
    //            currDir = "L";
    //        }

    //        else if (contactEdge == "W")
    //        {
    //            currDir = "R";
    //        }
    //    }

    //    // If setting up, hitting a wall means its done setting up, so start moving side to side
    //    else
    //    {
    //        setupMove = false;
    //    }
    //}

    public override void Contact(string contactEdge, bool hitShield = false)
    {
        // If it hits a wall, and not setting up, bounce off walls

        if (setupMove == false)
        {
            if (contactEdge == "N")
            {
                currDir = "D";
            }

            else if (contactEdge == "S")
            {
                currDir = "U";
            }

            else if (contactEdge == "E")
            {
                currDir = "L";
            }

            else if (contactEdge == "W")
            {
                currDir = "R";
            }
        }

        // If setting up, hitting a wall means its done setting up, so start moving side to side
        else
        {
            setupMove = false;
        }
    }
}
