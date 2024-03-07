using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowRotation : MonoBehaviour
{
    public Transform target; // The target object whose rotation will be followed

    void Update()
    {
        if (target != null)
        {
            // Copy the rotation from the target object
            transform.rotation = target.rotation;
        }
        else
        {
            Debug.LogWarning("Target is not assigned. Please assign a target object in the inspector.");
        }
    }
}
