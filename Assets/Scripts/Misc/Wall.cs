using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wall : MonoBehaviour
{
    void OnTriggerEnter2D(Collider2D obj)
    {
        if (obj.gameObject.tag == "Bullet")
        {
            if (obj.GetComponent<Bullet>().ignoresWalls == false)
            {
                obj.gameObject.GetComponent<Bullet>().HitWall();
            }
        }


        // For when enemies have to detect hitting a wall
        else if ((obj.gameObject.tag == "Edge"))
        {
            // To send the neccessary contact info
            //object[] temp = new object[2];

            Enemy enemy = obj.gameObject.GetComponent<Edges>().parent.GetComponent<Enemy>();

            if (obj.gameObject.name == "NorthEdge")
            {
                //obj.gameObject.SendMessage("Contact", "N");
                //temp[0] = "N";
                //temp[1] = false;
                //obj.gameObject.GetComponent<Edges>().parent.SendMessage("Contact", temp);

                enemy.Contact("N");
            }

            else if (obj.gameObject.name == "SouthEdge")
            {
                //obj.gameObject.SendMessage("Contact", "S");
                //temp[0] = "S";
                //temp[1] = false;
                //obj.gameObject.GetComponent<Edges>().parent.SendMessage("Contact", temp);

                enemy.Contact("S");
            }

            else if (obj.gameObject.name == "EastEdge")
            {
                //obj.gameObject.SendMessage("Contact", "E");
                //temp[0] = "E";
                //temp[1] = false;
                //obj.gameObject.GetComponent<Edges>().parent.SendMessage("Contact", temp);

                enemy.Contact("E");
            }

            else if (obj.gameObject.name == "WestEdge")
            {
                //obj.gameObject.SendMessage("Contact", "W");
                //temp[0] = "W";
                //temp[1] = false;
                //obj.gameObject.GetComponent<Edges>().parent.SendMessage("Contact", temp);

                enemy.Contact("W");
            }
        }
    }
}
