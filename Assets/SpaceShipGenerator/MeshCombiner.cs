using UnityEngine;
using System.Collections;

public class MeshCombiner : MonoBehaviour {

   

    // Use this for initialization
    void Start () {

        Combine(gameObject);

    }
	
	// Update is called once per frame
	void Update () {
	
	}

    void Combine(GameObject obj)
    {

        MeshFilter[] meshFilters; 
        MeshFilter meshFilter;
        MeshRenderer meshRenderer;

      
            // find all the mesh filters
            Component[] comps = obj.GetComponentsInChildren(typeof(MeshFilter));
            meshFilters = new MeshFilter[comps.Length];

            int mfi = 0;
            foreach (Component comp in comps)
                meshFilters[mfi++] = (MeshFilter)comp;
        

        CombineInstance[] combine = new CombineInstance[meshFilters.Length];
        int i = 0;
        int vertexCount = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            vertexCount += combine[i].mesh.vertices.Length;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);
            i++;
        }

        meshFilter = obj.transform.GetComponent<MeshFilter>() as MeshFilter;
        if (meshFilter == null) meshFilter = obj.gameObject.AddComponent<MeshFilter>();

        meshRenderer = obj.transform.GetComponent<MeshRenderer>() as MeshRenderer;
        if (meshRenderer == null) meshRenderer = obj.gameObject.AddComponent<MeshRenderer>();
        meshRenderer.material = meshFilters[0].gameObject.GetComponent<MeshRenderer>().material;


        //Debug.Log(meshFilter);
        meshFilter.mesh = new Mesh();

        Debug.Log("combine.Length " + combine.Length);
        Debug.Log("vertexCount " + vertexCount);

        meshFilter.mesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);
    }
}
