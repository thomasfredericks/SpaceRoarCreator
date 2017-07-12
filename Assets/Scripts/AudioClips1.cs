using UnityEngine;
using System.Collections;

public class AudioClips1 : MonoBehaviour {

	public AudioClip[] audioClips;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public AudioClip Get () {
		return  audioClips[Random.Range(0, audioClips.Length)];
	}
}
