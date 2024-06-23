using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FacingFromCenterParticle : MonoBehaviour
{
    public ParticleSystem particleSystem;
    public Transform centerObject;
    public Vector3 rotationOffset; // Rotation offset in degrees
    public Vector3 randomVariance; // Max random variance in degrees

    void Start()
    {
        if (particleSystem == null)
        {
            particleSystem = GetComponent<ParticleSystem>();
        }

        if (centerObject == null)
        {
            Debug.LogError("Center object is not assigned!");
            return;
        }

        ParticleSystem.MainModule main = particleSystem.main;
        main.startRotation3D = true;

        // Make sure the particle system uses world space
        var mainModule = particleSystem.main;
        mainModule.simulationSpace = ParticleSystemSimulationSpace.World;

        // Subscribe to the OnParticleTrigger event
        particleSystem.trigger.SetCollider(0, centerObject.GetComponent<Collider>());
    }

    void LateUpdate()
    {
        ParticleSystem.Particle[] particles = new ParticleSystem.Particle[particleSystem.main.maxParticles];
        int numParticlesAlive = particleSystem.GetParticles(particles);

        for (int i = 0; i < numParticlesAlive; i++)
        {
            Vector3 particlePosition = particles[i].position;
            Vector3 directionAwayFromCenter = (particlePosition - centerObject.position).normalized;

            // Apply the rotation offset
            Quaternion baseRotation = Quaternion.LookRotation(directionAwayFromCenter) * Quaternion.Euler(rotationOffset);

            // Apply random variance
            Vector3 randomRotation = new Vector3(
                Random.Range(-randomVariance.x, randomVariance.x),
                Random.Range(-randomVariance.y, randomVariance.y),
                Random.Range(-randomVariance.z, randomVariance.z)
            );

            Quaternion finalRotation = baseRotation * Quaternion.Euler(randomRotation);
            particles[i].rotation3D = finalRotation.eulerAngles;
        }

        particleSystem.SetParticles(particles, numParticlesAlive);
    }
}

