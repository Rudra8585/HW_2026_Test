using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance;

    private void Awake()
    {
        //This will ensure that the music object never gets destroyed when changing scenes or restarting
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            //This will destroy duplicate music objects
            Destroy(gameObject);
        }
    }
}