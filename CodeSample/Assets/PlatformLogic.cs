using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformLogic : MonoBehaviour
{
    public Transform target; // The object to follow
    public float smoothness = 5f; // Adjust this value to control the smoothness of the lift motion
    public bool followTarget = true;

    public void FollowTarget()
    {
        if (target != null && followTarget == true)
        {
            // Get the target's position
            Vector3 targetPosition = target.position;

            // Set the follow object's position to match the target's X and Z positions
            transform.position = new Vector3(targetPosition.x, transform.position.y, targetPosition.z);
        }
    }

    public void Lift()
    {
        followTarget = false;
        // Set the target position with the desired height
        Vector3 targetPosition = new Vector3(transform.position.x, 2f, transform.position.z);

        // Smoothly move the object towards the target position
        transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothness);
    }

    public void Reset()
    {
        if(transform.position.y != 0f)
        {
            followTarget = true;
            // Set the target position with the desired height
            Vector3 targetPosition = new Vector3(transform.position.x, 0f, transform.position.z);

            // Smoothly move the object towards the target position
            transform.position = Vector3.Lerp(transform.position, targetPosition, Time.deltaTime * smoothness);
        }

        if(transform.position.y <= 0.1f)
        {
            transform.position = new Vector3(transform.position.x,0f, transform.position.z);
            FollowTarget();
        }
    }
}
