using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Seedling : Enemy
{
    private void FixedUpdate()
    {
        if (moving)
        {
            MoveForward();
        }

        if (health <= 0)
        {
            bm.GetComponent<BattleManager>().enemies.Remove(gameObject);
            Dead();
        }

        ColorUpdate();
    }


    public override void StartAttackLoop()
    {

        Debug.Log("test");
        StartCoroutine(Shamble());
    }


    IEnumerator Shamble()
    {
        if (true)
        {
            moving = true;
        }
        moving = true;
        yield return null;
    }
}
