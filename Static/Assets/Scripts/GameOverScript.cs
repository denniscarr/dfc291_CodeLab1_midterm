using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.Characters.FirstPerson;
using System.Collections;

public class GameOverScript : MonoBehaviour {

	public GameObject gameOverScreen;

	GameObject player;
	bool gameOver;

	void Start ()
	{
		player = GameObject.Find ("FPSController");
	}

	void Update ()
	{
		if (gameOver) {
			Cursor.lockState = CursorLockMode.None;
			Cursor.visible = true;
			player.GetComponent<FirstPersonController> ().m_MouseLook.lockCursor = false;
			player.GetComponent<CharacterController> ().center = new Vector3 (player.GetComponent<CharacterController> ().center.x, player.GetComponent<CharacterController> ().center.y-0.5f, player.GetComponent<CharacterController> ().center.z);
			player.GetComponent<FirstPersonController> ().m_WalkSpeed = 0;
			player.GetComponent<FirstPersonController> ().m_RunSpeed = 0;
			GameObject.Find ("Gun").GetComponent<GunScript> ().enabled = false;
			GameObject.Find ("Background Grain").GetComponent<MeshRenderer> ().material.color = new Color (GameObject.Find ("Background Grain").GetComponent<MeshRenderer> ().material.color.r+0.1f, GameObject.Find ("Background Grain").GetComponent<MeshRenderer> ().material.color.g, GameObject.Find ("Background Grain").GetComponent<MeshRenderer> ().material.color.b);
			gameOverScreen.SetActive(true);
			GameObject.Find ("Final Score Number").GetComponent<Text> ().text = GameObject.Find ("Score Display").GetComponent<ScoreControllerScript> ().score.ToString();
		}
	}

	void GameOver() {
		gameOver = true;
	}
}
