using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CreateGridOfObjects : MonoBehaviour
{
    public GameObject objectToSpawn; // The prefab to be instantiated
    public int rows = 5;             // Number of rows in the grid
    public int columns = 5;          // Number of columns in the grid
    public Vector3 offset = new Vector3(1.5f, 0, 1.5f); // Offset between objects

    void Start()
    {
        SpawnGrid();
    }

    public void SpawnGrid()
    {
        if (objectToSpawn == null)
        {
            Debug.LogError("Object to spawn is not set.");
            return;
        }

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Vector3 position = new Vector3(transform.position.x + column * offset.x - rows * offset.x/2.5f, transform.position.y,transform.position.z + row * offset.z - columns * offset.z/2.5f);
                Instantiate(objectToSpawn, position, Quaternion.identity, transform);
            }
        }
    }

    public void DestroyGrid()
    {
        foreach (Transform child in transform)
        {
            Destroy(child.gameObject);
        }
    }
}
