using UnityEngine;
using System.Collections.Generic; 

public class PulpitManager : MonoBehaviour
{
    public Pulpit pulpitPrefab;
    
    //Track the last few positions to completely prevent looping exploits
    private List<Vector3> recentPositions = new List<Vector3>();
    private int memoryLimit = 4; //Remembers the last 4 platforms

    //Prevents the player from exploiting the fairness system
    private int fairnessCooldown = 0;

    public void SpawnInitialPulpit()
    {
        //Placing the first platform at the center. 
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

        //Adding this new platform's position to our memory list
        recentPositions.Add(position);

        //Removing the oldest position if our memory gets too long
        if (recentPositions.Count > memoryLimit)
        {
            recentPositions.RemoveAt(0);
        }
    }

    private void HandleNextSpawn(Vector3 currentPos)
    {
        //Reduce the cooldown every time a new platform generates
        if (fairnessCooldown > 0)
        {
            fairnessCooldown--;
        }

        //Getting the player's exact location to make a fair decision
        PlayerController player = Object.FindFirstObjectByType<PlayerController>();
        Vector3 playerPos = player != null ? player.transform.position : currentPos;

        //Restoring all 4 directions so the path can snake around wildly
        Vector3[] directions = { Vector3.forward, Vector3.back, Vector3.left, Vector3.right };
        
        Vector3 chosenPos = Vector3.zero;
        bool validPosition = false;

        //First, we roll a completely random direction
        while (!validPosition)
        {
            Vector3 randomDir = directions[Random.Range(0, directions.Length)];
            
            //Multiply by 9 to place it perfectly adjacent to a 9x9 platform
            chosenPos = currentPos + (randomDir * 9f);
            
            //Make sure this spot isn't anywhere in our recent memory
            if (!recentPositions.Contains(chosenPos))
            {
                validPosition = true;
            }
        }

        //Calculate the distance from the player to this newly chosen platform center
        float distanceToChosen = Vector3.Distance(playerPos, chosenPos);

        //The game will only help the player if the cooldown is at zero
        if (distanceToChosen > 10f && fairnessCooldown == 0)
        {
            bool overrideTriggered = false;

            //Calculate the current path direction
            Vector3 pathDirection = Vector3.forward; 
            if (recentPositions.Count >= 2)
            {
                //Subtracting the previous platform from the current one gives us the exact direction
                pathDirection = (currentPos - recentPositions[recentPositions.Count - 2]) / 9f;
            }

            Vector3 straightPos = currentPos + (pathDirection * 9f);

            //Force the path straight to prevent diagonal staircases
            if (!recentPositions.Contains(straightPos) && Vector3.Distance(playerPos, straightPos) < 10f)
            {
                chosenPos = straightPos;
                distanceToChosen = Vector3.Distance(playerPos, straightPos);
                overrideTriggered = true;
            }
            else
            {
                //Fallback: Scan for the closest safe option if straight is blocked
                foreach (Vector3 dir in directions)
                {
                    Vector3 testPos = currentPos + (dir * 9f);
                    
                    if (!recentPositions.Contains(testPos))
                    {
                        float distanceToTest = Vector3.Distance(playerPos, testPos);
                        
                        if (distanceToTest < distanceToChosen)
                        {
                            chosenPos = testPos;
                            distanceToChosen = distanceToTest;
                            overrideTriggered = true;
                        }
                    }
                }
            }

            //Trigger the cooldown if the fairness override was used
            if (overrideTriggered)
            {
                fairnessCooldown = 2;
                Debug.Log("Fairness Override used! Forced a straight path to prevent exploits. Cooldown active.");
            }
        }

        //Spawning the finalized platform
        SpawnPulpit(chosenPos);
    }
}