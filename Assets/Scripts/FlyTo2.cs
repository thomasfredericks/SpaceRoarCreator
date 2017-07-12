using UnityEngine;
using System.Collections;

public class FlyTo2 : MonoBehaviour {


	public float reachTargetMs = 1000f;
	private float reachTargetMsCurrent = 0;
	private Vector3 targetPosition;
	private Vector3 originalPosition;
	private Transform targetTransform;
	public bool perpendicularView = false;
	private Vector3 perpendicularViewPosition;

	// Use this for initialization
	void Start () {
		reachTargetMsCurrent = reachTargetMs;
		originalPosition = transform.position;
		targetPosition = originalPosition;
	}

	public void SetTargetPosition(Transform t) {

		reachTargetMsCurrent = 0;
		originalPosition = transform.position;
		if( t.position.x - transform.position.x  < 0 ) {
			targetPosition = t.position + new Vector3(1.5f,0,3);

		} else {
			targetPosition = t.position + new Vector3(-1.5f,0,3);

		}
		perpendicularViewPosition = t.position + new Vector3(0,0,3);
		targetTransform = t;

	}

	// Update is called once per frame
	void Update () {

	

		if ( reachTargetMsCurrent <= reachTargetMs ) {
			reachTargetMsCurrent += Time.deltaTime*1000;
			transform.position = Vector3.Lerp(originalPosition, targetPosition, reachTargetMsCurrent/reachTargetMs);

		}

		if ( targetTransform != null ) {

			Vector3 relativePos = (targetTransform.position+perpendicularViewPosition)*0.5f - transform.position;
			Quaternion targetRotation = Quaternion.LookRotation(relativePos);
			transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5	); 
			/*
			if ( perpendicularView == false ) {
				//transform.LookAt(targetTransform);
				Vector3 relativePos = targetTransform.position - transform.position;
				Quaternion targetRotation = Quaternion.LookRotation(relativePos);
				//transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, Time.deltaTime * 100);
				transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5	); 
			} else {
				Vector3 relativePos = perpendicularViewPosition - transform.position;
				Quaternion targetRotation = Quaternion.LookRotation(relativePos);
				transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 5	); 
			}
			*/
		}
			
	
	}
}
