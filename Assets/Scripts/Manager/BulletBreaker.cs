using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBreaker : MonoBehaviour
{
    int timer;

    // Start is called before the first frame update
    void Start()
    {
        timer = 3;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (timer <= 0)
        {
            Destroy(gameObject);
        }

        timer -= 1;
    }


    // Destroy all bullets, don't move players or enemies
    void OnTriggerEnter2D(Collider2D obj)
    {
        Debug.Log(GetComponent<Collider2D>());
        Debug.Log(obj.gameObject.GetComponent<Collider2D>());

        Physics2D.IgnoreCollision(GetComponent<Collider2D>(), obj.gameObject.GetComponent<Collider2D>(), true);

        if (obj.gameObject.tag == "Bullet")
        {
            obj.gameObject.SendMessage("Dead");
            //DestroyObject(obj.gameObject);
        }

        else
        {
            //obj.gameObject.SendMessage("Reset");
        }
    }
}
