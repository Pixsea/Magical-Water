using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stormite_Overworld : Enemy
{
    private GameObject player;

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
    private Rigidbody2D rb;

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
    void Start()
    {
        EnemySetup();

        bm = GameObject.Find("BattleManager");
        health = maxHealth;

        colorCode = new Color(1, 1, 1, 1);
        flickerCode = new Color(1, 1, 1, 1);
        tintCode = new Color(1, 1, 1, 1);

        spawnX = transform.position.x;
        spawnY = transform.position.y;
        spawnRotation = transform.rotation.eulerAngles.z;

        turnSpeed = turnSpeed * Time.fixedDeltaTime;
        aimPeriod = aimPeriod / Time.fixedDeltaTime;
        chargePeriod = chargePeriod / Time.fixedDeltaTime;
        cooldownPeriod = cooldownPeriod / Time.fixedDeltaTime;
        overcastPeriod = overcastPeriod / Time.fixedDeltaTime;

        bulletCount = 0;
        bulletDelay = bulletDelay / Time.fixedDeltaTime;

        rb = GetComponent<Rigidbody2D>();

        player = GameObject.FindGameObjectWithTag("Player");
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

    //void FixedUpdate()
    //{
    //    battlePhase = bm.GetComponent<BattleManager>().phase;
    //    ModifyStats();

    //    if ((battlePhase != "combat"))
    //    {
    //        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    //    }

    //    else if ((battlePhase == "combat") && (currMove == 1))
    //    {
    //        rb.constraints = RigidbodyConstraints2D.None;
    //        SweepAttackUpdate();
    //    }

    //    else if ((battlePhase == "combat") && (currMove == 2))
    //    {
    //        rb.constraints = RigidbodyConstraints2D.None;
    //        SweepSingleAttackUpdate();
    //    }

    //    else if ((battlePhase == "combat") && (currMove == 3))
    //    {
    //        rb.constraints = RigidbodyConstraints2D.None;
    //        EvasiveAttackUpdate();
    //    }

    //    else if ((battlePhase == "combat") && (currMove == 4))
    //    {
    //        rb.constraints = RigidbodyConstraints2D.None;
    //        EvasiveSingleAttackUpdate();
    //    }

    //    else if ((battlePhase == "combat") && (currMove == 5))
    //    {
    //        rb.constraints = RigidbodyConstraints2D.None;
    //        SupportAttackUpdate();
    //    }

    //    else if ((battlePhase == "combat") && (currMove == 6))
    //    {
    //        rb.constraints = RigidbodyConstraints2D.None;
    //        OvercastAttackUpdate();
    //    }

    //    if (health <= 0)
    //    {
    //        bm.GetComponent<BattleManager>().enemies.Remove(gameObject);
    //        Dead();
    //    }

    //    ColorUpdate();
    //}



    public override void Setup()
    {
        currDir = "U";

        //ChooseMove();


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



    public override void Damage(int damage)
    {
        health -= damage;

        Flash("red");
    }

    public override void Dead()
    {
        DestroyObject(gameObject);
    }
}
