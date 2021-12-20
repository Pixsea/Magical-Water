using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int power;
    public int shieldPower; // power against shields
    public float speed;

    [HideInInspector]
    public float powerMultiplier;
    [HideInInspector]
    public float speedMultiplier;

    public float timer;
    public string type;  // Red breaks shields, blue deals no damage to shields
    public bool ignoresWalls;

    // Start is called before the first frame update
    //void Start()
    //{
    //    //power = Mathf.RoundToInt(power * powerMultiplier);
    //    //speed = speed * speedMultiplier;
    //}

    //void Awake()
    //{
    //    // Set to one if not modified
    //    if (powerMultiplier == 0f)
    //    {
    //        powerMultiplier = 1f;
    //    }

    //    if (speedMultiplier == 0f)
    //    {
    //        speedMultiplier = 1f;
    //    }
    //}



    public virtual void HitPlayer(GameObject player)
    {
    }

    public virtual void HitShield(GameObject shield)
    {
    }

    public virtual void HitWall()
    {
    }

    public virtual void Dead()
    {
    }
}
