using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireProjectile : MonoBehaviour
{
    public GameObject projectilePrefab; // The projectile prefab to be instantiated
    public Transform launchPoint; // The point from which the projectile is fired
    public float fireDelay = 5f; // Delay between firing projectiles
    private bool fired = false;

    public void StartFiring()
    {
        StartCoroutine(FireProjectileWithDelay());
    }

    private IEnumerator FireProjectileWithDelay()
    {
        if(!fired)
        {
            fired = true;
            // Instantiate the projectile at the launch point
            GameObject projectile = Instantiate(projectilePrefab, launchPoint.position, launchPoint.rotation);

            // Wait for the specified delay before firing the next projectile
            yield return new WaitForSeconds(fireDelay);
            fired = false;
        }
    }
}
