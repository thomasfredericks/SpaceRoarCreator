using UnityEngine;
using System.Collections;

public class KeepInFront1 : MonoBehaviour {

	public Transform target;
	public float distance = 86;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = target.position + new Vector3(0,0,distance);

	
	}
}
