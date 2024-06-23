using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearAllChildren : MonoBehaviour
{
    public void ClearChildrenInGameObject()
    {
        // Loop through each child of the parent GameObject
        foreach (Transform child in transform)
        {
            // Destroy the child GameObject
            Destroy(child.gameObject);
        }
    }
}
