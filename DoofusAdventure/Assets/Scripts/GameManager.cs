using UnityEngine;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    //The Singleton instance so any script can easily access GameManager.Instance
    public static GameManager Instance { get; private set; }

    //The variable holding all the JSON data
    public GameConfig ConfigData { get; private set; }

    //Score tracking variables
    public int currentScore = 0;
    public TextMeshProUGUI scoreText;

    //UI screen references
    public GameObject startScreen;
    public GameObject gameOverScreen;

    //Controls whether scripts should run
    public bool isGameActive { get; private set; } = false;

    //This will start the game directly instead of going to the start menu.
    //Its for when the player restarts the game after losing
    private static bool autoStart = false;

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

    private void Start()
    {
        //If we hit restart previously, it will skip the menu and jump right in
        if(autoStart)
        {
            autoStart = false;
            StartGame();
        }
        else
        {
            //Set up initial UI state
            startScreen.SetActive(true);
            gameOverScreen.SetActive(false);
        }
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

    //This function starts the game
    public void StartGame()
    {
        isGameActive = true;
        startScreen.SetActive(false);
        gameOverScreen.SetActive(false);

        //This will tell the pulpitManager to spawn the first platform now
        FindFirstObjectByType<PulpitManager>().SpawnInitialPulpit();
    }

    //This function stops the game
    public void TriggerGameOver()
    {
        isGameActive = false;
        gameOverScreen.SetActive(true);
    }

    //This function restarts the game
    public void RestartGame()
    {
        //Tell the next scene load to skip the Start Menu
        autoStart = true;
        
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
