using UnityEngine;
using System.Collections;




public class Ship2 : MonoBehaviour {

	//private float speedZ = 0.5f;
	public float speedZMax = 0.75f;
	public float speedZMin = 0.25f;
	internal Transform centerOn;
	internal GameObject audioClipsProvider;

	public GameObject explosion;




	// Use this for initialization
	void Start () {

		//audio.clip = audioClipsProvider.GetComponent<AudioClips1>().Get(); 
		//audio.Play();
		//speedZ = Random.Range(speedZMin,speedZMax);

	}
	
	// Update is called once per frame
	void Update () {
		/*
		transform.position = transform.position + new Vector3(0,0,speedZ*Time.deltaTime);
		if ( Vector3.Distance(transform.position, centerOn.position)  > Instatiator2.sphereSize ) {
			Vector3 pos;
			//do {
				pos = Random.onUnitSphere * Instatiator1.sphereSize + centerOn.position;
			//} while ( Vector3.Dot(pos,centerOn.position) <= 0 );

			transform.position = pos; //new Vector3(0, 0, 0);
		}
*/
		/*
		 * Implement the following code instead of the wrong after so that sounds behind do not play:
		 * 
//         Vector3 forward = transform.TransformDirection(Vector3.forward);
//            Vector3 toOther = other.position - transform.position;
//            if (Vector3.Dot(forward, toOther) < 0)
//                print("The other transform is behind me!");
//            
//        }
        

		if ( Vector3.Dot(transform.position,centerOn.position ) < 0 ) {
			audio.mute = true;
		} else {
			audio.mute = false;
		}
*/

	
	}

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
}
