using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPosition : MonoBehaviour
{
    public Transform player;  // Reference to the player's transform
    public float smoothSpeed = 0.125f;  // Speed of the camera's smoothing

    public bool followOnX = true;
    public bool followOnY = true;
    public bool followOnZ = true;

    float playerXPosition = 0f;
    float playerYosition = 0f;
    float playerZPosition = 0f;

    public Vector3 offset;  // Offset of the camera from the player

    void FixedUpdate()
    {
        FollowPlayer();
    }

    void FollowPlayer()
    {
        playerXPosition = 0f;
        playerYosition = 0f;
        playerZPosition = 0f;

        if(followOnX)
        {
            playerXPosition = player.position.x + offset.x;
        }

        if(followOnY)
        {
            playerYosition = offset.y;
        }

        if(followOnZ)
        {
            playerZPosition = player.position.z + offset.z;
        }

        // Desired position based on the player's position and the offset
        Vector3 desiredPosition = new Vector3(playerXPosition, playerYosition, playerZPosition);
        
        // Smoothly interpolate between the camera's current position and the desired position
        Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);
        
        // Apply the smoothed position to the camera
        transform.position = smoothedPosition;
    }
}
