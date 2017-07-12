using UnityEngine;
using System.Collections;

public class Ship : MonoBehaviour {

	public new string name;

	public string country;
	public int id;
	public float size;
	public float uniformity;
	public int rockets;
	public float disposal;
	public float arsenal;
	public float particles;
	public int collection;
	public Vector3 position;
	public Quaternion rotation;
	public float speed;

	public Vector3 boundCenter;
	public Vector3 rocketPosition;

    private AudioSource audioSource;

    

    void Start()
    {
        speed = Random.Range(1f, 5f);

        audioSource = GetComponentInChildren<AudioSource>();
    }

   
   

    void Update()
    {
        
        if (audioSource != null) {
            float distance = Vector3.SqrMagnitude(Camera.main.transform.position - transform.position);
            audioSource.enabled = distance < 500;
        }
        transform.Rotate(new Vector3(0, 0, speed), Space.Self);
    }

}
