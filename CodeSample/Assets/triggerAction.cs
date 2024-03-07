using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triggerAction : MonoBehaviour
{
    public GameObject objectToSpawn; // The object to instantiate
    public Transform spawnPoint; // The point where the object will be instantiated
    public float spawnInterval = 3f; // The interval between spawns
    public int maxSpawnedObjects = 5; // Maximum number of spawned objects allowed
    private float timer = 0f; // Timer to keep track of spawn time
    public float SpawnRange = 5f;
    public Vector3 boxSize = new Vector3(5f, 5f, 5f); // Size of the box for physics check
    private bool spawnItems = false;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }

    void Update()
    {
        // Increment the timer
        timer += Time.deltaTime;

        // Check if it's time to spawn a new object and there are less than the maximum allowed spawned objects
        if (timer >= spawnInterval && CountSpawnedObjects() < maxSpawnedObjects && spawnItems)
        {
            // Reset the timer
            timer = 0f;

            // Generate a random point within the spawn range
            Vector3 randomPoint = new Vector3(spawnPoint.position.x + Random.Range(-SpawnRange, SpawnRange),
                spawnPoint.position.y,
                spawnPoint.position.z + Random.Range(-SpawnRange, SpawnRange));
            
            // Spawn the object at the random point
            Instantiate(objectToSpawn, randomPoint, spawnPoint.rotation, spawnPoint);
        }
    }

    // Count the number of spawned objects in the world
    int CountSpawnedObjects()
    {
        return spawnPoint.childCount;
    }

    private void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, boxSize / 2);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                // Start spawning objects
                spawnItems = true;
                return;
            }
        }

        // Stop spawning objects
        spawnItems = false;
    }
}

