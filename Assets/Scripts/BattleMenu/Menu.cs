using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviour
{
    public GameObject cursor;
    private int currentSelection = 0;
    public GameObject[] options;

    //public Text Option1 = null;
    //public Text Option2 = null;
    //public Text Option3 = null;
    //public Text Option4 = null;
    //public Text Option5 = null;
    //public Text Option6 = null;


    // Movement keys for cursor
    [SerializeField]
    private string Upkey;
    [SerializeField]
    private string Leftkey;
    [SerializeField]
    private string Downkey;
    [SerializeField]
    private string Rightkey;
    [SerializeField]
    private string Selectkey;
    [SerializeField]
    private string Backkey;

    // Return True on frame pushed down
    private bool Updown;
    private bool Leftdown;
    private bool Downdown;
    private bool Rightdown;
    private bool Selectdown;
    private bool Backdown;


    // Used so that it rememebers when a key is pressed down if an update frame happens without a fixed update
    private bool Updown2;
    private bool Leftdown2;
    private bool Downdown2;
    private bool Rightdown2;
    private bool Selectdown2;
    private bool Backdown2;


    // Start is called before the first frame update
    void Start()
    {
        cursor.transform.position = options[0].transform.position + new Vector3(75, -20, 0);
    }

    // Update is called once per frame
    void Update()
    {
        Updown = Input.GetKeyDown(Upkey);
        Leftdown = Input.GetKeyDown(Leftkey);
        Downdown = Input.GetKeyDown(Downkey);
        Rightdown = Input.GetKeyDown(Rightkey);
        Selectdown = Input.GetKeyDown(Selectkey);
        Backdown = Input.GetKeyDown(Backkey);

        // Remeber if true for fixed update, will get set back to false at first fixed update
        if (Updown)
        {
            Updown2 = true;
        }
        if (Leftdown)
        {
            Leftdown2 = true;
        }
        if (Downdown)
        {
            Downdown2 = true;
        }
        if (Rightdown)
        {
            Rightdown2 = true;
        }
        if (Selectdown)
        {
            Selectdown2 = true;
        }
        if (Backdown)
        {
            Backdown2 = true;
        }
    }


    void FixedUpdate()
    {
        if (Rightdown2 && (currentSelection < options.Length - 1))
        {
            Rightdown2 = false;

            currentSelection += 1;
            cursor.transform.position = options[currentSelection].transform.position + new Vector3(75, -20, 0);
        }

        else if (Leftdown2 && (currentSelection > 0))
        {
            Leftdown2 = false;

            currentSelection -= 1;
            cursor.transform.position = options[currentSelection].transform.position + new Vector3(75, -20, 0);
        }
    }
}
