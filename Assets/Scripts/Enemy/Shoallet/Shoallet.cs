using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shoallet : Enemy
{
    [SerializeField]
    private float turnSpeed;
    [SerializeField]
    private float aimPeriod;
    [SerializeField]
    private float chargePeriod;
    [SerializeField]
    private float staggerPeriod;

    private Vector2 velocity;  //Vector controlling movement

    private float angle;
    private Quaternion angle2;

    // What state the enemy is in, 0 = aiming, 1 = spinning in place, 2 is spinning and moving, 3 is staggered, 4 is getting up
    
    private int rebounds;  // Increases everytime it bounces off a wall
    private float staggerSpeed; //Controls how fast the shoallet goes after staggering, slowly decreases

    [SerializeField]
    private float reboundBuffer; // Time in seconds that must pass before rebound can be incremeted, so that one wall can't increment multiple times
    private float reboundTimer;

    // For blockable shots
    [HideInInspector]
    public bool blockable;
    public bool blinking; // So that the blinking function is only called once

    // For homing attacks
    [SerializeField]
    private float acceleration;
    [SerializeField]
    private float accelerationMultiplier;
    [SerializeField]
    private float maxSpeed;
    [SerializeField]
    private float attackDelay;
    private int noSpeedCount;  // Amount of checks until the homing attack can speed shot again


    // Start is called before the first frame update
    public override void EnemySetup()
    {
        turnSpeed = turnSpeed * Time.fixedDeltaTime;
        aimPeriod = aimPeriod / Time.fixedDeltaTime;
        chargePeriod = chargePeriod / Time.fixedDeltaTime;
        staggerPeriod = staggerPeriod / Time.fixedDeltaTime;

        timer = aimPeriod;
        state = 0;
        rebounds = 0;

        reboundBuffer = reboundBuffer / Time.fixedDeltaTime;
        reboundTimer = 0;

        blockable = false;
        blinking = false;

        accelerationMultiplier = 1f;
        maxSpeed = maxSpeed * Time.fixedDeltaTime;
        attackDelay = attackDelay / Time.fixedDeltaTime;
    }

    void FixedUpdate()
    {
        battlePhase = bm.GetComponent<BattleManager>().currPhase;
        ModifyStats();


        if (state == 0)
        {
            StopTint();
            blockable = false;
        }

        if (blockable)
        {
            shieldPower = 0;
        }
        else
        {
            shieldPower = power;
        }

        if (turnToPlayer)
        {
            Turn();
        }
        if (facePlayer)
        {
            Face();
        }

        // Do a telegraph flash if going to do a blockable attack
        if ((state == 1) && blockable && (blinking == false))
        {
            blinking = true;
            Flicker("blue", 5);
            //Flash("blue");
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
        //ChooseMove();
        StartAttackLoop();
    }

    public override void StartAttackLoop()
    {
        ChooseMove();
        StartCoroutine(RichochetAttackLoop());
    }



    public override void Reset()
    {
        transform.position = new Vector3(spawnX, spawnY);
        transform.rotation = Quaternion.Euler(0, 0, spawnRotation);

        timer = aimPeriod;
        state = 0;
        eAnimator.SetInteger("state", state);
        rebounds = 0;
        reboundTimer = 0;
        accelerationMultiplier = 1f;
        blockable = false;
        eAnimator.SetBool("strong spin", false);
        StopFlicker();
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
            moves.Add(1); // Richochet attack
            moves.Add(2); // Homing Attack

            currMove = moves[Random.Range(0, moves.Count)];
        }
    }


    public IEnumerator RichochetAttackLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(aimPeriod);

            // Standing -> Spinning
            turnToPlayer = true;
            state = 1;
            eAnimator.SetInteger("state", state);
            eAnimator.SetBool("strong spin", true);

            // Randomly do a speed shot that has less charge time, is faster, and is blockable
            if (Random.Range(0f, 1f) <= .4f)
            {
                blockable = true;
                blinking = false;
                yield return new WaitForSeconds(chargePeriod * .6f);
            }

            else
            {
                yield return new WaitForSeconds(chargePeriod);
            }


            // Spinning -> Moving
            state = 2;
            eAnimator.SetInteger("state", state);
            eAnimator.SetBool("strong spin", false);
            StopFlicker();
        }

        yield return null;
    }



    void RichochetAttackUpdate()
    {


        if (state == 0)
        {
            Turn();
        }

        else if (state == 1)
        {
            Face();
        }

        velocity = new Vector2(0.0f, 0.0f);


        // Do a telegraph flash if going to do a blockable attack
        if ((state == 1) && blockable && (blinking == false) && (timer <= chargePeriod * .75 * .6f))
        {
            blinking = true;
            Flicker("blue", 5);
            //Flash("blue");
        }


        // switch state
        if (timer <= 0)
        {
            // Standing -> Spinning
            if (state == 0)
            {
                timer = chargePeriod;
                state = 1;
                eAnimator.SetInteger("state", state);
                eAnimator.SetBool("strong spin", true);

                // Randomly do a speed shot that has less charge time, is faster, and is blockable
                if (Random.Range(0f, 1f) <= .4f)
                {
                    blockable = true;
                    blinking = false;
                    timer = timer * .6f;
                }
            }

            // Spinning -> Moving
            else if (state == 1)
            {
                state = 2;
                eAnimator.SetInteger("state", state);
                eAnimator.SetBool("strong spin", false);
                StopFlicker();
            }

            // Staggering -> Getup
            else if (state == 3)
            {
                state = 4;
                timer = aimPeriod;
                eAnimator.SetInteger("state", state);
            }

            // Getup -> Standing
            else if (state == 4)
            {
                state = 0;
                timer = aimPeriod;
                eAnimator.SetInteger("state", state);
            }
        }

        if ((state == 2) && (blockable == false))
        {
            // Start moving/keep moving
            rb.velocity = transform.up * speed * Time.fixedDeltaTime;
        }
        else if ((state == 2) && (blockable == true))
        {
            // Start moving/keep moving
            rb.velocity = transform.up * speed * 1.5f * Time.fixedDeltaTime;
            Tint("blue", 99);
        }

        else if (state == 3)
        {
            // Slow down after staggering
            rb.velocity = transform.up * staggerSpeed * Time.fixedDeltaTime;
            staggerSpeed -= 10f;

            if (staggerSpeed <= 0)
            {
                staggerSpeed = 0;
            }
        }

        timer--;
        reboundTimer--;
    }



    // Works on velocity
    //void HomingAttackUpdate()
    //{
    //    if (state == 0)
    //    {
    //        Turn();
    //    }

    //    else if ((state == 2) && (reboundTimer < (-1 / Time.fixedDeltaTime)))
    //    //else if (state == 2)
    //    {
    //        //Face();
    //        TurnFast();
    //    }

    //    // switch state
    //    if (timer <= 0)
    //    {
    //        // Standing -> Spinning
    //        if (state == 0)
    //        {
    //            timer = chargePeriod;
    //            state = 1;
    //            eAnimator.SetInteger("state", state);
    //        }

    //        // Spinning -> Moving
    //        else if (state == 1)
    //        {
    //            state = 2;
    //            eAnimator.SetInteger("state", state);
    //        }

    //        // Staggering -> Getup
    //        else if (state == 3)
    //        {
    //            state = 4;
    //            timer = aimPeriod;
    //            eAnimator.SetInteger("state", state);
    //        }

    //        // Getup -> Standing
    //        else if (state == 4)
    //        {
    //            state = 0;
    //            timer = aimPeriod;
    //            eAnimator.SetInteger("state", state);
    //        }
    //    }


    //    if ((state == 2) && (blockable == false) && (reboundTimer < (-1 / Time.fixedDeltaTime)))
    //    {
    //        // Start moving/keep moving
    //        rb.velocity = transform.up * (speed * .3f) * Time.fixedDeltaTime;
    //        //rb.AddForce(transform.up * speed * .25f * Time.fixedDeltaTime);
    //    }
    //    else if ((state == 2) && (blockable == true))
    //    {
    //        // Start moving/keep moving faster if doing a blockable seed shot
    //        rb.velocity = transform.up * speed * 1.5f * Time.fixedDeltaTime;
    //        Tint("blue", 99);
    //    }

    //    else if ((state == 2) && (reboundTimer > (-1 / Time.fixedDeltaTime)))
    //    {
    //        // keep applying backward momentum after hitting a wall
    //        rb.velocity = transform.up * staggerSpeed;
    //        staggerSpeed -= .05f;

    //        if (staggerSpeed <= 0)
    //        {
    //            staggerSpeed = 0;
    //        }
    //    }

    //    timer--;
    //    reboundTimer--;
    //}



    // Works on acceleration
    void HomingAttack2Update()
    {
        //float angleDif = -500f;

        //// Only check angle difference if moving
        //if (rb.velocity.magnitude != 0)
        //{
        //    // Angle difference between rotation and velocity direction, 180 means they are equal
        //    angleDif = Quaternion.Angle(transform.rotation, Quaternion.LookRotation(rb.velocity.normalized, Vector3.forward));
        //}

        //if ((state == 2) && ((angleDif < 120) | (angleDif > 250)) && (angleDif != -500f))
        //{
        //    Debug.Log(angleDif);
        //    accelerationMultiplier = 4f;
        //    //eAnimator.SetBool("strong spin", true);
        //}
        //else
        //{
        //    Debug.Log(angleDif);
        //    accelerationMultiplier = 1f;
        //    //eAnimator.SetBool("strong spin", false);
        //}


        if (rb.velocity.magnitude > maxSpeed)
        {
            rb.velocity = rb.velocity.normalized * maxSpeed;
        }


        if (state == 0)
        {
            Turn();
        }

        else if (state == 1)
        {
            Face();
            //TurnFast();
        }

        //else if ((state == 2) && (reboundTimer < (-1 / Time.fixedDeltaTime)))
        else if ((state == 2) && (blockable == false))
        {
            Face();
            //TurnFast();
        }


        // Do a telegraph flash if going to do a blockable attack
        if ((state == 1) && blockable && (blinking == false))
        {
            blinking = true;
            Flicker("blue", 5);
            //Flash("blue");
        }


        // switch state
        if (timer <= 0)
        {
            // Standing -> Spinning
            if (state == 0)
            {
                timer = chargePeriod * .4f;
                state = 1;
                eAnimator.SetInteger("state", state);
            }

            // Spinning -> Moving
            else if (state == 1)
            {
                timer = attackDelay;
                timer += (Random.Range(0f, 2.5f) / Time.fixedDeltaTime);
                state = 2;
                eAnimator.SetInteger("state", state);
            }

            // Moving -> maybe start spinning for speed dash
            else if (state == 2)
            {
                // Randomly rushes the player while homing with a blockable richochetting shop, so that it can hit layers exploiting the movement
                if ((Random.Range(0f, 1f) <= .6f) && (noSpeedCount <= 0))
                {
                    blockable = true;
                    blinking = false;
                    state = 1;
                    rb.velocity = Vector3.zero;
                    timer = chargePeriod * .6f;

                    noSpeedCount = 2;  // Must do 2 checks before can speed shot again

                    eAnimator.SetBool("strong spin", true);
                }
                // Reset the delay and keep homing
                else
                {
                    timer = attackDelay;
                    timer += (Random.Range(-1f, 1f) / Time.fixedDeltaTime);
                    noSpeedCount -= 1;
                }
            }

            // Staggering -> Getup
            else if (state == 3)
            {
                state = 4;
                timer = aimPeriod * .25f;
                eAnimator.SetInteger("state", state);
            }

            // Getup -> Standing
            else if (state == 4)
            {
                state = 0;
                timer = aimPeriod;
                eAnimator.SetInteger("state", state);
            }
        }


        if ((state == 2) && (blockable == false)) //&& (reboundTimer < (-1 / Time.fixedDeltaTime)))
        {
            // Start moving/keep moving

            //rb.velocity = transform.up * (speed * .3f) * Time.fixedDeltaTime;
            rb.AddForce(transform.up * acceleration * accelerationMultiplier * Time.fixedDeltaTime);
        }
        else if ((state == 2) && (blockable == true))
        {
            // Start moving/keep moving faster if doing a blockable speed shot
            rb.velocity = transform.up * speed * 1.5f * Time.fixedDeltaTime;

            Tint("blue", 99);
            StopFlicker();
            eAnimator.SetBool("strong spin", false);
        }

        else if (state == 3)
        {
            // Slow down after hitting a wall
            rb.velocity = transform.up * staggerSpeed * Time.fixedDeltaTime;
            staggerSpeed -= 10f;

            if (staggerSpeed <= 0)
            {
                staggerSpeed = 0;
            }
        }

        timer--;
        reboundTimer--;
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
        Vector3 direction = player.transform.position - transform.position;
        angle = (Mathf.Atan2(direction.x, direction.y)) * (180 / Mathf.PI);
        angle = 0 - angle;
        angle2 = Quaternion.Euler(new Vector3(0, 0, angle));

        //Debug.Log(angle);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, angle2, turnSpeed * 1.5f);
    }



    // Always faces the player
    void Face()
    {
        Vector3 direction = player.transform.position - transform.position;
        angle = (Mathf.Atan2(direction.x, direction.y)) * (180 / Mathf.PI);
        angle = 0 - angle;
        angle2 = Quaternion.Euler(new Vector3(0, 0, angle));

        //Debug.Log(angle);

        transform.rotation = angle2;
    }


    public override void Contact(string contactEdge, bool hitShield = false)
    {
        // If attacking or staggered and still moving, flip the rotation so that the richochet attack can keep moving forward, or that the dashing part of the homing attack can bounce

        //if ((state == 2) | ((state == 3) && (staggerSpeed > 0)))
        if (((state == 2) |
            ((state == 3) && (staggerSpeed > 0))) &&
            ((currMove == 1) | ((currMove == 2) && blockable)))
        {
            if ((state == 2) && (reboundTimer <= 0))
            {
                rebounds += 1;
                reboundTimer = reboundBuffer;
            }

            // If it hits a wall, bounce off walls
            if (contactEdge == "N")
            {
                float tempAngle = transform.rotation.eulerAngles.z;

                tempAngle += (180 - (2 * tempAngle));

                transform.rotation = Quaternion.Euler(0, 0, tempAngle);
            }

            else if (contactEdge == "S")
            {
                float tempAngle = transform.rotation.eulerAngles.z;

                tempAngle += (180 - (2 * tempAngle));

                transform.rotation = Quaternion.Euler(0, 0, tempAngle);
            }

            else if (contactEdge == "E")
            {
                float tempAngle = transform.rotation.eulerAngles.z;
                transform.rotation = Quaternion.Euler(0, 0, -tempAngle);
            }

            else if (contactEdge == "W")
            {
                float tempAngle = transform.rotation.eulerAngles.z;
                transform.rotation = Quaternion.Euler(0, 0, -tempAngle);
            }

            // Only stagger if doing the richochet move
            if (rebounds >= 7)
            {
                Stagger();
            }
        }

        // Only for when homing and not blockable
        else if ((state == 2) && (currMove == 2))
        {
            float speedLoss = .85f;

            // If it hits a wall, bounce off walls
            if (contactEdge == "N")
            {
                rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y * speedLoss);
            }

            else if (contactEdge == "S")
            {
                rb.velocity = new Vector2(rb.velocity.x, -rb.velocity.y * speedLoss);
            }

            else if (contactEdge == "E")
            {
                rb.velocity = new Vector2(-rb.velocity.x * speedLoss, rb.velocity.y);
            }

            else if (contactEdge == "W")
            {
                rb.velocity = new Vector2(-rb.velocity.x * speedLoss, rb.velocity.y);
            }
        }



        if (blockable && hitShield && (state == 2))
        {
            Stagger();
        }

        //else if (currMove == 2)
        //{
        //    //staggerSpeed = rb.velocity.magnitude;
        //    //rb.velocity = Vector3.zero;
        //    rb.velocity = -rb.velocity;
        //}
    }



    void Stagger()
    {
        rebounds = 0;
        state = 3;
        eAnimator.SetInteger("state", state);
        staggerSpeed = speed;
        timer = staggerPeriod;

        StopTint();
    }
}
