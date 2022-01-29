using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    // Enemy classifications
    public string enemyName; // Kind of enemy it is so that other scripts can call the correct enemy script
    [HideInInspector]
    public Dictionary<string, bool> enemyTypes = new Dictionary<string, bool>();  // Categories the enemy falls into

    // Enemy Types
    [SerializeField]
    private bool isShelled = false;  //Shelled enemies ram into you, so the shield must know to block the ram attack but not enemies that are just standing there

    [HideInInspector]
    public GameObject player;  // Reference to enemy player
    [HideInInspector]
    public Rigidbody2D rb;


    // Stats
    public int maxHealth;
    [HideInInspector]
    public int health;
    public int basePower;
    [HideInInspector]
    public int power;  // Damage dealt by touching the enemy
    public int shieldPower;  // Damage dealth to shields by touching the enemy
    public float baseSpeed;
    [HideInInspector]
    public float speed;
    public float baseRotationSpeed;
    [HideInInspector]
    public float rotationSpeed;

    [HideInInspector]
    public float powerMultiplier;
    [HideInInspector]
    public float speedMultiplier;


    // Phase/Turn Management
    [HideInInspector]
    public GameObject bm;  //Battle manager
    [HideInInspector]
    public Phase battlePhase;
    

    [HideInInspector]
    public int state; // What state the enemy is in
    [HideInInspector]
    public float timer;
    [HideInInspector]
    public int currMove;
    public int preferedMove;  //If not -1 (null), will always do the move corresponding to the given int

    [HideInInspector]
    public bool turning = false; //Uses in update loops to control when the enemy should turn to / face the player / moving
    [HideInInspector]
    public bool facingPlayer = false;
    [HideInInspector]
    public bool moving = false;

    // Spawn coordinates for turn based battles when enemies reset
    [HideInInspector]
    public float spawnX;
    [HideInInspector]
    public float spawnY;
    [HideInInspector]
    public float spawnRotation;


    // Used for animations
    public Animator eAnimator;


    // Color changers
    [SerializeField]
    private SpriteRenderer eSriteRenderer;

    [HideInInspector]
    public Color colorCode;
    [HideInInspector]
    public Color flickerCode;
    private float flickerTimer;
    [HideInInspector]
    public Color tintCode;
    private float tintTimer;


    //Start is called before the first frame update
    void Start()
    {
        bm = GameObject.Find("BattleManager");
        health = maxHealth;

        colorCode = new Color(1, 1, 1, 1);
        flickerCode = new Color(1, 1, 1, 1);
        tintCode = new Color(1, 1, 1, 1);

        spawnX = transform.position.x;
        spawnY = transform.position.y;
        spawnRotation = transform.rotation.eulerAngles.z;

        rb = GetComponent<Rigidbody2D>();

        player = GameObject.FindGameObjectWithTag("Player");

        power = basePower;
        speed = baseSpeed;

        powerMultiplier = 1f;
        speedMultiplier = 1f;

        EnemySetup();
    }


    // Called during start function for enemy
    public virtual void EnemySetup()
    {
        
    }


    // Call at the start of enemy fixedupdates to apply stat changes, should call any time stats are changed
    public void ModifyStats()
    {
        power = (int)Mathf.Ceil(basePower * powerMultiplier);
        speed = (int)Mathf.Ceil(baseSpeed * speedMultiplier);
    }

    public virtual void Setup()
    {

    }

    public virtual void StartAttackLoop()
    {

    }

    public virtual void Reset()
    {

    }

    public virtual void Damage(int damage)
    {
        health -= damage;

        Flash("red");
    }

    public virtual void Dead()
    {
        DestroyObject(gameObject);
    }


    public virtual int GetDamage()
    {
        return (int) (basePower*powerMultiplier);
    }


    public void MoveForward()
    {
        rb.velocity = transform.up * speed;
    }


    // Turns to player with rotationSpeed
    public void TurnTowardsPlayer()
    {
        Vector3 direction = player.transform.position - transform.position;
        float angle = (Mathf.Atan2(direction.x, direction.y)) * (180 / Mathf.PI);
        angle = 0 - angle;
        Quaternion angle2 = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.rotation = Quaternion.RotateTowards(transform.rotation, angle2, rotationSpeed);
    }



    // Always faces the player
    public void FacePlayer()
    {
        Vector3 direction = player.transform.position - transform.position;
        float angle = (Mathf.Atan2(direction.x, direction.y)) * (180 / Mathf.PI);
        angle = 0 - angle;
        Quaternion angle2 = Quaternion.Euler(new Vector3(0, 0, angle));
        transform.rotation = angle2;
    }






    /// <summary>
    /// Color changing functions
    /// </summary>


    // Call at the end of enemy fixedupdates to modify color
    public virtual void ColorUpdate()
    {
        FlashUpdate();

        if (flickerTimer > 0)
        {
            FlickerUpdate();
        }

        else if (tintTimer > 0)
        {
            TintUpdate();
        }

        eSriteRenderer.color = colorCode;
    }



    // Given a color, makes the enemy that color
    public void Flash(string color)
    {
        if (color == "red")
        {
            colorCode = Color.red;
        }

        else if (color == "blue")
        {
            colorCode = Color.blue;
        }
    }

    // Slowly makes the color back to white/basic
    void FlashUpdate()
    {
        if (colorCode.r < 1)
        {
            colorCode.r += .05f;
        }
        if (colorCode.b < 1)
        {
            colorCode.b += .05f;
        }
        if (colorCode.g < 1)
        {
            colorCode.g += .05f;
        }
    }

    // Given a color and a float, causes the enemy to flicker for that amount if time in seconds
    public void Flicker(string color, float time)
    {
        flickerTimer = time / Time.fixedDeltaTime;

        if (color == "red")
        {
            flickerCode = Color.red;
        }

        else if (color == "blue")
        {
            flickerCode = Color.blue;
        }
    }

    public void StopFlicker()
    {
        flickerTimer = 0;
    }

    // Alternate between the enemy being the flickerCode color and regular color
    void FlickerUpdate()
    {
        if ((flickerTimer % 8) >= 4)
        {
            colorCode = flickerCode;
        }
        else
        {
            colorCode = new Color(1, 1, 1, 1);
        }

        flickerTimer--;
    }



    // Given a color and a float, causes the enemy be that color for that amount if time in seconds
    public void Tint(string color, float time)
    {
        tintTimer = time / Time.fixedDeltaTime;

        if (color == "red")
        {
            tintCode = Color.red;
        }

        else if (color == "blue")
        {
            tintCode = Color.blue;
        }
    }

    public void StopTint()
    {
        tintTimer = 0;
    }

    // Make the color the tintColor
    void TintUpdate()
    {
        colorCode = tintCode;

        tintTimer--;
    }



    public virtual void Contact(string contactEdge, bool hitShield = false)
    {
    }



    //void Damage(int damage)
    //{
    //    health -= damage;
    //}
}
