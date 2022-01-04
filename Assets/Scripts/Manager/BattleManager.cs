using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Phase { Idle, Combat, Dialogue, Overworld }

public class BattleManager : MonoBehaviour
{
    public bool overworldStart;
    public bool combatStart;

    [SerializeField]
    private Phase startPhase;

    [SerializeField]
    private string Switchkey; // For testing purposes
    private bool Switch;
    private bool Switch2;

    private GameObject player;

    [HideInInspector]
    public List<GameObject> enemies = new List<GameObject>();
    [HideInInspector]
    public Phase currPhase;  // Keeps track of what part of the turn it is, can be "idle", "combat", "overworld", or "dialogue"

    public float edgeN;  //Edge of arena for enemies that move
    public float edgeE;
    public float edgeS;
    public float edgeW;


    // Start is called before the first frame update
    void Start()
    {
        //foreach (GameObject enemy in enemies)
        //{
        //    Debug.Log(enemy.name);
        //}

        //enemies = GameObject.FindGameObjectsWithTag("Respawn").ToList();

        player = GameObject.FindGameObjectWithTag("Player");
        enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));

        SwitchPhase(startPhase);


    }

    // Update is called once per frame
    void Update()
    {
        Switch = Input.GetKeyDown(Switchkey);

        if (Switch)
        {
            Switch2 = true;
        }
    }

    void FixedUpdate()
    {
        //Debug.Log(phase);

        //if (currPhase == Phase.Idle)
        //{
        //    if (Switch2)
        //    {
        //        currPhase = Phase.Combat;
        //        Switch2 = false;
        //        //SetupAll();
        //    }
        //}

        //else if (currPhase == Phase.Combat)
        //{
        //    if (Switch2)
        //    {
        //        currPhase = Phase.Idle;
        //        Switch2 = false;
        //        //ResetAll();
        //    }
        //}
    }


    void SwitchPhase(Phase newPhase)
    {
        currPhase = newPhase;

        if (currPhase == Phase.Combat)
        {
            List<GameObject> allEnemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
            foreach (GameObject enemy in enemies)
            {
                enemy.gameObject.GetComponent<Enemy>().StartAttackLoop();
            }
        }
    }



    void SetupAll()
    {
        foreach (GameObject enemy in enemies)
        {
            enemy.gameObject.GetComponent<Enemy>().Setup();
        }
    }



    // Calls the Reset function for every enemy and player to reset position
    void ResetAll()
    {
        player.gameObject.GetComponent<PlayerStats>().Reset();

        foreach (GameObject enemy in enemies)
        {
            //enemy.gameObject.SendMessage("Reset");
            enemy.gameObject.GetComponent<Enemy>().Reset();
        }

        List<GameObject>  bullets = new List<GameObject>(GameObject.FindGameObjectsWithTag("Bullet"));
        foreach (GameObject bullet in bullets)
        {
            // Uses send message since must go to enemy and player bullets, maybe optimize with get component later
            //bullet.gameObject.SendMessage("Dead");
            bullet.gameObject.GetComponent<Bullet>().Dead();
        }
    }
}
