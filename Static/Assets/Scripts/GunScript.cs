﻿using UnityEngine;
using System.Collections;

public class GunScript : MonoBehaviour {

    /* TWEAKABLE VARIABLES */

    // How many bullets are fired per shot.
	[SerializeField] int bulletsPerBurstMin = 2;
	[SerializeField] int bulletsPerBurstMax = 30;

    // How many shots can be fired per second.
	[SerializeField] int burstsPerSecondMin = 1;
    [SerializeField] int burstsPerSecondMax = 5;

    // Bullet spread.
    [SerializeField] float inaccuracyMin = 1f;
    [SerializeField] float inaccuracyMax = 10f;

    // How far bullets can travel.
    [SerializeField] float bulletRange = 500f;

    // How quickly the gun oscillates between extremes.
    [SerializeField] float oscSpeed = 0.3f;
    

    /* PREFABS */

	public GameObject bulletPrefab;
	public GameObject bulletStrikePrefab;

	public AudioSource shootAudio;
	public AudioSource bulletStrikeAudio;


    /* REFERENCES */
    ScoreControllerScript scoreControllerScript;
    Transform bulletSpawnTransform; // The point where bullets originate (ie the tip of the player's gun

    /* MISC */

    float timeSinceLastShot;
    GameObject[] bullets;    // Holds references to all bullets.
    int bulletIndex = 0;

	void Start ()
    {
        // Instantiate all bullet prefabs. (Just make 100 for now so I don't have to do math to figure out how many could potentially be on screen at once.)
        // Then, move them all to a far away place so the player doesn't see them.
        bullets = new GameObject[100];
        for (int i = 0; i < 100; i++)
        {
            bullets[i] = Instantiate(bulletPrefab);
            bullets[i].transform.position = new Vector3(0, -500, 0);
        }

        // Get the point from which bullets will spawn.
		bulletSpawnTransform = GameObject.Find ("BulletSpawnPoint").transform;

        // Get a reference to the score controller.
        scoreControllerScript = FindObjectOfType<ScoreControllerScript>();
	}


	void Update ()
	{
		// Get new firing variables based on current oscillation.
		int bulletsPerBurst = Mathf.RoundToInt(MyMath.Map(Mathf.Sin(Time.time*oscSpeed), -1f, 1f, bulletsPerBurstMax, bulletsPerBurstMin));
		float burstsPerSecond = MyMath.Map (Mathf.Sin (Time.time*oscSpeed), -1f, 1f, burstsPerSecondMin, burstsPerSecondMax);
		float inaccuracy = MyMath.Map (Mathf.Sin (Time.time*oscSpeed), -1f, 1f, inaccuracyMax, inaccuracyMin);
		shootAudio.pitch = MyMath.Map (Mathf.Sin (Time.time * oscSpeed), -1f, 1f, 0.8f, 2f);

        // Run shot timer.
        timeSinceLastShot += Time.deltaTime;

		if (Input.GetButton ("Fire1") && timeSinceLastShot >= 1/burstsPerSecond)
		{
			// Fire a burst
			FireBurst(bulletsPerBurst, inaccuracy);

            // Reset timer.
            timeSinceLastShot = 0f;
		}
	}

    
    // Firing a burst of bullets.
	void FireBurst(int numberOfBullets, float inaccuracy)
	{
        // Play shooting sound.
		shootAudio.Play ();

        // Fire the specified number of bullets.
		for (int i = 0; i < numberOfBullets; i++)
        {
			FireBullet (inaccuracy);
		}
	}


    // Firing an individual bullet.
	void FireBullet(float inaccuracy)
	{	
        // Rotate bullet spawner to get the direction of the next bullet.
		bulletSpawnTransform.localRotation = Quaternion.Euler(new Vector3(90+Random.insideUnitCircle.x * inaccuracy, 0, Random.insideUnitCircle.y * inaccuracy));

        // Declare variables for bullet size & position.
		float bulletScale;
		Vector3 bulletPosition;

        // Whether we should play the bullet hit sound.
        bool playAudio = false;

        // Raycast to see if the bullet hit an object and to see where it hit.
        RaycastHit hit;
        if (Physics.Raycast (bulletSpawnTransform.position, bulletSpawnTransform.up, out hit, bulletRange, 1 << 8)) {

			// Show particle effect
			Instantiate(bulletStrikePrefab, hit.point, Quaternion.identity);

			// The new bullet's y scale will be half of the ray's length
			bulletScale = hit.distance / 2;

			// The new bullet's position will be halfway down the ray
			bulletPosition = bulletSpawnTransform.up.normalized * (hit.distance / 2);

            // If the bullet hit an enemy...
			if (hit.collider.tag == "Enemy")
            {
                // Tell the enemy it was hurt.
                hit.collider.GetComponent<EnemyScript>().GetHurt();

                // Tell the score controller that the player hit an enemy with a bullet.
                scoreControllerScript.BulletHit();

                // We only want to play the bullet strike sound once, not once for every bullet that hit an enemy. So set a bool which tells the sound to play
                // later on.
				playAudio = true;
			}

		}

        // If the bullet did not strike anything give it a generic size and position.
        else
        {
			bulletScale = bulletRange/2;
			bulletPosition = bulletSpawnTransform.up.normalized*(bulletRange/2);
		}

        // If a bullet hit an enemy, play the bullet strike audio.
		if (playAudio)
        {
			bulletStrikeAudio.Play ();
		}

        // Fire bullet.
        bullets[bulletIndex].GetComponent<BulletScript>().GetFired(
            bulletSpawnTransform.position + bulletPosition,
            bulletSpawnTransform.rotation,
            new Vector3(bulletPrefab.transform.localScale.x, bulletScale, bulletPrefab.transform.localScale.z)
        );

        // Get a new bullet index.
        bulletIndex += 1;
        if (bulletIndex >= 100)
        {
            bulletIndex = 0;
        }
    }
}
