using UnityEngine;
using System;
using TMPro;

public class GameManager : MonoBehaviour
{
    //The Singleton instance so any script can easily access GameManager.Instance
    public static GameManager Instance { get; private set; }

    //The variable holding all the JSON data
    public GameConfig ConfigData { get; private set; }

    //Score tracking variables
    public int currentScore = 0;
    public TextMeshProUGUI scoreText;

    private void Awake()
    {
        //Set up the Singleton
        if(Instance == null) 
        {
            Instance = this;
        }
        else 
        {
            Destroy(gameObject);
        }

        LoadGameData();
    }

    private void LoadGameData() 
    {
        //Loading the JSON file from the Resources folder
        TextAsset jsonFile = Resources.Load<TextAsset>("doofus_diary"); 

        if(jsonFile != null)
        {
            ConfigData = JsonUtility.FromJson<GameConfig>(jsonFile.text);
            Debug.Log("JSON file loaded");
        }
    }

    //The player calls this function when it lands on a new platform
    public void AddScore()
    {
        currentScore++;
        if(scoreText != null) 
        {
            scoreText.text = "Score:\n" + currentScore;
        }
    }
}
