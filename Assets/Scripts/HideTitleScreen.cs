using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HideTitleScreen : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.anyKey ) // && Input.GetKey(KeyCode.Mouse0) == false
        {

            gameObject.SetActive(false);
        }
    }

    void OnGUI()
    {
        

     
    }
}
