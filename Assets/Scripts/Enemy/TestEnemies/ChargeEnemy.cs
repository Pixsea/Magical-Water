using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargeEnemy : MonoBehaviour
{
    private GameObject bm;  //Battle manager
    private Phase battlePhase;

    private float spawnX;
    private float spawnY;
    private float spawnRotation;

    private GameObject player;

    [SerializeField]
    private GameObject projectile;

    private int health;

    [SerializeField]
    private float turnSpeed;
    [SerializeField]
    private float speed;
    [SerializeField]
    private float aimPeriod;
    [SerializeField]
    private float chargePeriod;
    [SerializeField]
    private float cooldownPeriod;

    private float cooldown;
    private float angle;
    private Quaternion angle2;

    // What state the enemy is in, 0 = aiming, 1 = charging, 2 = cooldown
    private int state;

    // Use to trigger attack animation
    [SerializeField]
    private Animator eAnimator;


    //public List<GameObject> bullets = new List<GameObject>();  // List used to destroy all bullets


    // Start is called before the first frame update
    void Start()
    {
        bm = GameObject.Find("Manager");

        spawnX = transform.position.x;
        spawnY = transform.position.y;
        spawnRotation = transform.rotation.eulerAngles.z;

        turnSpeed = turnSpeed * Time.fixedDeltaTime;
        speed = speed * Time.fixedDeltaTime;
        aimPeriod =aimPeriod / Time.fixedDeltaTime;
        chargePeriod = chargePeriod / Time.fixedDeltaTime;
        cooldownPeriod = cooldownPeriod / Time.fixedDeltaTime;

        player = GameObject.FindGameObjectWithTag("Player");
        cooldown = aimPeriod;
        state = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        battlePhase = bm.GetComponent<BattleManager>().currPhase;

        if (battlePhase == Phase.Combat)
        {
            if (state == 0)
            {
                Turn();
            }


            // switch state
            if (cooldown <= 0)
            {
                if (state == 0)
                {
                    cooldown = chargePeriod;
                    state = 1;
                    eAnimator.SetInteger("state", state);
                }

                else if (state == 1)
                {
                    cooldown = cooldownPeriod;
                    state = 2;
                    eAnimator.SetInteger("state", state);
                    Attack();
                }

                else if (state == 2)
                {
                    cooldown = aimPeriod;
                    state = 0;
                    eAnimator.SetInteger("state", state);
                }
            }

            cooldown--;
        }


        health = gameObject.GetComponent<EnemyStats>().health;

        if (health <= 0)
        {
            bm.GetComponent<BattleManager>().enemies.Remove(gameObject);
            Dead();
        }
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



    void Turn()
    {
        Vector3 direction = player.transform.position - transform.position;
        angle = (Mathf.Atan2(direction.x, direction.y)) * (180 / Mathf.PI);
        angle = 0 - angle;
        angle2 = Quaternion.Euler(new Vector3(0, 0, angle));

        //Debug.Log(angle);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, angle2, turnSpeed);
    }



    void Attack()
    {
        GameObject bullet = Instantiate(projectile, transform.position, transform.rotation) as GameObject;
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        //bullets.Add(bullet);

        eAnimator.SetTrigger("attack");
    }



    void Setup()
    {
        // Nothing
    }



    void Reset()
    {
        transform.position = new Vector3(spawnX, spawnY);
        transform.rotation = Quaternion.Euler(0, 0, spawnRotation);
        cooldown = aimPeriod;
        state = 0;
        eAnimator.SetInteger("state", state);
    }
}
