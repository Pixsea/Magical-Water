using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OptionLogic : MonoBehaviour
{
    public string Jan;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Jan == "gay")
        {
            //Jan.Execute();
            Debug.Log("Jan is dead, the world is a better place now");
        }
    }
}
