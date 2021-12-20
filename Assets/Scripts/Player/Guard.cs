using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Guard : MonoBehaviour
{
    private GameObject bm;  //Battle manager
    private Phase battlePhase;

    [SerializeField]
    private GameObject Shield;

    private GameObject shield;  // The instance of the shield so that it can be destroyed

    [SerializeField]
    private string Guardkey;
    private bool Guarding;
    private bool Guarddown;
    private bool Guardup;
    private bool Guarddown2; // Buffer between update and fixedupdate
    //private bool Guardup2;

    [HideInInspector]
    public int shieldMaxHealth;
    [HideInInspector]
    public int shieldHealth;

    private float regenerationDelay;  // How many seconds it takes to regenerate one shield health
    private float regenerationTimer;

    private float decayDelay;  // How many seconds it takes to decay one shield health
    private float decayTimer;


    // Start is called before the first frame update
    void Start()
    {
        bm = GameObject.Find("BattleManager");

        shieldMaxHealth = gameObject.GetComponent<PlayerStats>().shieldMaxHealth;
        shieldHealth = shieldMaxHealth;

        regenerationDelay = gameObject.GetComponent<PlayerStats>().shieldRegenerationDelay / Time.fixedDeltaTime;
        regenerationTimer = regenerationDelay;

        decayDelay = gameObject.GetComponent<PlayerStats>().shieldDecayDelay / Time.fixedDeltaTime;
        decayTimer = decayDelay;
    }

    // Update is called once per frame
    void Update()
    {
        Guarding = Input.GetKey(Guardkey);
        Guarddown = Input.GetKeyDown(Guardkey);
        //Guardup = Input.GetKeyUp(Guardkey);

        if (Guarddown)
        {
            Guarddown2 = true;
        }

        //if (Guardup)
        //{
        //    Guardup2 = true;
        //}
    }

    void FixedUpdate()
    {
        battlePhase = bm.GetComponent<BattleManager>().currPhase;

        // If not in combat, prevent guarding
        if (battlePhase != Phase.Combat)
        {
            Guarding = false;
            Guarddown = false;
            //Guardup = false;
            Guarddown2 = false;
            //Guardup2 = false;
        }


        gameObject.GetComponent<PlayerStats>().guarding = Guarding;
        gameObject.GetComponent<PlayerStats>().shieldHealth = shieldHealth;


        if ((Guarddown2) && (shield == null))
        {
            Guarddown2 = false;

            Quaternion direction = Quaternion.Euler(0, 0, transform.eulerAngles.z);

            shield = Instantiate(Shield, transform.position, direction) as GameObject;
            Physics2D.IgnoreCollision(shield.GetComponent<CapsuleCollider2D>(), GetComponent<Collider2D>());

            //shield.transform.SetParent(transform);
            shield.GetComponent<Shield>().player = gameObject;
            shield.GetComponent<Shield>().health = shieldHealth;
        }

        if (Guarding == false)
        {
            // Destroy Shield if player stops guarding
            if (shield != null)
            {
                DestroyObject(shield);
            }

            // Shield regeneration
            if ((shieldHealth < shieldMaxHealth) && (regenerationTimer <= 0))
            {
                shieldHealth += 1;
                regenerationTimer = regenerationDelay;
            }

            regenerationTimer -= 1;
        }

        else
        {
            // Shield decay
            if (decayTimer <= 0)
            {
                shieldHealth -= 1;
                decayTimer = decayDelay;
            }

            decayTimer -= 1;
        }
    }



    public void ApplyShieldDamage(int damage)
    {
        shieldHealth -= damage;
    }
}
