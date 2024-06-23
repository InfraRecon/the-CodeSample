using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HomingProjectile : MonoBehaviour
{
    private Transform target;  // The target the projectile will home in on
    public float speed = 20f; // Speed of the projectile
    public float rotationSpeed = 5f; // How fast the projectile adjusts its direction
    public float lifeTime = 1f;

    void Start()
    {
        target = GameObject.Find("Main Character").GetComponent<Transform>();
        Destroy(gameObject,lifeTime); // Destroy the projectile
    }

    void Update()
    {
        if (target == null)
        {
            // If there's no target, keep moving forward
            transform.Translate(Vector3.forward * speed * Time.deltaTime);
            return;
        }

        // Calculate the direction to the target
        Vector3 direction = (target.position - transform.position).normalized;

        // Create a rotation towards the target
        Quaternion targetRotation = Quaternion.LookRotation(direction);

        // Smoothly rotate towards the target
        transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        // Move the projectile forward
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    private void OnTriggerEnter(Collider other)
    {
        // Add collision logic here (e.g., damage to the target, destroy the projectile)
        if (other.transform == target)
        {
            // Implement what should happen when the projectile hits the target
            Destroy(gameObject); // Destroy the projectile
        }
    }
}
