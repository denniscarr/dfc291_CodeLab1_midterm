using UnityEngine;
using System.Collections;

public class ScreenScript : MonoBehaviour {

	int myHealth = 5;

	public GameObject[] healthBlocks;

	void GetHurt()
    {
        // Decrease health.
		myHealth -= 1;

        // If health is now less than zero, trigger a game over.
        if (myHealth <= 0)
        {
            GameObject.Find("Game Manager").SendMessage("GameOver");
        }

        // Otherwise, stop displaying the corresponding health block.
        else
        {
            healthBlocks[myHealth - 1].SetActive(false);
        }
    }
}
