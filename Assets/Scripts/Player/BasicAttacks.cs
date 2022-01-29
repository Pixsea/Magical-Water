using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BasicAttacks : MonoBehaviour
{
    // Animator for attacking
    [SerializeField]
    private GameObject pSprite;  // tell the animator script to attack

    [SerializeField]
    private GameObject projectile;
    [SerializeField]
    private GameObject rapid_projectile;
    [SerializeField]
    private GameObject spread_projectile;
    [SerializeField]
    private GameObject spray_projectile;
    [SerializeField]
    private GameObject precision_projectile;

    [HideInInspector]
    public bool canAttack = true;
    [HideInInspector]
    public bool canRotate = true;  // Maybe use to control rotating in the direction while moving if attack cooldowns are too long

    [SerializeField]
    private string Switchkey;
    [SerializeField]
    private string Guardkey;  // So you can't shoot while guarding

    private int mode;

    public List<GameObject> bullets = new List<GameObject>();  // List used to destroy all bullets


    // Start is called before the first frame update
    void Start()
    {
        mode = 0;
    }



    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButton(0) && canAttack)
        {

            StartCoroutine(ShootBullet());
        }
        {

        }

        
        if (Input.GetKeyDown(Switchkey))
        {
            mode += 1;

            if (mode > 4)
            {
                mode = 0;
            }
        }
    }


    private void FixedUpdate()
    {
        // Rotate towards mouse if trying to attack
        if (Input.GetMouseButton(0))
        {
            Vector3 object_pos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
            Vector2 shootDirection = new Vector2(0, 0);
            shootDirection.x = Input.mousePosition.x - object_pos.x;
            shootDirection.y = Input.mousePosition.y - object_pos.y;
            float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;

            Quaternion newDirection = Quaternion.Euler(new Vector3(0, 0, angle - 90));
            gameObject.transform.rotation = newDirection;
        }
    }


    IEnumerator ShootBullet()
    {
        canAttack = false;
        Vector3 object_pos = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        Vector2 shootDirection = new Vector2(0, 0);
        shootDirection.x = Input.mousePosition.x - object_pos.x;
        shootDirection.y = Input.mousePosition.y - object_pos.y;
        float angle = Mathf.Atan2(shootDirection.y, shootDirection.x) * Mathf.Rad2Deg;

        StartCoroutine(Attack(Quaternion.Euler(new Vector3(0, 0, angle - 90))));



        yield return null;
    }



    


    IEnumerator Attack(Quaternion direction)
    {

        // Given an quaternion roation, attacks in that direction
        if (mode == 0)
        {
            StartCoroutine(BasicShot(direction));
        }

        else if (mode == 1)
        {
            StartCoroutine(RapidShot(direction));
        }

        else if (mode == 2)
        {
            StartCoroutine(SpreadShot(direction));
        }

        else if (mode == 3)
        {
            StartCoroutine(SprayShot(direction));
        }

        else if (mode == 4)
        {
            StartCoroutine(PrecisionShot(direction));
        }

        yield return null;
    }


    ////////////////////
    // Start of Shots //
    ////////////////////



    IEnumerator BasicShot(Quaternion direction)
    {
        GameObject bullet1 = Instantiate(projectile, transform.position, direction) as GameObject;
        GameObject bullet2 = Instantiate(projectile, transform.position, direction * Quaternion.Euler(0, 0, 7)) as GameObject;
        GameObject bullet3 = Instantiate(projectile, transform.position, direction * Quaternion.Euler(0, 0, -7)) as GameObject;

        bullets.Add(bullet1);
        bullets.Add(bullet2);
        bullets.Add(bullet3);

        Physics2D.IgnoreCollision(bullet1.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(bullet2.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(bullet3.GetComponent<Collider2D>(), GetComponent<Collider2D>());


        yield return new WaitForSeconds(.6f);

        canAttack = true;

        yield return null;
    }



    IEnumerator RapidShot(Quaternion direction)
    {
        GameObject bullet = Instantiate(rapid_projectile, transform.position, direction) as GameObject;
        bullets.Add(bullet);
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        yield return new WaitForSeconds(.2f);

        canAttack = true;

        yield return null;
    }



    IEnumerator SpreadShot(Quaternion direction)
    {
        GameObject bullet1 = Instantiate(spread_projectile, transform.position, direction) as GameObject;
        GameObject bullet2 = Instantiate(spread_projectile, transform.position, direction * Quaternion.Euler(0, 0, 10)) as GameObject;
        GameObject bullet3 = Instantiate(spread_projectile, transform.position, direction * Quaternion.Euler(0, 0, -10)) as GameObject;
        GameObject bullet4 = Instantiate(spread_projectile, transform.position, direction * Quaternion.Euler(0, 0, 20)) as GameObject;
        GameObject bullet5 = Instantiate(spread_projectile, transform.position, direction * Quaternion.Euler(0, 0, -20)) as GameObject;
        GameObject bullet6 = Instantiate(spread_projectile, transform.position, direction * Quaternion.Euler(0, 0, 30)) as GameObject;
        GameObject bullet7 = Instantiate(spread_projectile, transform.position, direction * Quaternion.Euler(0, 0, -30)) as GameObject;

        bullets.Add(bullet1);
        bullets.Add(bullet2);
        bullets.Add(bullet3);
        bullets.Add(bullet4);
        bullets.Add(bullet5);
        bullets.Add(bullet6);
        bullets.Add(bullet7);

        Physics2D.IgnoreCollision(bullet1.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(bullet2.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(bullet3.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(bullet4.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(bullet5.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(bullet6.GetComponent<Collider2D>(), GetComponent<Collider2D>());
        Physics2D.IgnoreCollision(bullet7.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        yield return new WaitForSeconds(1.25f);

        canAttack = true;

        yield return null;
    }



    IEnumerator SprayShot(Quaternion direction)
    {
        Quaternion temp = Quaternion.Euler(0, 0, Random.Range(-30.0f, 30.0f));
        GameObject bullet = Instantiate(spray_projectile, transform.position, direction * temp) as GameObject;
        bullets.Add(bullet);
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        yield return new WaitForSeconds(.1f);

        canAttack = true;

        yield return null;
    }



    IEnumerator PrecisionShot(Quaternion direction)
    {
        GameObject bullet = Instantiate(precision_projectile, transform.position, direction) as GameObject;
        bullets.Add(bullet);
        Physics2D.IgnoreCollision(bullet.GetComponent<Collider2D>(), GetComponent<Collider2D>());

        yield return new WaitForSeconds(2.5f);

        canAttack = true;

        yield return null;
    }
}
