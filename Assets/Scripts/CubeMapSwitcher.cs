using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeMapSwitcher : MonoBehaviour {

    public Material[] materials;
    int currentMaterial = 0;
    Skybox skybox;
    // public float blend = 0;
    // public bool autoBlend = true;
   


    // Use this for initialization
    void Start()
    {
        skybox = GetComponent<Skybox>();
        //material = GetComponent<Skybox>().material;
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown("m"))
        {
            Switch();
            //Debug.Log(currentMaterial);

        }
        // if (autoBlend) blend = Mathf.PingPong(Time.time * 0.1f, 1.0F);
        // material.SetFloat("_Blend", blend);
    }

    public void Switch()
    {
        currentMaterial = (currentMaterial + 1) % materials.Length;
        skybox.material = materials[currentMaterial];
    }
}
