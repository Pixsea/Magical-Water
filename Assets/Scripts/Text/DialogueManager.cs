using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogueManager : MonoBehaviour
{
    // For UI
    public Image Box;
    public Text Name;
    public Text Dialogue;

    public Queue<string> text;

    private string nextScene;

    private GameObject bm;
    private Phase oldPhase;

    [SerializeField]
    private string Switchkey; // For advancing test
    private bool Switch;
    private bool Switch2;

    // Start is called before the first frame update
    void Start()
    {
        text = new Queue<string>();

        nextScene = "";

        bm = GameObject.Find("BattleManager");
        //oldPhase = "";

        Box.color = new Color(1, 1, 1, 0);
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
        if (Switch2)
        {
            ProgressText();
            Switch2 = false;
        }
    }



    public void StartDialogue(Dialogue dialogue)
    {
        Name.text = dialogue.name;

        text.Clear();

        // Make Text box appear
        Box.color = new Color(1, 1, 1, 1);

        foreach (string sentence in dialogue.sentences)
        {
            text.Enqueue(sentence);
        }

        // Remember old phase and set to dialogue
        oldPhase = bm.GetComponent<BattleManager>().currPhase;
        bm.GetComponent<BattleManager>().currPhase = Phase.Dialogue;

        ProgressText();
    }



    public void ProgressText()
    {
        if (text.Count == 0)
        {
            EndText();
        }

        else
        {
            string sentence = text.Dequeue();
            Dialogue.text = sentence;
        }
    }



    public void EndText()
    {
        // Hide Box
        Box.color = new Color(1, 1, 1, 0);

        Name.text = "";
        Dialogue.text = "";

        // Set back to old phase
        bm.GetComponent<BattleManager>().currPhase = oldPhase;

        // If a new scene name is given
        if (nextScene == "")
        {
            StartCoroutine(LoadScene());
        }
    }


    // Loads next scen asyncrylly
    IEnumerator LoadScene()
    {
        // The Application loads the Scene in the background as the current Scene runs.
        // This is particularly good for creating loading screens.
        // You could also load the Scene by using sceneBuildIndex. In this case Scene2 has
        // a sceneBuildIndex of 1 as shown in Build Settings.

        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync("StormiteTest");

        // Wait until the asynchronous scene fully loads
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
    }
}
