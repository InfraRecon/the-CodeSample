using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomObjectPlacer : MonoBehaviour
{
    public GameObject[] objectsToInstantiate;   // the object to instantiate on mesh faces
    public int numObjectsToInstantiate = 1;  // the number of objects to instantiate on each face
    public float offset = 0.01f;             // the distance to offset the instantiated objects from the mesh face
    public bool randomSeed = true;
    public int randomPossibility = 5;
    private int spawnNum = 0;

    private Mesh mesh; // the mesh to place objects on

    private GameObject emptyGameObject; // Assign the prefab in the Inspector

    void Start()
    {   
        createEmptyObject();
        mesh = GetComponent<MeshFilter>().mesh; // get the mesh from the object's MeshFilter component
        PlaceObjectsOnMeshFaces();
    }

    void PlaceObjectsOnMeshFaces()
    {
        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            int[] triangles = mesh.GetTriangles(i); // get the triangles that make up the submesh

            for (int j = 0; j < triangles.Length; j += 3) // loop through each triangle
            {
                Vector3 v1 = transform.TransformPoint(mesh.vertices[triangles[j]]);
                Vector3 v2 = transform.TransformPoint(mesh.vertices[triangles[j + 1]]);
                Vector3 v3 = transform.TransformPoint(mesh.vertices[triangles[j + 2]]);

                Vector3 center = (v1 + v2 + v3) / 3f; // calculate the center of the triangle

                for (int k = 0; k < numObjectsToInstantiate; k++) // instantiate objects on the triangle
                {
                    // calculate a random point on the triangle
                    Vector3 randomPoint = RandomPointOnTriangle(v1, v2, v3);
                    
                    // calculate the normal of the triangle
                    Vector3 normal = Vector3.Cross(v2 - v1, v3 - v1).normalized;
                    
                    int objRandomNum = Random.Range(0,objectsToInstantiate.Length);

                    if(randomSeed)
                    {
                        spawnNum = Random.Range(0,randomPossibility);
                    }
                    else
                    {
                        spawnNum = 0;
                    }

                    if(spawnNum == 0)
                    {        
                        // instantiate the object at the random point, offset from the mesh face
                        GameObject instObject = Instantiate(objectsToInstantiate[objRandomNum], randomPoint + normal * offset, Quaternion.identity);
                        instObject.transform.parent = emptyGameObject.transform;
                    }
                }
            }
        }
    }

    private void createEmptyObject()
    {
        // Create an empty GameObject
        emptyGameObject = new GameObject("EmptyGameObject");
        
        // You can modify the empty GameObject or its components here
        // For example, you can access its Transform component and change its position:
        emptyGameObject.transform.position = transform.position;

        //emptyGameObject.AddComponent(System.Type.GetType("DestroyOnDistance"));

        Transform newParent = transform;
        emptyGameObject.transform.SetParent(newParent);
    }

    private System.Type GetScriptType()
    {
        // Replace "MyScript" with the desired script name or modify this logic as needed
        return System.Type.GetType("MyScript");
    }


    // helper function to calculate a random point on a triangle
    Vector3 RandomPointOnTriangle(Vector3 v1, Vector3 v2, Vector3 v3)
    {
        float r1 = Random.value;
        float r2 = Random.value;

        if (r1 + r2 > 1f)
        {
            r1 = 1f - r1;
            r2 = 1f - r2;
        }

        return r1 * v1 + r2 * v2 + (1f - r1 - r2) * v3;
    }
}
