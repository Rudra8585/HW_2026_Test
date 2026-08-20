using UnityEngine;
using TMPro;
using System.Collections;

public class Pulpit : MonoBehaviour
{
    private float lifetime;
    private float spawnNextTimer;
    private bool hasSpawnedNext = false;
    public bool hasBeenScored = false;
    public TextMeshPro timerText;

    //An event that requests the Manager to spawn the next platform
    public System.Action<Vector3> OnSpawnNextRequested;

    //Animation variables for spawning and despawning
    private Vector3 originalScale;
    private float animDuration = 0.3f;
    private bool isShrinking = false;

    private void Start()
    {
        //Save the platform's original size(9, 0.5, 9)
        originalScale = transform.localScale;

        //Immediately shrinks it to 0 so its invisible on frame 1
        transform.localScale = Vector3.zero;

        //Start of growing animation
        StartCoroutine(GrowRoutine());
    }

    public void Initialize(float minTime, float maxTime, float spawnThreshold)
    {
        lifetime = Random.Range(minTime, maxTime);
        spawnNextTimer = spawnThreshold;
    }

    private void Update()
    {
        //Stops counting down if the game is over
        if(!GameManager.Instance.isGameActive) return;

        //If pulpit already shrinking and about to be destroyed, this stops running the timer logic
        if(isShrinking) return;

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

        //Shrinks the pulpit to 0
        if(lifetime <= 0)
        {
            isShrinking = true;
            StartCoroutine(ShrinkRoutine());
        }
    }

    //Animation Coroutines:
    private IEnumerator GrowRoutine()
    {
        float elapsedTime = 0f;
        
        while (elapsedTime < animDuration)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, elapsedTime / animDuration);
            elapsedTime += Time.deltaTime;
            yield return null; 
        }
        
        // Ensure it snaps exactly to its full size at the very end
        transform.localScale = originalScale; 
    }

    private IEnumerator ShrinkRoutine()
    {
        float elapsedTime = 0f;
        Vector3 startScale = transform.localScale;
        
        while (elapsedTime < animDuration)
        {
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, elapsedTime / animDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
        // Once the animation finishes, cleanly destroy the object
        Destroy(gameObject);
    }
}