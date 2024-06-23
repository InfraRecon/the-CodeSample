using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtGameObject : MonoBehaviour
{
    // The target object to look at
    private Transform target;
    
    // The range within which the object should look at the target
    public float range = 10f;

    // The speed of rotation
    public float rotationSpeed = 2f;

    private FireProjectile fireProjectile;

    void Start()
    {
        target = GameObject.Find("Main Character").GetComponent<Transform>();
        fireProjectile = GetComponent<FireProjectile>();
    }

    // Update is called once per frame
    void Update()
    {
        if (target == null)
        {
            Debug.LogWarning("Target not set for LookAtGameObject script.");
            return;
        }

        // Calculate the distance to the target
        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        // If the target is within range, look at it smoothly
        if (distanceToTarget <= range)
        {
            // Get the direction to the target
            Vector3 direction = target.position - transform.position;
            direction.y = 0; // Keep only the horizontal direction

            // Calculate the rotation required to look at the target
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // Smoothly rotate towards the target
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

            fireProjectile.StartFiring();
        }
    }

    // Draw Gizmos to visualize the range in the editor
    void OnDrawGizmos()
    {
        // Set the Gizmos color
        Gizmos.color = Color.green;

        // Draw a wire sphere to represent the range
        Gizmos.DrawWireSphere(transform.position, range);

        // If the target is within range, draw a line to the target
        if (target != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, target.position);
            if (distanceToTarget <= range)
            {
                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, target.position);
            }
        }
    }
}
