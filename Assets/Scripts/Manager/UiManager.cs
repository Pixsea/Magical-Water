using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UiManager : MonoBehaviour
{
    // Used for the health text
    [SerializeField]
    private GameObject uiHealth;
    private Text uiHealthText;

    [SerializeField]
    private GameObject uiShieldHealth;
    private Text uiShieldHealthText;

    private GameObject player;
    private int playerHealth;
    private int shieldHealth;


    // Used for the health icon
    [SerializeField]
    private GameObject uiHeart;
    private Image uiHeartImage;

    // Used for the shield icon
    [SerializeField]
    private GameObject uiShield;
    private Image uiShieldImage;

    [SerializeField]
    private Sprite bigHeart;
    [SerializeField]
    private Sprite heart;


    // Start is called before the first frame update
    void Start()
    {
        uiHealthText = uiHealth.GetComponent<Text>();
        uiHeartImage = uiHeart.GetComponent<Image>();

        uiShieldHealthText = uiShieldHealth.GetComponent<Text>();
        uiShieldImage = uiShield.GetComponent<Image>();

        player = GameObject.Find("Player");
    }

    // Update is called once per frame
    void Update()
    {
        // Change health text to the current player health
        playerHealth = player.GetComponent<PlayerStats>().health;
        //uiHealthText.text = $"HP: {playerHealth}";
        uiHealthText.text = playerHealth.ToString();


        // Change shield health text to the current shield health
        shieldHealth = player.GetComponent<PlayerStats>().shieldHealth;
        uiShieldHealthText.text = shieldHealth.ToString();


        // Change health heart image depending if low on health
        if (playerHealth > 9)
        {
            uiHeartImage.sprite = bigHeart;
        }
        else
        {
            uiHeartImage.sprite = heart;
        }
    }
}
