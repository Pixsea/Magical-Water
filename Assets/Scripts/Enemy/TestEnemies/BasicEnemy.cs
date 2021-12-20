using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicEnemy : MonoBehaviour
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
    private float fireDelay;

    private float cooldown;
    private float angle;
    private Quaternion angle2;

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
        fireDelay = fireDelay / Time.fixedDeltaTime;

        player = GameObject.FindGameObjectWithTag("Player");
        cooldown = fireDelay;
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
            Turn();

            if (cooldown <= 0)
            {
                Attack();
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
        angle = (Mathf.Atan2(direction.x, direction.y)) *(180 / Mathf.PI);
        angle = 0 - angle;
        angle2 = Quaternion.Euler(new Vector3(0, 0, angle));

        //Debug.Log(angle);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, angle2, turnSpeed);
    }



    void Attack()
    {
        GameObject bullet = Instantiate(projectile, transform.position, transform.rotation) as GameObject;
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        cooldown = fireDelay;

        //bullets.Add(bullet);

        eAnimator.SetTrigger("attack");
    }



    void Setup()
    {
        //Nothing
    }



    void Reset()
    {
        transform.position = new Vector3(spawnX, spawnY);
        transform.rotation = Quaternion.Euler(0, 0, spawnRotation);
        cooldown = fireDelay;
    }
}
