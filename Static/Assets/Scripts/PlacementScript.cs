using UnityEngine;
using System.Collections;

public class PlacementScript : MonoBehaviour {

	LevelGenScript lgn;
	bool placed = false;

	void Start ()
	{
		lgn = GameObject.Find ("Game Manager").GetComponent<LevelGenScript> ();

		Vector3 newPostion = transform.position;
		Vector3 newScale = transform.localScale;

		while (!placed)
		{
			if (gameObject.tag == "Obstacle") {
				
				// Get my size
				newScale.x = Random.Range (lgn.obstacleMinSize, lgn.obstacleMaxSize);
				newScale.y = transform.localScale.y;
				newScale.z = Random.Range (lgn.obstacleMinSize, lgn.obstacleMaxSize);

				// Get my position
				newPostion.x = Random.Range (-lgn.levelSize + newScale.x / 2, lgn.levelSize - newScale.x / 2);
				newPostion.y = transform.localPosition.y;
				newPostion.z = Random.Range (-lgn.levelSize + newScale.z / 2, lgn.levelSize - newScale.z / 2);

				// Test this location
				Collider[] overlaps = Physics.OverlapBox (newPostion, newScale * 0.6f);
				foreach (Collider c in overlaps) {
					if (c.tag == "Player" || c.tag == "Enemy") {
						print ("Not good");
						placed = false;
					} else {
						placed = true;
					}
				}
			}

			else if (gameObject.tag == "Enemy") {
				
				// Get my position
				newPostion.x = Random.Range(-lgn.levelSize + transform.localScale.x/2, lgn.levelSize - transform.localScale.x/2);
				newPostion.y = transform.position.y;
				newPostion.z = Random.Range(-lgn.levelSize + transform.localScale.z/2, lgn.levelSize - transform.localScale.z/2);

				// Test this location
				Collider[] overlaps = Physics.OverlapSphere (newPostion, transform.localScale.x);
				foreach (Collider c in overlaps) {
					if (c.tag == "Player" || c.tag == "Enemy" || c.tag == "Obstacle") {
						print ("Not good");
						placed = false;
					} else {
						placed = true;
					}
				}
			}
		}

		transform.position = newPostion;
		transform.localScale = newScale;

		enabled = false;
	}
}
