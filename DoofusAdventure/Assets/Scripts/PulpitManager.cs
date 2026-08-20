using UnityEngine;

public class PulpitManager : MonoBehaviour
{
    public Pulpit pulpitPrefab;
    
    // Track the previous position to completely prevent backtracking/overlapping
    private Vector3 previousPos = new Vector3(999f, 999f, 999f); 

    public void SpawnInitialPulpit()
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
        Vector3 newPos = Vector3.zero;
        bool validPosition = false;

        // A while loop to reroll the direction if it tries to spawn on the old platform
        while (!validPosition)
        {
            Vector3 randomDir = directions[Random.Range(0, directions.Length)];
            
            //Multiply by 9 to place it perfectly adjacent to a 9x9 platform
            newPos = currentPos + (randomDir * 9f);
            
            // Only checking to prevent backtracking
            if (newPos != previousPos)
            {
                validPosition = true;
            }
        }

        // Save the current position as the new "previous" position before spawning
        previousPos = currentPos;
        SpawnPulpit(newPos);
    }
}