using UnityEngine;
using System.Collections;

public class Instatiator2 : MonoBehaviour {

	public GameObject shipPrefab; // Prefab can be anything
	public GameObject smallShipPrefab;

	public int instatiateAtStart = 0;
	public float sphereSize = 100.0f;
	public float overlapShereRadius = 3.0f;
	public Transform centerOn;
	public GameObject audioClipsProvider;

	public float shipSpeedAverage = 0.5f;
	public float shipSpeedRange = 0.25f;

	GameObject[] shipArray;
	int shipCount;
	float[] shipSpeeds;

	// Use this for initialization
	void Start () {

		shipArray = new GameObject[instatiateAtStart];
		shipSpeeds = new float[instatiateAtStart];

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

				shipArray[shipCount] = clone;
				shipSpeeds[shipCount] = Random.Range(-shipSpeedRange,shipSpeedRange) + shipSpeedAverage;

				shipCount++;

				//clone.GetComponent<Ship2>().centerOn = centerOn;
				clone.GetComponent<ShipAudio>().audioClipsProvider = audioClipsProvider;

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

	void Update () {
	  if ( Input.GetMouseButton(0) ) {

			Camera.main.SendMessage("SetTargetPosition", shipArray[Mathf.FloorToInt(Random.Range(0,shipCount))].transform);

		}

		for ( int i =0; i < shipCount; i++ ) {
			Transform shipTransfrom = shipArray[i].transform;
			shipTransfrom.position = shipTransfrom.position + new Vector3(0,0,shipSpeeds[i]*Time.deltaTime);
			if ( Vector3.Distance(shipTransfrom.position, centerOn.position)  > sphereSize ) {
				Vector3 pos;
				//do {
				pos = Random.onUnitSphere * sphereSize + centerOn.position;
				//} while ( Vector3.Dot(pos,centerOn.position) <= 0 );
				
				shipTransfrom.position = pos; //new Vector3(0, 0, 0);
			}
		}



	}

}
