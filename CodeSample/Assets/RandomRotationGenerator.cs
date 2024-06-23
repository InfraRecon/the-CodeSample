using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRotationGenerator : MonoBehaviour
{
    public bool xRot = false;
    public bool yRot = true;
    public bool zRot = false;

    public int rotationMin = 0;
    public int rotationMax = 360;

    void Start()
    {
        if(xRot)
        {
            // Assign a random rotation on the y-axis
            transform.rotation = Quaternion.Euler(Random.Range(rotationMin, rotationMax), transform.eulerAngles.y,transform.eulerAngles.z);
        }

        if(yRot)
        {
            // Assign a random rotation on the y-axis
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, Random.Range(rotationMin, rotationMax), transform.eulerAngles.z);
        }
        
        if(zRot)
        {
            // Assign a random rotation on the y-axis
            transform.rotation = Quaternion.Euler(transform.eulerAngles.x, transform.eulerAngles.y,Random.Range(rotationMin, rotationMax));
        }
    }
}
