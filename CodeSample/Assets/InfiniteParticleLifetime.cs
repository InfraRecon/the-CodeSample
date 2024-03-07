using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InfiniteParticleLifetime : MonoBehaviour
{
    private ParticleSystem particleSystemComponent;
    private ParticleSystem.Particle[] particles;

    public float speed = 1f; // Speed of oscillation
    public float amplitude = 1f; // Amplitude of oscillation

    void Start()
    {
        particleSystemComponent = GetComponent<ParticleSystem>();
        if (particleSystemComponent == null)
        {
            Debug.LogError("Particle system component not found!");
            enabled = false; // Disable the script if particle system component is not found
        }
        
        particles = new ParticleSystem.Particle[particleSystemComponent.main.maxParticles];
        
        // Disable auto destruction of particles
        var mainModule = particleSystemComponent.main;
        mainModule.stopAction = ParticleSystemStopAction.None;
    }

    void LateUpdate()
    {
        int numParticlesAlive = particleSystemComponent.GetParticles(particles);

        for (int i = 0; i < numParticlesAlive; i++)
        {
            // Reset particle lifetime
            particles[i].remainingLifetime = particles[i].startLifetime;

            // Calculate vertical offset using a sine function
            float offset = Mathf.Sin(Time.time * speed + i) * amplitude;

            // Update particle position
            particles[i].position += Vector3.up * offset;
        }

        particleSystemComponent.SetParticles(particles, numParticlesAlive);

        // Check if the particle system is still playing
        if (!particleSystemComponent.isPlaying)
        {
            particleSystemComponent.Play();
        }
    }
}
