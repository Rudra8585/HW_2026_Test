using UnityEngine;

public class PulpitManager : MonoBehaviour
{
    public Pulpit pulpitPrefab;

    private void Start()
    {
        //Placing the first platform at the center. 
        //This will trigger a chain reaction that will keep spawning new platforms.
        SpawnPulpit(Vector3.zero);
    }

    private void SpawnPulpit(Vector3 position)
    {
        Pulpit newPulpit = Instantiate(pulpitPrefab, position, Quaternion.identity);

        //Fetching JSON constraints dynamically from GameManager
        var pulpitData = GameManager.Instance.ConfigData.pulpit_data;
        newPulpit.Initialize(
            pulpitData.min_pulpit_destroy_time,
            pulpitData.max_pulpit_destroy_time,
            pulpitData.pulpit_spawn_time
        );

        //Subscribe to the spawn event to handle adjacent placement
        newPulpit.OnSpawnNextRequested += HandleNextSpawn;
    }

    private void HandleNextSpawn(Vector3 currentPos)
    {
        //4 possible directions in 3D space
        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
        Vector3 randomDir = directions[Random.Range(0, directions.Length)];

        //Multiply by 9 to place it perfectly adjacent to a 9x9 platform
        Vector3 newPos = currentPos + (randomDir * 9f);

        SpawnPulpit(newPos);
    }
}
