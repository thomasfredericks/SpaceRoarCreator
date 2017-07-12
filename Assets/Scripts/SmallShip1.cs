using UnityEngine;
using System.Collections;

public class SmallShip1 : MonoBehaviour {

	private float speedZ;
	public float speedZMax = 10f;
	public float speedZMin = 3f;

	private float changeDelta = 0 ;
	private float lastTimeChanged = 0;

	public float	changeDeltaMax = 3;
	public float	changeDeltaMin = 0.5f;

	public Transform centerOn;

	private Quaternion targetQuaternion;

	public float turnSpeed = 0.05f;


	//internal GameObject audioClipsProvider;
	
	//public GameObject explosion;
	
	
	
	
	// Use this for initialization
	void Start () {
		
		//audio.clip = audioClipsProvider.GetComponent<AudioClips1>().Get(); 
		//audio.Play();
		RandomPositionAndRotation();

		
	}
	
	// Update is called once per frame
	void Update () {
		float timeElapsed = Time.time-lastTimeChanged;

		transform.rotation = Quaternion.Slerp (transform.rotation, targetQuaternion, timeElapsed * turnSpeed);
		transform.Translate(new Vector3(0,0,speedZ*Time.deltaTime),Space.Self);

		if ( Vector3.Distance(transform.position, centerOn.position)  > Instatiator1.sphereSize ) {
			Vector3 pos;
			//do {
			pos = Random.onUnitSphere * Instatiator1.sphereSize + centerOn.position;
			//} while ( Vector3.Dot(pos,centerOn.position) <= 0 );
			
			transform.position = pos; //new Vector3(0, 0, 0);
			RandomPositionAndRotation();
		}

		if ( timeElapsed >  changeDelta )  RandomRotation();

	}

	void RandomPositionAndRotation(){
		//Random.onUnitSphere
		speedZ = Random.Range(speedZMin,speedZMax);
		transform.rotation = Random.rotation;

		RandomRotation();
	}

	void RandomRotation() {
		targetQuaternion = Random.rotation;
		lastTimeChanged = Time.time;
		changeDelta = Random.Range (changeDeltaMin, changeDeltaMax);
	}

	/*
	void OnMouseOver() {
		if (Input.GetMouseButton(0)) {
			centerOn.GetComponent<FlyTo1>().SetTargetPosition(transform);
		} else if ( Input.GetMouseButton(1)) {
			Instantiate(explosion, transform.position, Quaternion.identity);
			Destroy(this.gameObject);
		}
	}
	
	void OnMouseDown() {
		
		
		
		
	}
	*/
}
