using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueTrigger : MonoBehaviour
{
    public Dialogue dialogue;

    private GameObject dm;  // Dialogue manager


    void Start()
    {
        dm = GameObject.Find("DialogueManager");
    }

    public void StartDialogue()
    {
        dm.GetComponent<DialogueManager>().StartDialogue(dialogue);
    }
}
