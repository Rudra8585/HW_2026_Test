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

    [Header("Nature Decorations")]
    //Arrays to hold the 3d models and the 9 empty game objects on the cubes
    public GameObject[] naturePrefabs; 
    public Transform[] spawnPoints;

    private void Start()
    {
        //Save the platform's original size(9, 0.5, 9)
        originalScale = transform.localScale;

        //Spawns the random bushes and grass
        SpawnDecorations();

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

    private void SpawnDecorations()
    {
        //Chooses how many props to spawn on this specific pulpit
        int propCount = Random.Range(4, 10); 

        for (int i = 0; i < propCount; i++)
        {
            //Picks a random model from the array
            int randomIndex = Random.Range(0, naturePrefabs.Length);
            
            //Spawns the chosen model
            GameObject decor = Instantiate(naturePrefabs[randomIndex], transform.position, Quaternion.identity, transform);
            
            //Places it at a random local location on the 9x9 grid
            //X and Z are between -0.48 and 0.48 so they stay slightly inside the absolute edges
            float randomX = Random.Range(-0.48f, 0.48f);
            float randomZ = Random.Range(-0.48f, 0.48f);
            
            decor.transform.localPosition = new Vector3(randomX, 0.39f, randomZ); 
            
            //Spins the model randomly so they dont look completely identical
            decor.transform.localRotation = Quaternion.Euler(0, Random.Range(0f, 360f), 0);

            //Chooses a random uniform size for the prop
            float randomSize = Random.Range(0.9f, 1.5f); 
            
            //Counteracts the parent's 9, 0.5, 9 scale so the props dont flatten
            decor.transform.localScale = new Vector3(randomSize / 9f, randomSize / 0.5f, randomSize / 9f);
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
        
        //Ensure it snaps exactly to its full size at the very end
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
        
        //Once the animation finishes, cleanly destroy the object
        Destroy(gameObject);
    }
}