using UnityEngine;
using System.Collections;

public class MusicScript : MonoBehaviour {

	public AudioClip[] tracks;
	AudioSource audioSource;

	void Start () {
		audioSource = GetComponent<AudioSource> ();
		int clipIndex = Random.Range(0, tracks.Length);
		audioSource.clip = tracks [clipIndex];
	}

	void ChooseNewClip()
	{
		int clipIndex = Random.Range(0, tracks.Length);
		audioSource.clip = tracks [clipIndex];
		audioSource.Play ();
	}
	
	void Update () {
		if (!audioSource.isPlaying) {
			ChooseNewClip ();
		}
	}
}
