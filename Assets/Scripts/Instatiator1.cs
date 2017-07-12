using UnityEngine;
using System.Collections;

public class Instatiator1 : MonoBehaviour {

	public GameObject shipPrefab; // Prefab can be anything
	public GameObject smallShipPrefab;

	public int instatiateAtStart = 0;
	public static float sphereSize = 50.0f;
	public float overlapShereRadius = 3.0f;
	public Transform centerOn;
	public GameObject audioClipsProvider;

	// Use this for initialization
	void Start () {
		for ( int i = 0 ; i < instatiateAtStart ; i++ ) {
			Vector3 pos;
			int reps = 0;
			do {
				pos = Random.insideUnitSphere * sphereSize + centerOn.position; //new Vector3(0, 0, 0);
				reps++;
			} while (  Physics.CheckSphere(pos, overlapShereRadius) && reps < 100 );

			if ( reps < 100 ) {
				Quaternion rot = Quaternion.identity;
				GameObject clone =  Instantiate(shipPrefab, pos , rot )  as GameObject;
				clone.transform.parent = transform;
				clone.GetComponent<Ship1>().centerOn = centerOn;
				clone.GetComponent<Ship1>().audioClipsProvider = audioClipsProvider;

			} else {
				print ("Too much overlap. Instatiated this many : " + i);
				break;
			}
		}

		for ( int i = 0 ; i < 3 ; i++ ) {
			Vector3 pos = Random.insideUnitSphere * sphereSize + centerOn.position;
			Quaternion rot = Quaternion.identity;
			GameObject clone =  Instantiate(smallShipPrefab, pos , rot )  as GameObject;
			clone.transform.parent = transform;
			clone.GetComponent<SmallShip1>().centerOn = centerOn;
		}

	}
	
	// Update is called once per frame
	/*
	void Update () {
	  
	}
	*/
}
