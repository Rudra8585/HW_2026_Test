using UnityEngine;

public class Pulpit : MonoBehaviour
{
    private float lifetime;
    private float spawnNextTimer;
    private bool hasSpawnedNext = false;

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

        //Trigger the spawn of the overlapping pulpit
        if(lifetime <= spawnNextTimer && !hasSpawnedNext)
        {
            OnSpawnNextRequested?.Invoke(transform.position);
            hasSpawnedNext = true;
        }

        //Destroy when time runs out
        if(lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }
}
