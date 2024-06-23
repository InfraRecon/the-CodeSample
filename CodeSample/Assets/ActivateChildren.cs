using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ActivateChildren : MonoBehaviour
{
    private Transform CameraFollowTarget;
    private Transform CameraLookAtTarget;
    public CinemachineVirtualCamera virtualCamera;

    public Vector3 boxCenter; // The center of the OverlapBox
    public Vector3 boxSize; // The size of the OverlapBox
    public LayerMask layerMask; // LayerMask to filter objects in the OverlapBox
    public GameObject cameraObject; // The parent object whose children will be activated

    private void Start()
    {
        CameraFollowTarget = GameObject.Find("Main Character").GetComponent<Transform>();
        CameraLookAtTarget = GameObject.Find("mixamorig:Hips").GetComponent<Transform>();
        virtualCamera.m_Follow = CameraFollowTarget;
        virtualCamera.m_LookAt = CameraLookAtTarget;
    }

    private void Update()
    {
        // Check for objects within the OverlapBox
        Collider[] hitColliders = Physics.OverlapBox(new Vector3(transform.position.x + boxCenter.x, transform.position.y + boxCenter.y, transform.position.z + boxCenter.z), boxSize / 2, Quaternion.identity, layerMask);

        if (hitColliders.Length > 0)
        {
            // If any objects are detected, activate children
            SetActiveChildren(cameraObject, true);
        }
        else
        {
            // If no objects are detected, deactivate children
            SetActiveChildren(cameraObject, false);
        }
    }

    private void SetActiveChildren(GameObject parent, bool isActive)
    {
        // Set the active state of all children of the specified parent object
        parent.SetActive(isActive);
    }

    // Optional: Visualize the OverlapBox in the Scene view for easier debugging
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireCube(boxCenter, boxSize);
    }
}
