using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicMovement : MonoBehaviour
{
    private GameObject bm;  //Battle manager
    private Phase battlePhase;

    private float speed;  //Base speed of movement
    private Vector2 velocity;  //Vector controlling movement
    [HideInInspector]
    public static int playerDirection;  //Int representing direction facing, 1 is up, 2 is up-right, ect.

    //private bool isMoving;  //Makes it so you can't move in another direction while moving
    private string last; //Remembers last move


    private Rigidbody2D rb;

    // Movement keys
    [SerializeField]
    private string Upkey;
    [SerializeField]
    private string Leftkey;
    [SerializeField]
    private string Downkey;
    [SerializeField]
    private string Rightkey;

    // Return true when pushed down
    private bool Uppress;
    private bool Leftpress;
    private bool Downpress;
    private bool Rightpress;

    // Return True on frame pushed down
    private bool Updown;
    private bool Leftdown;
    private bool Downdown;
    private bool Rightdown;



    // Start is called before the first frame update
    void Start()
    {
        bm = GameObject.Find("BattleManager");
        speed = gameObject.GetComponent<PlayerStats>().speed;

        rb = GetComponent<Rigidbody2D>();
        playerDirection = 1;
    }

    // Update is called once per frame
    void Update()
    {
        Uppress = Input.GetKey(Upkey);
        Leftpress = Input.GetKey(Leftkey);
        Downpress = Input.GetKey(Downkey);
        Rightpress = Input.GetKey(Rightkey);

        Updown = Input.GetKeyDown(Upkey);
        Leftdown = Input.GetKeyDown(Leftkey);
        Downdown = Input.GetKeyDown(Downkey);
        Rightdown = Input.GetKeyDown(Rightkey);
    }

    void FixedUpdate()
    {
        battlePhase = bm.GetComponent<BattleManager>().currPhase;
        speed = gameObject.GetComponent<PlayerStats>().speed;

        // If not in combat or in the overworld, prevent movement
        if ((battlePhase != Phase.Combat) && (battlePhase != Phase.Overworld))
        {
            Uppress = false;
            Leftpress = false;
            Downpress = false;
            Rightpress = false;

            Updown = false;
            Leftdown = false;
            Downdown = false;
            Rightdown = false;
        }


        velocity = new Vector2(0.0f, 0.0f);

        Movement3();

        //velocity = new Vector2(0.0f, 0.0f);

        // GroundMove();
        //Movement2();
        //CheckDirection();


        // Limits max speed
        //if (myRigid.velocity.magnitude > speed)
        //{
        //    myRigid.velocity = myRigid.velocity.normalized * speed;
        //}
    }



    void MoveUp()
    {
        velocity.y += speed;
        //transform.rotation = Quaternion.Euler(new Vector2(0.0f, 1.0f));
        //direction = new Vector2(0.0f, 1.0f);
        //isMoving = true;
    }

    void MoveDown()
    {
        velocity.y += -speed;
        //transform.rotation = Quaternion.Euler(new Vector2(0.0f, -1.0f));
        //direction = new Vector2(0.0f, -1.0f);
        //isMoving = true;
    }

    void MoveLeft()
    {
        velocity.x += -speed;
        //transform.rotation = Quaternion.Euler(new Vector2(-1.0f, 0.0f));
        //direction = new Vector2(-1.0f, 0.0f);
        //isMoving = true;
    }

    void MoveRight()
    {
        velocity.x += speed;
        //transform.rotation = Quaternion.Euler(new Vector2(1.0f, 0.0f));
        //direction = new Vector2(1.0f, 0.0f);
        //isMoving = true;
    }



    //void GroundMove()
    //{
    //    //Basically makes it so if you press a direction while holding another, the new direction is registered
    //    if (isMoving)
    //    {
    //        // Check for new direction
    //        if (Updown)
    //        {
    //            last = "Up";
    //        }
    //        else if (Leftdown)
    //        {
    //            last = "Left";
    //        }
    //        else if (Downdown)
    //        {
    //            last = "Down";
    //        }
    //        else if (Rightdown)
    //        {
    //            last = "Right";
    //        }


    //        // Keep current direction
    //        if ((Uppress) & (last == "Up"))
    //        {
    //            MoveUp();
    //        }

    //        else if ((Leftpress) & (last == "Left"))
    //        {
    //            MoveLeft();
    //        }

    //        else if ((Downpress) & (last == "Down"))
    //        {
    //            MoveDown();
    //        }

    //        else if ((Rightpress) & (last == "Right"))
    //        {
    //            MoveRight();
    //        }

    //        else
    //        {
    //            isMoving = false;
    //            last = "";
    //        }

    //    }


    //    //Else check for a new direction
    //    else if (Uppress)
    //    {
    //        MoveUp();
    //        // rigidbody.AddTorque( x, y, z) continuously rotates
    //    }

    //    else if (Leftpress)
    //    {
    //        MoveLeft();
    //    }

    //    else if (Downpress)
    //    {
    //        MoveDown();
    //    }

    //    else if (Rightpress)
    //    {
    //        MoveRight();
    //    }

    //    else
    //    {
    //        isMoving = false;
    //        last = "";
    //    }
    //}



    void Movement()
    {
        // If holding two opposite directions, cancel both
        if (Uppress && !Downpress)
        {
            MoveUp();
        }

        if (Leftpress && !Rightpress)
        {
            MoveLeft();
        }

        if (Downpress && !Uppress)
        {
            MoveDown();
        }

        if (Rightpress && !Leftpress)
        {
            MoveRight();
        }


        // If going diagonal, limit speed
        if ((playerDirection == 2) | (playerDirection == 4) | (playerDirection == 6) | (playerDirection == 8))
        {
            velocity.x = velocity.x * 2 / 3;
            velocity.y = velocity.y * 2 / 3;
        }

        // Actually move the player
        transform.Translate(velocity * Time.deltaTime);
    }



    void Movement2()
    {
        //Check direction, then rotate to that direction and apply velocity that way

        if (Uppress && !Downpress && !Leftpress && !Rightpress)
        {
            playerDirection = 1;
            transform.rotation = Quaternion.Euler(0, 0, 0);
            //velocity.y += speed;
            velocity = new Vector2(0.0f, speed);
        }

        else if (Uppress && !Downpress && !Leftpress && Rightpress)
        {
            playerDirection = 2;
            transform.rotation = Quaternion.Euler(0, 0, 315);
            //velocity.y += speed;
            velocity = new Vector2(speed * .7071f, speed * .7071f);
        }

        else if (!Uppress && !Downpress && !Leftpress && Rightpress)
        {
            playerDirection = 3;
            transform.rotation = Quaternion.Euler(0, 0, 270);
            //velocity.y += speed;
            velocity = new Vector2(speed, 0.0f);
        }

        else if (!Uppress && Downpress && !Leftpress && Rightpress)
        {
            playerDirection = 4;
            transform.rotation = Quaternion.Euler(0, 0, 225);
            //velocity.y += speed;
            velocity = new Vector2(speed * .7071f, -speed * .7071f);
        }

        else if (!Uppress && Downpress && !Leftpress && !Rightpress)
        {
            playerDirection = 5;
            transform.rotation = Quaternion.Euler(0, 0, 180);
            //velocity.y += speed;
            velocity = new Vector2(0.0f,-speed);
        }

        else if (!Uppress && Downpress && Leftpress && !Rightpress)
        {
            playerDirection = 6;
            transform.rotation = Quaternion.Euler(0, 0, 135);
            //velocity.y += speed;
            velocity = new Vector2(-speed * .7071f, -speed * .7071f);
        }

        else if (!Uppress && !Downpress && Leftpress && !Rightpress)
        {
            playerDirection = 7;
            transform.rotation = Quaternion.Euler(0, 0, 90);
            //velocity.y += speed;
            velocity = new Vector2(-speed, 0.0f);
        }

        else if (Uppress && !Downpress && Leftpress && !Rightpress)
        {
            playerDirection = 8;
            transform.rotation = Quaternion.Euler(0, 0, 45);
            //velocity.y += speed;
            velocity = new Vector2(-speed * .7071f, speed * .7071f);
        }

        //transform.Translate(velocity * Time.deltaTime);
        //transform.Translate(velocity);
        rb.velocity = velocity * Time.fixedDeltaTime;
    }



    void Movement3()
    {
        //Check direction, then apply velocity in that direction

        if (Uppress && !Downpress && !Leftpress && !Rightpress)
        {
            playerDirection = 1;
            //transform.rotation = Quaternion.Euler(0, 0, 0);
            //velocity.y += speed;
            velocity = new Vector2(0.0f, speed);
        }

        else if (Uppress && !Downpress && !Leftpress && Rightpress)
        {
            playerDirection = 2;
            //transform.rotation = Quaternion.Euler(0, 0, 315);
            //velocity.y += speed;
            velocity = new Vector2(speed * .7071f, speed * .7071f);
        }

        else if (!Uppress && !Downpress && !Leftpress && Rightpress)
        {
            playerDirection = 3;
            //transform.rotation = Quaternion.Euler(0, 0, 270);
            //velocity.y += speed;
            velocity = new Vector2(speed, 0.0f);
        }

        else if (!Uppress && Downpress && !Leftpress && Rightpress)
        {
            playerDirection = 4;
            //transform.rotation = Quaternion.Euler(0, 0, 225);
            //velocity.y += speed;
            velocity = new Vector2(speed * .7071f, -speed * .7071f);
        }

        else if (!Uppress && Downpress && !Leftpress && !Rightpress)
        {
            playerDirection = 5;
            //transform.rotation = Quaternion.Euler(0, 0, 180);
            //velocity.y += speed;
            velocity = new Vector2(0.0f, -speed);
        }

        else if (!Uppress && Downpress && Leftpress && !Rightpress)
        {
            playerDirection = 6;
            //transform.rotation = Quaternion.Euler(0, 0, 135);
            //velocity.y += speed;
            velocity = new Vector2(-speed * .7071f, -speed * .7071f);
        }

        else if (!Uppress && !Downpress && Leftpress && !Rightpress)
        {
            playerDirection = 7;
            //transform.rotation = Quaternion.Euler(0, 0, 90);
            //velocity.y += speed;
            velocity = new Vector2(-speed, 0.0f);
        }

        else if (Uppress && !Downpress && Leftpress && !Rightpress)
        {
            playerDirection = 8;
            //transform.rotation = Quaternion.Euler(0, 0, 45);
            //velocity.y += speed;
            velocity = new Vector2(-speed * .7071f, speed * .7071f);
        }

        //transform.Translate(velocity * Time.deltaTime);
        //transform.Translate(velocity);
        rb.velocity = velocity * Time.fixedDeltaTime;
    }



    void CheckDirection()
    {
        if (Uppress && !Downpress && !Leftpress && !Rightpress)
        {
            playerDirection = 1;
        }

        else if (Uppress && !Downpress && !Leftpress && Rightpress)
        {
            playerDirection = 2;
        }

        else if (!Uppress && !Downpress && !Leftpress && Rightpress)
        {
            playerDirection = 3;
        }

        else if (!Uppress && Downpress && !Leftpress && Rightpress)
        {
            playerDirection = 4;
        }

        else if (!Uppress && Downpress && !Leftpress && !Rightpress)
        {
            playerDirection = 5;
        }

        else if (!Uppress && Downpress && Leftpress && !Rightpress)
        {
            playerDirection = 6;
        }

        else if (!Uppress && !Downpress && Leftpress && !Rightpress)
        {
            playerDirection = 7;
        }

        else if (Uppress && !Downpress && Leftpress && !Rightpress)
        {
            playerDirection = 8;
        }
    }
}
