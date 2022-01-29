using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public GameObject child;

    public Transform player;

    [SerializeField]
    private float speed;  //Base speed of movement
    private Vector2 velocity;  //Vector controlling movement


    // Start is called before the first frame update
    void Start()
    {
        child.transform.SetParent(player, false);
    }

    // Update is called once per frame
    void update()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);

        velocity = new Vector3(0.0f, 0.0f);


        if (transform.position.x > player.position.x)
        {
            //moveleft();
            transform.position += new Vector3(-speed, 0, 0);
        }

        if (transform.position.x < player.position.x)
        {
            //moveright();
            transform.position += new Vector3(speed, 0, 0);
        }

        if (transform.position.y > player.position.y)
        {
            //movedown();
            transform.position += new Vector3(0, -speed, 0);
        }

        if (transform.position.y < player.position.y)
        {
            //moveup();
            transform.position += new Vector3(0, speed, 0);
        }




        //if ((playerdirection == 2) | (playerdirection == 4) | (playerdirection == 6) | (playerdirection == 8))
        //{
        //    velocity.x = velocity.x * 2 / 3;
        //    velocity.y = velocity.y * 2 / 3;
        //}

        // actually move the player
        //transform.translate(velocity * time.deltatime);
    }


    void LateUpdate()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
    }



    void moveup()
    {
        velocity.y += speed;
    }

    void movedown()
    {
        velocity.y += -speed;
    }

    void moveleft()
    {
        velocity.x += -speed;
    }

    void moveright()
    {
        velocity.x += speed;
    }
}
