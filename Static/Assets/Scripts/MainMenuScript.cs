using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class MainMenuScript : MonoBehaviour {

	public void PlayButton() {
		SceneManager.LoadScene (1);
	}

	public void QuitButton() {
		Application.Quit ();
	}

	public void InfoButton() {
		SceneManager.LoadScene (2);
	}
}
