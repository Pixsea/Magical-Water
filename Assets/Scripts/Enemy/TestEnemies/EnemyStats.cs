using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStats : MonoBehaviour
{
    private GameObject bm; // Battle Manager

    public string enemyName; // Kind of enemy it is so that other scripts can call the correct enemy script
    [HideInInspector]
    public Dictionary<string, bool> enemyTypes = new Dictionary<string, bool>();

    public int maxHealth;
    [HideInInspector]
    public int health;

    public int power;
    public int shieldPower;
    public float speed;

    // Enemy Types
    [SerializeField]
    private bool isShelled = false;  //Example is shelled for enemies that ram into you so that shield blocks the ram attack but not standing enemies


    // Color changers
    [SerializeField]
    private SpriteRenderer eSriteRenderer;

    private Color colorCode;
    private Color flickerCode;
    private float flickerTimer;
    private Color tintCode;
    private float tintTimer;


    // Start is called before the first frame update
    void Start()
    {
        enemyTypes.Add("shelled", isShelled);

        bm = GameObject.Find("Manager");
        health = maxHealth;

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



    //void Damage(int damage)
    //{
    //    health -= damage;
    //}
}
