using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class triggerLevelEnd : MonoBehaviour
{
    public Vector3 boxSize = new Vector3(5f, 5f, 5f); // Size of the box for physics check
    public Transform startPosition; 
    private GameManager gameManager;
    private ClearAllChildren levelBin;
    private LevelGenerator startLevelGenerator;
    private CreateGridOfObjects GridObjects;
    
    void Start()
    {
        gameManager = GameObject.Find("Game Manager").GetComponent<GameManager>();
        startPosition = GameObject.Find("Level Start").GetComponent<Transform>();
        levelBin = GameObject.Find("LevelBin").GetComponent<ClearAllChildren>();
        startLevelGenerator = GameObject.Find("Level Start").GetComponent<LevelGenerator>();
        //GridObjects = GameObject.Find("ParticleGridBin").GetComponent<CreateGridOfObjects>();
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(transform.position, boxSize);
    }

    private void teleportPlayerToStart(Transform playerPosition)
    {
        playerPosition.position = new Vector3(startPosition.position.x, startPosition.position.y + 1f,startPosition.position.z);
        levelBin.ClearChildrenInGameObject();
        gameManager.ResetGenLevelNumber();
        startLevelGenerator.GenerateLevel();
    }

    private void FixedUpdate()
    {
        Collider[] colliders = Physics.OverlapBox(transform.position, boxSize / 2);
        foreach (var collider in colliders)
        {
            if (collider.CompareTag("Player"))
            {
                // Start spawning objects
                // GridObjects.DestroyGrid();
                // GridObjects.SpawnGrid();
                teleportPlayerToStart(collider.gameObject.transform);
                return;
            }
        }
    }
}

