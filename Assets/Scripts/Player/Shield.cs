using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shield : MonoBehaviour
{
    public GameObject player;

    public int health;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        //player.GetComponent<Guard>().shieldHealth = health;

        float angle = player.transform.eulerAngles.z;

        transform.position = player.transform.position;
        transform.rotation = Quaternion.Euler(0, 0, angle);
    }



    void OnCollisionEnter2D(Collision2D obj)
    {
        if (obj.gameObject.tag == "Wall")
        {
            Physics2D.IgnoreCollision(GetComponent<CapsuleCollider2D>(), obj.gameObject.GetComponent<Collider2D>());
        }
    }



    void OnTriggerEnter2D(Collider2D obj)
    {
        //if (obj.gameObject.tag == "Wall")
        //{
        //    Debug.Log("HITWALL");
        //    Physics2D.IgnoreCollision(GetComponent<Collider2D>(), obj.gameObject.GetComponent<Collider2D>());
        //}

        if (obj.gameObject.tag == "Bullet")
        {
            //int damage = obj.gameObject.GetComponent<BulletStats>().shieldPower;
            ////ApplyDamage(damage);
            //player.gameObject.GetComponent<Guard>().SendMessage("ApplyShieldDamage", damage);

            //obj.gameObject.SendMessage("HitShield", player);
            obj.gameObject.GetComponent<Bullet>().HitShield(player);
        }


        // For shell enemy bouncing, doesn't block stormite movement since checks to see if the enemy type is Shell
        else if (obj.gameObject.tag == "Edge")
        {
            Enemy enemy = obj.gameObject.GetComponent<Edges>().parent.GetComponent<Enemy>();

            if (enemy.enemyTypes["shelled"] == true)
            {
                // Only damage if actually moving
                if (enemy.state == 2)
                {
                    int damage = enemy.shieldPower;
                    //player.gameObject.GetComponent<Guard>().SendMessage("ApplyShieldDamage", damage);
                    player.gameObject.GetComponent<Guard>().ApplyShieldDamage(damage);
                }

                //object[] temp = new object[2];

                if (obj.gameObject.name == "NorthEdge")
                {
                    //obj.gameObject.SendMessage("Contact", "N");
                    //temp[0] = "N";
                    //temp[1] = true;
                    //enemy.SendMessage("Contact", temp);

                    enemy.Contact("N", true);
                }

                else if (obj.gameObject.name == "SouthEdge")
                {
                    //obj.gameObject.SendMessage("Contact", "S");
                    //temp[0] = "S";
                    //temp[1] = true;
                    //enemy.SendMessage("Contact", temp);

                    enemy.Contact("S", true);
                }

                else if (obj.gameObject.name == "EastEdge")
                {
                    //obj.gameObject.SendMessage("Contact", "E");
                    //temp[0] = "E";
                    //temp[1] = true;
                    //enemy.SendMessage("Contact", temp);

                    enemy.Contact("E", true);
                }

                else if (obj.gameObject.name == "WestEdge")
                {
                    //obj.gameObject.SendMessage("Contact", "W");
                    //temp[0] = "W";
                    //temp[1] = true;
                    //enemy.SendMessage("Contact", temp);

                    enemy.Contact("W", true);
                }
            }
        }


        //// For shell enemy bouncing, doesn't block stormite movement since checks to see if the enemy type is Shell
        //else if ((obj.gameObject.tag == "Edge") && (obj.gameObject.GetComponent<Edges>().parent.GetComponent<EnemyStats>().enemyTypes["shelled"] == true))
        //{
        //    // Only damage if actually moving
        //    if (obj.gameObject.GetComponent<Edges>().parent.GetComponent<Shoallet_Old>().state == 2)
        //    {
        //        int damage = obj.gameObject.GetComponent<Edges>().parent.GetComponent<EnemyStats>().shieldPower;
        //        player.gameObject.GetComponent<Guard>().SendMessage("ApplyShieldDamage", damage);
        //    }

        //    object[] temp = new object[2];

        //    if (obj.gameObject.name == "NorthEdge")
        //    {
        //        //obj.gameObject.SendMessage("Contact", "N");
        //        temp[0] = "N";
        //        temp[1] = true;
        //        obj.gameObject.GetComponent<Edges>().parent.SendMessage("Contact", temp);
        //    }

        //    else if (obj.gameObject.name == "SouthEdge")
        //    {
        //        //obj.gameObject.SendMessage("Contact", "S");
        //        temp[0] = "S";
        //        temp[1] = true;
        //        obj.gameObject.GetComponent<Edges>().parent.SendMessage("Contact", temp);
        //    }

        //    else if (obj.gameObject.name == "EastEdge")
        //    {
        //        //obj.gameObject.SendMessage("Contact", "E");
        //        temp[0] = "E";
        //        temp[1] = true;
        //        obj.gameObject.GetComponent<Edges>().parent.SendMessage("Contact", temp);
        //    }

        //    else if (obj.gameObject.name == "WestEdge")
        //    {
        //        //obj.gameObject.SendMessage("Contact", "W");
        //        temp[0] = "W";
        //        temp[1] = true;
        //        obj.gameObject.GetComponent<Edges>().parent.SendMessage("Contact", temp);
        //    }
        //}
    }
}
