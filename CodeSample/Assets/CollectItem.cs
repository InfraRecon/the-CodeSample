using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectItem : MonoBehaviour
{
    public float sphereRadius = 1f; // Radius of the sphere
    public LayerMask destroyableLayers; // Layers of objects that can be destroyed
    public GameManager gameManager;

    void Update()
    {
        // Check for overlap with the sphere
        Collider[] colliders = Physics.OverlapSphere(transform.position, sphereRadius, destroyableLayers);

        // Destroy overlapped objects
        foreach (Collider collider in colliders)
        {
            Destroy(collider.gameObject);
            gameManager.AddToOrbas();
        }
    }

    // Draw a gizmo to visualize the sphere in the Unity editor
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, sphereRadius);
    }
}
