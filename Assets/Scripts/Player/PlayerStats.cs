using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerStats : MonoBehaviour
{
    private GameObject bm;  //Battle manager
    private Phase battlePhase;

    [SerializeField]
    private GameObject pSprite; // For animations
    [SerializeField]
    private SpriteRenderer pSriteRenderer;

    private float spawnX;
    private float spawnY;
    private float spawnRotation;

    public int maxHealth;
    [HideInInspector]
    public int health;

    public float baseSpeed;
    [HideInInspector]
    public float speed;

    [HideInInspector]
    public bool guarding;
    public int shieldMaxHealth;
    [HideInInspector]
    public int shieldHealth;

    public float shieldRegenerationDelay;  // How many seconds it takes to regenerate one shield health
    public float shieldDecayDelay;  // How many seconds it takes to lose one shield health

    [SerializeField]
    private float damageInvincibilityLength;  // How long the player is invincible for when hurt in seconds
    private float damageInvincibilityTimer;
    private bool damageInvincible;

    // Color changers
    private Color colorCode;
    private Color flickerCode;
    private float flickerTimer;
    private Color tintCode;
    private float tintTimer;

    // Start is called before the first frame update
    void Start()
    {
        bm = GameObject.Find("BattleManager");

        spawnX = transform.position.x;
        spawnY = transform.position.y;
        spawnRotation = transform.rotation.eulerAngles.z;

        health = maxHealth;
        //uiHealthText = uiHealth.GetComponent<Text>();

        damageInvincibilityLength = damageInvincibilityLength / Time.fixedDeltaTime;
        damageInvincible = false;

        colorCode = new Color(1, 1, 1, 1);
        flickerCode = new Color(1, 1, 1, 1);
        tintCode = new Color(1, 1, 1, 1);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        battlePhase = bm.GetComponent<BattleManager>().currPhase;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        
        //if (battlePhase == "idle")
        //{
        //    rb.constraints = RigidbodyConstraints2D.FreezeAll;
        //}

        //else if (battlePhase != "idle")
        //{
        //    rb.constraints = RigidbodyConstraints2D.None;
        //    rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        //}

        if (guarding)
        {
            speed = baseSpeed / 2;
        }
        else
        {
            speed = baseSpeed;
        }


        // Color changers
        FlashUpdate();

        if (flickerTimer > 0)
        {
            FlickerUpdate();
        }

        else if (tintTimer > 0)
        {
            TintUpdate();
        }

        pSriteRenderer.color = colorCode;


        if (damageInvincible)
        {
            //pSriteRenderer.color = new Color(1, 1, 1, .5f);  // When invincible after taking damage, make the player 50% opaque/transparent

            // When invincible after taking damage, make the player flicker
            if ((damageInvincibilityTimer % 4) >= 2)
            {
                pSriteRenderer.color = new Color(1, 1, 1, 1);
            }
            else
            {
                pSriteRenderer.color = new Color(1, 1, 1, 0);
            }
        }
        else
        {
            pSriteRenderer.color = new Color(1, 1, 1, 1);
        }

        if (damageInvincibilityTimer <=0)
        {
            damageInvincible = false;
        }

        damageInvincibilityTimer--;
    }



    public void Reset()
    {
        transform.position = new Vector3(spawnX, spawnY);
        transform.rotation = Quaternion.Euler(0, 0, spawnRotation);
        gameObject.SendMessage("Reset2");
    }



    public void ApplyDamage(int damage)
    {
        health -= damage;
        DamageInvincibility();
    }



    public void ApplyEffect(string effect)
    {
        
    }



    // Player looks to see if it touches enemy/bullet triggers, then tells bullets they hit the player
    void OnTriggerStay2D(Collider2D obj)
    {
        if (damageInvincible == false)
        {
            if (obj.gameObject.tag == "Bullet")
            {
                //int damage = obj.gameObject.GetComponent<BulletStats>().power;
                //ApplyDamage(damage);

                obj.gameObject.SendMessage("HitPlayer", gameObject);
            }

            // IOf it collides with soemthing on the "Enemy Hitbox" layer
            else if (obj.gameObject.layer == 14)
            {
                int damage = obj.gameObject.transform.parent.gameObject.GetComponent<Enemy>().power;
                Debug.Log(damage);
                ApplyDamage(damage);
            }
        }
    }



    void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.gameObject.tag == "NPC")
        {
            obj.gameObject.GetComponent<DialogueTrigger>().StartDialogue();
        }
    }



    void DamageInvincibility()
    {
        damageInvincible = true;
        damageInvincibilityTimer = damageInvincibilityLength;
    }



    // Given a color, makes the enemy that color
    public void Flash(string color)
    {
        if (color == "red")
        {
            colorCode = Color.red;
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
}
