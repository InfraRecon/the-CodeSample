using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyParticleSystem : MonoBehaviour
{
    public bool DestroySelf = true;
    private void OnTriggerEnter(Collider other)
    {
        // Check if the other GameObject has a ParticleSystem component
        ParticleSystem particleSystem = other.GetComponent<ParticleSystem>();

        if (particleSystem != null)
        {
            // Destroy the GameObject with the ParticleSystem component
            Destroy(other.gameObject);

            if(DestroySelf)
            {
                Destroy(gameObject,5f);
            }
        }
    }
}
