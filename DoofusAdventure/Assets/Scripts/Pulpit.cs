using UnityEngine;
using TMPro; // Required to control TextMeshPro objects!

public class Pulpit : MonoBehaviour
{
    private float lifetime;
    private float spawnNextTimer;
    private bool hasSpawnedNext = false;
    public bool hasBeenScored = false;
    public TextMeshPro timerText;

    //An event that requests the Manager to spawn the next platform
    public System.Action<Vector3> OnSpawnNextRequested;

    public void Initialize(float minTime, float maxTime, float spawnThreshold)
    {
        lifetime = Random.Range(minTime, maxTime);
        spawnNextTimer = spawnThreshold;
    }

    private void Update()
    {
        lifetime -= Time.deltaTime;

        //Updates the 3D text every frame. 
        if (timerText != null)
        {
            // Don't let it show negative numbers right before destroying
            timerText.text = Mathf.Max(0, lifetime).ToString("F1"); 
        }

        //Trigger the spawn of the overlapping pulpit
        if(lifetime <= spawnNextTimer && !hasSpawnedNext)
        {
            if (Object.FindObjectsByType<Pulpit>(FindObjectsSortMode.None).Length < 2)
            {
                OnSpawnNextRequested?.Invoke(transform.position);
                hasSpawnedNext = true;
            }
        }

        //Destroy when time runs out
        if(lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }
}