using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestNPC : MonoBehaviour
{
    private GameObject player;

    [SerializeField]
    private float turnSpeed;

    private float angle;
    private Quaternion angle2;


    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        turnSpeed = turnSpeed * Time.fixedDeltaTime;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        TurnFast();
    }



    void TurnFast()
    {
        Vector3 direction = player.transform.position - transform.position;
        angle = (Mathf.Atan2(direction.x, direction.y)) * (180 / Mathf.PI);
        angle = 0 - angle;
        angle2 = Quaternion.Euler(new Vector3(0, 0, angle));

        //Debug.Log(angle);

        transform.rotation = Quaternion.RotateTowards(transform.rotation, angle2, turnSpeed * 1.5f);
    }
}
