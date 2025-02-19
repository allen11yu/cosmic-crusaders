using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FireMissiles : MonoBehaviour
{
    public PlayerStats playerStats; 
    public GameObject regularMissilePrefab;
    public GameObject homingMissilePrefab;
    public Transform firingPoint;
    public float regularMissileForce = 1000f;

    public float fireRate = 1f;
    private float nextFireTime = 0f;

    public AudioClip MissileSound;
    private AudioSource audioSource; 

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (playerStats.isMissileUnlock && Input.GetButton("Fire2") && Time.time > nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            FireMissile(regularMissilePrefab, regularMissileForce);
            PlayMissileSound();
        }

        if (playerStats.isMissileUnlock && Input.GetKey("x") && Time.time > nextFireTime)
        {
            nextFireTime = Time.time + 1f / fireRate;
            FireMissile(homingMissilePrefab, 0f);
            PlayMissileSound();
        }
    }

    void FireMissile(GameObject missilePrefab, float force)
    {
        GameObject missile = Instantiate(missilePrefab, firingPoint.position, firingPoint.rotation);

        if (force > 0f)
        {
            missile.GetComponent<Rigidbody>().AddForce(firingPoint.forward * force);
        }
    }

    void PlayMissileSound()
    {
        if (MissileSound != null)
        {
            audioSource.PlayOneShot(MissileSound);
        }
    }
}