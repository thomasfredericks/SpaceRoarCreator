using UnityEngine;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine.Networking;


public class SpaceShip
{
    public GameObject main;
    public GameObject rockets;
    public AudioSource audio;
    public GameObject parts;
    public Renderer renderer;

    public SpaceShip()
    {
        main = null;
        audio = null;
        rockets = null;
        parts = null;
    }
}



public class SpaceShipConnector {

	public string name;
	public Vector3 position;
	public Vector3 normal;
	public float weight;

}


public class SpaceShipPart {

	public string name;
	public int id;
	public Mesh mesh;
	public GameObject obj;
	public List<SpaceShipConnector> connectors;
	public Vector3[] vertices;
	public int[] faces;

}


public class SpaceShipSettings
{
    public int seed;
    public int iterations;
    public float scale;
    public float uniformity;
    public float arsenal;
    public int rockets;
    public String name;

}

public class SpaceShipGeneratorScript : MonoBehaviour {

   


	private static Vector3 mirrorVector = new Vector3(-1, 1, 1);



	private List<SpaceShipPart> parts;
	private List<SpaceShipPart> hulls;
	private List<SpaceShipPart> hardpoints;
	private List<SpaceShipPart> softpoints;
	private List<SpaceShipPart> rockets;
	private List<SpaceShipPart> siderockets;

	private float uniformity = 0;

	private GameObject generatedParts;
	private List<SpaceShipConnector> openConnectors;
	private float openConnectorsWeight = 0;

    public GameObject rootObject;

    public bool debug = true;
    public Material debugMaterial;
    public GameObject debugConnectorObject;

    Material currentMaterial;


     bool combine = false;

    bool computeObjectBounds = true;

    public Vector2 scaleRange = new Vector2(1,100);
    public Vector2 iterationsRange = new Vector2(3,20);

    public bool addRockets = true;

    public GameObject[] partsMeshes;
    public GameObject[] hullsMeshes;
    public GameObject[] hardpointsMeshes;
    public GameObject[] softpointsMeshes;
    public GameObject[] rocketsMeshes;
    public GameObject[] siderocketsMeshes;

    public Material[] materials;

    public ParticleSystem debrisParticleSystem;
    public Vector2 debrisRange = new Vector2(10, 100);

    // Use this for initialization
    void Start() {

		if ( debug) Debug.Log("Init");

		parts = new List<SpaceShipPart>();
		hulls = new List<SpaceShipPart>();
		hardpoints = new List<SpaceShipPart>();
		softpoints = new List<SpaceShipPart>();
		rockets = new List<SpaceShipPart>();
		siderockets = new List<SpaceShipPart>();

		//Shader shader = Shader.Find("Custom/VertexColor");
		//this.debugMaterial = new Material(shader);

        if ( rootObject == null )
        {
            rootObject = new GameObject();
            rootObject.name = "Generated Ship Root";
        }

		generatedParts = new GameObject();
		generatedParts.name = "Generated Parts Container";

		LoadParts(partsMeshes, parts);
		LoadParts(hullsMeshes, hulls);
		LoadParts(softpointsMeshes, softpoints);
		LoadParts(hardpointsMeshes, hardpoints);
		LoadParts(rocketsMeshes, rockets);
		LoadParts(siderocketsMeshes, siderockets);

		
        if (debug) Debug.Log("Done");

	}


//	void Update() {
//
//		if (Input.GetKeyDown("g"))
//			GenerateShip();
//
//	}


	/**
	 * Loading
	 */

	private void LoadParts(GameObject[] meshesArray, List<SpaceShipPart> partsArray) {

		foreach (GameObject mesh in meshesArray)
			partsArray.Add(LoadPart(mesh));
		
	}


	SpaceShipPart LoadPart(GameObject mesh) {

		SpaceShipPart part = new SpaceShipPart();
		part.name = mesh.name;
		part.mesh = mesh.GetComponentInChildren<MeshFilter>().sharedMesh;
		part.connectors = new List<SpaceShipConnector>();

		// Extract connectors from vertex colors
		Color[] colors = part.mesh.colors;
		int[] tris = part.mesh.triangles;
		Vector3[] verts = part.mesh.vertices;
		Vector3[] normals = part.mesh.normals;

		Vector3 connectorPosition = new Vector3();
		Vector3 connectorNormal = new Vector3();

        //Debug.Log(colors);

		for (int i = 0; i < tris.Length;) {

			if (colors[tris[i]] != Color.green) {

				i += 3;
				continue;

			}

			connectorNormal.Set(0, 0, 0);
			connectorPosition.Set(0, 0, 0);
			for (int v = i + 6; i < v; ++i) {

				int vIndex = tris[i];
				connectorPosition += verts[vIndex];
				connectorNormal += normals[vIndex];

			}

			connectorPosition /= 6;
			connectorNormal /= 6;

			SpaceShipConnector c = new SpaceShipConnector();
			c.position = connectorPosition;
			c.normal = connectorNormal.normalized;
			part.connectors.Add(c);

		}

		part.obj = Instantiate(mesh);
		part.obj.SetActive(false);
		MeshRenderer renderer = part.obj.GetComponentInChildren<MeshRenderer>();
      
       
		renderer.receiveShadows = false;
		renderer.shadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.Off;

		part.obj.transform.parent = generatedParts.transform;

		return part;

	}


	


	/**
	 * Generator
	 */

	public SpaceShip GenerateShip(char[] dna) {


        string dnaString = new string(dna);

        SpaceShip se = new SpaceShip();

        int previousSeed = UnityEngine.Random.seed;
        UnityEngine.Random.seed = dnaString.GetHashCode();

        // population, population density, GINI index (equity), nuclear, CO2, weapons and manufacture. 

        float shipNormalizedLength = Mathf.Max((float)((int)dna[0] - 65) / 26.0f, 0);
        float scale = scaleRange.x + (shipNormalizedLength * (scaleRange.y - scaleRange.x));

        int seed = dnaString.GetHashCode();
        float normalizedDisp = Mathf.Max((float)((int)dna[5] - 65) / 26.0f, 0);
        int iterations = (int)(iterationsRange.x + (normalizedDisp * (iterationsRange.y - iterationsRange.x)));
        float uniformity = Mathf.Max((float)((int)dna[1] - 65) / 26.0f, 0);
        float arsenal = Mathf.Max((float)((int)dna[4] - 65) / 26.0f, 0);
        int rockets = Mathf.Max((int)dna[2] - 65, 0);
        float co2 = Mathf.Max((float)((int)dna[3] - 65) / 26.0f, 0); Mathf.Max((int)dna[3] - 65, 0);
        co2 = debrisRange.x + (co2 * (debrisRange.y - debrisRange.x));

        print("dna: "+dnaString+" seed " + seed+ ", " + "scale " + scale + ", " + "iterations " + iterations + ", " + "uniformity " + uniformity + ", " + "arsenal " + arsenal +", " + "rockets " + rockets);


        int f = (int)(UnityEngine.Random.value * (hulls.Count - 1));

		SpaceShipPart parentPart = hulls[f];
       
		if (debug) Debug.Log("parent hulls: " + f);

		GameObject mainObj = new GameObject();
		mainObj.name = "Generated Ship";
		GameObject parentObj = CreateObject(parentPart);
		parentObj.transform.SetParent(mainObj.transform);

        GameObject partsGo = new GameObject();
        partsGo.transform.parent = mainObj.transform;
        partsGo.name = "Arsenal";

        se.renderer = parentObj.GetComponent<Renderer>();


        mainObj.transform.parent = rootObject.transform;

        openConnectors = new List<SpaceShipConnector>();
		openConnectorsWeight = 0;
		foreach (SpaceShipConnector cnt in parentPart.connectors)
			addOpenConnector(cnt);

        if ( addRockets ) se.rockets = generateRockets(mainObj, rockets);

        


        
		int depth = 0;
		while (depth++ < iterations) {

			// Pick a random parent connector
			if (openConnectors.Count == 0)
				break;

			SpaceShipConnector parentConnector = pickConnector();

			// Pick a random child part
			SpaceShipPart childPart = null;
			int p = (int)(UnityEngine.Random.value * (parts.Count - 1));
        	if (debug) Debug.Log("child part: " + p);

			childPart = parts[p];

			// Pick a random child connector
			SpaceShipConnector childConnector = pickChildConnector(childPart);

            if (parentConnector == null) print("bob");

			// Create a child object and assemble the 2 parts
			GameObject childObj = CreateChildObject(parentObj, parentConnector, childPart, childConnector, true, false);
			childObj.transform.SetParent(partsGo.transform);

            //addedParts.Add(childObj);

            GameObject mirrorObj = CreateMirrorObject(parentConnector, childObj, childPart, childConnector);
			if (mirrorObj)
            {
                mirrorObj.transform.SetParent(partsGo.transform);
                //addedParts.Add(mirrorObj);
            }
           
            parentObj = childObj;
			parentPart = childPart;

		}

		generateArsenal(mainObj, arsenal, partsGo);

		openConnectors = null;


        if ( combine) Combine(mainObj);

        if (computeObjectBounds)
        {
           BoxCollider collider = ComputeObjectBounds(mainObj);
            // Rescale
            Vector3 s = collider.bounds.size;
            print("scale " + scale);
            print("bounds " + s);
            float objectLength = Mathf.Max(s.x, Mathf.Max(s.y, s.z));
            print("objectLength " + objectLength);
            float localScale = scale / objectLength;
            print("localScale " + localScale);
            mainObj.transform.localScale = new Vector3(localScale, localScale, localScale);
        }

      //  Debug.Log("collider.bounds.size "+collider.bounds.size);

      
		mainObj.transform.Rotate(new Vector3(0, 180, 0));

    
       


        currentMaterial = materials[UnityEngine.Random.Range(0, materials.Length)];
        MeshRenderer mainObjRenderer = mainObj.GetComponentInChildren<MeshRenderer>();

        if (mainObjRenderer != null)
        {
            if (debug) mainObjRenderer.sharedMaterial = this.debugMaterial;
            else mainObjRenderer.sharedMaterial = currentMaterial;
        }

        GameObject rocketsGo = se.rockets;
        if (rocketsGo) {
            
            for (int i = 0; i < rocketsGo.transform.childCount; i++)
            {
                GameObject go = rocketsGo.transform.GetChild(i).gameObject;
                MeshRenderer r = go.GetComponentInChildren<MeshRenderer>();
                if ( r != null )
                {
                    if (debug)
                    {
                        r.sharedMaterial = this.debugMaterial;
                    }   else  {
                        r.sharedMaterial = currentMaterial;
                       // r.material.color = UnityEngine.Random.ColorHSV(0, 1, 0, 1,0,1);
                    }
                        
                }
           

            }
        }

        
        if (partsGo)
        {

            for (int i = 0; i < partsGo.transform.childCount; i++)
            {
                GameObject go = partsGo.transform.GetChild(i).gameObject;
                MeshRenderer r = go.GetComponentInChildren<MeshRenderer>();
                if (r != null)
                {
                    if (debug) r.sharedMaterial = this.debugMaterial;
                    else r.sharedMaterial = currentMaterial;
                }


            }
        }

        se.main = mainObj;
        se.parts = partsGo;


        if ( debrisParticleSystem != null )
        {
            ParticleSystem.EmissionModule pe = debrisParticleSystem.emission;

           
            pe.rateOverTime = co2;


            debrisParticleSystem.Clear();
            debrisParticleSystem.Simulate(debrisParticleSystem.main.duration);
            debrisParticleSystem.Play();

            ParticleSystem.ShapeModule ps = debrisParticleSystem.shape;
            Bounds b = se.main.GetComponent<Collider>().bounds;



            ps.angle = 27;
            ps.length = b.size.z;

            Vector3 tp = debrisParticleSystem.transform.localPosition;
            tp.z = b.size.z *0.25f ;
            debrisParticleSystem.transform.localPosition = tp;
        }
            
        return se;

	}


	private void generateArsenal(GameObject mainObj, float arsenalValue, GameObject partsGo) {
	
		int pointsCount = (int)(arsenalValue * openConnectors.Count);
		for (int i = 0; i < pointsCount; ++i) {

			var partsList = UnityEngine.Random.value > 0.5 ? softpoints : hardpoints;
			var childPart = partsList[(int)(UnityEngine.Random.value * (partsList.Count - 1))];

			SpaceShipConnector parentConnector = pickConnector();
			SpaceShipConnector childConnector = childPart.connectors[(int)(UnityEngine.Random.value * (childPart.connectors.Count - 1))];

            if (parentConnector != null)
            {
             
                GameObject childObj = CreateChildObject(mainObj, parentConnector, childPart, childConnector, true, false);
                childObj.transform.SetParent(partsGo.transform);
            } else
            {
                print("oups, no connector");
            }

			

		}

	}


	private GameObject generateRockets(GameObject mainObj, float rocketValue) {

       
        

		SpaceShipConnector selectedConnector = null;
		bool mirrored = false;

		// rear rocket, centered or mirrored
		foreach (SpaceShipConnector cnt in openConnectors) {

			if (cnt.normal.z <= 0.999)
				continue;

			int mirrorCntIndex = getMirrorConnectorIndex(cnt);
			if (Mathf.Abs(cnt.position.x) < 0.0001 && mirrorCntIndex == -1)
				continue;
		
			if (selectedConnector == null || cnt.position.z < selectedConnector.position.z) {
				
				selectedConnector = cnt;
				mirrored = mirrorCntIndex != -1;
			
			}

		}

		// Side rockets
		if (selectedConnector == null) {

			float selectedZDist = 0;

			foreach (SpaceShipConnector cnt in openConnectors) {

				if (Mathf.Abs(cnt.normal.z) >= 0.0001)
					continue;

				int mirrorCntIndex = getMirrorConnectorIndex(cnt);
				if (mirrorCntIndex == -1)
					continue;

				float distFromZAxis = (cnt.position - (new Vector3(0, 0, cnt.position.z))).sqrMagnitude;
				if (selectedConnector == null || distFromZAxis > selectedZDist) {

					selectedConnector = cnt;
					selectedZDist = distFromZAxis;
					mirrored = true;

				}

			}

			if (selectedConnector == null) {

				// rear rocket, single uncentered
				foreach (SpaceShipConnector cnt in openConnectors) {

					if (cnt.normal.z <= 0.999)
						continue;

					if (selectedConnector == null || cnt.position.z < selectedConnector.position.z) {

						selectedConnector = cnt;
						mirrored = false;

					}

				}

				if (selectedConnector == null) {

                    if (debug) Debug.Log("No rocket connector found");
					return null;

				}

				if (debug)   Debug.Log("uncentered");

			}

		}

        GameObject rocketsGo = new GameObject();
        rocketsGo.transform.parent = mainObj.transform;
        rocketsGo.name = "rockets";

        SpaceShipPart rocketPart;

		if (mirrored)
			rocketPart = siderockets[(int)(UnityEngine.Random.value * (siderockets.Count - 1))];
		else
			rocketPart = rockets[(int)(UnityEngine.Random.value * (rockets.Count - 1))];

		GameObject rocketObj = CreateChildObject(mainObj, selectedConnector, rocketPart, rocketPart.connectors[0], false, true);
		rocketObj.transform.SetParent(rocketsGo.transform);

		if (mirrored) {
			
			GameObject mirrorObj = CreateMirrorObject(selectedConnector, rocketObj, rocketPart, rocketPart.connectors[0]);
			mirrorObj.transform.SetParent(rocketsGo.transform);

		}

        return rocketsGo;



    }


	int getMirrorConnectorIndex(SpaceShipConnector refCnt) {

		Vector3 mirrorPos = Vector3.Scale(refCnt.position, mirrorVector);

		int mirrorConnectorIndex = -1;

		for (int index = 0; index < openConnectors.Count; ++index) {

			SpaceShipConnector cnt = openConnectors[index];
			if (cnt != refCnt && Vector3.SqrMagnitude(cnt.position - mirrorPos) < 0.0001) {

				mirrorConnectorIndex = index;
				break;

			}

		}

		return mirrorConnectorIndex;

	}


	private void addOpenConnector(SpaceShipConnector cnt) {

		float zness = Mathf.Abs(Vector3.Dot(cnt.normal, Vector3.forward));
		cnt.weight = 1 + zness * uniformity * 100;
		openConnectorsWeight += cnt.weight;

		int index = 0;
		for (; index < openConnectors.Count; ++index) {

			if (openConnectors[index].weight < cnt.weight)
				break;
			
		}

		openConnectors.Insert(index, cnt);

	}


	private void removeOpenConnector(int index) {

		openConnectorsWeight -= openConnectors[index].weight;
		openConnectors.RemoveAt(index);

	}


	private void removeOpenConnector(SpaceShipConnector cnt) {

		openConnectorsWeight -= cnt.weight;
		openConnectors.Remove(cnt);

	}


	SpaceShipConnector pickChildConnector(SpaceShipPart childPart) {

		List<SpaceShipConnector> prefCnt = new List<SpaceShipConnector>();

		foreach (SpaceShipConnector cnt in childPart.connectors) {

			if (cnt.position.x < Mathf.Epsilon && cnt.normal.x < Mathf.Epsilon)
				prefCnt.Add(cnt);

		}

		if (prefCnt.Count > 0)
			return prefCnt[(int)(UnityEngine.Random.value * (prefCnt.Count - 1))];
		else
			return childPart.connectors[(int)(UnityEngine.Random.value * (childPart.connectors.Count - 1))];

	}


	SpaceShipConnector pickConnector() {

		int targetWeight = (int)(UnityEngine.Random.value * openConnectorsWeight);

		SpaceShipConnector connector = null;
		float weightAccum = 0;
		int index = 0;

		for (; index < openConnectors.Count; ++index) {

			connector = openConnectors[index];
			weightAccum += connector.weight;
			if (weightAccum >= targetWeight)
				break;
		
		}

		if (debug) Debug.Log("parent connector: " + index);

        
		return connector;

	}
		

	BoxCollider ComputeObjectBounds(GameObject shipObj) {

		Bounds rootBounds = new Bounds();
		bool hasBounds = false;

		foreach(var r in shipObj.GetComponentsInChildren<Renderer>()) {

			if(!hasBounds) {
				
				rootBounds.center = r.bounds.center;
				rootBounds.size = r.bounds.size;
				hasBounds = true;
			
			}
			else {
				
				rootBounds.Encapsulate(r.bounds);
			
			}

			//r.gameObject.AddComponent<BoxCollider>();

		}

		if (!hasBounds)
			return null;

		BoxCollider collider = shipObj.AddComponent<BoxCollider>();
		collider.center = shipObj.transform.InverseTransformPoint(rootBounds.center);


         if (rootBounds.size.x < 0 || rootBounds.size.y < 0 || rootBounds.size.z < 0) Debug.Log("rootBounds.size " + rootBounds.size);



        collider.size = rootBounds.size;

		return collider;

	}


	GameObject CreateChildObject(GameObject parentObj, SpaceShipConnector parentConnector, SpaceShipPart childPart, SpaceShipConnector childConnector, bool addChildConnectors, bool zRotOnly) {

		GameObject childObj = CreateObject(childPart);
//DebugConnectors(childObj, childPart, cc);
		AssembleParts(parentObj, parentConnector, childObj, childConnector, zRotOnly);

		// Add child part connectors to open connectors list
		if(addChildConnectors)
			AppendChildObjectConnectors(childObj, childPart, childConnector);

		removeOpenConnector(parentConnector);

		return childObj;

	}


	GameObject CreateMirrorObject(SpaceShipConnector parentConnector, GameObject childObj, SpaceShipPart childPart, SpaceShipConnector childConnector) {

		// Check if a mirror connector exists
		int mirrorConnectorIndex = getMirrorConnectorIndex(parentConnector);
		if (mirrorConnectorIndex == -1)
			return null;

		// Instanciate the same object and mirror its matrix
		GameObject mirrorObj = Instantiate(childObj);

		mirrorObj.transform.position = Vector3.Scale(mirrorObj.transform.position, mirrorVector);
		mirrorObj.transform.localScale = Vector3.Scale(mirrorObj.transform.localScale, mirrorVector);
		mirrorObj.transform.rotation = Quaternion.Euler(Vector3.Scale(mirrorObj.transform.rotation.eulerAngles, new Vector3(1, -1, -1)));

		ParticleSystem[] psList = mirrorObj.GetComponentsInChildren<ParticleSystem>();
		foreach (ParticleSystem ps in psList) {

			ps.transform.localScale = Vector3.Scale(ps.transform.localScale, mirrorVector);

		}

		// 
		removeOpenConnector(mirrorConnectorIndex);

		// Add mirrored child part connectors to open connectors list
		AppendChildObjectConnectors(mirrorObj, childPart, childConnector);

		return mirrorObj;

	}


	void AppendChildObjectConnectors(GameObject childObj, SpaceShipPart childPart, SpaceShipConnector usedChildConnector) {

		foreach(SpaceShipConnector cnt in childPart.connectors) {

			if (cnt == usedChildConnector)
				continue;

			SpaceShipConnector worldCnt = new SpaceShipConnector();
			worldCnt.normal = childObj.transform.TransformDirection(cnt.normal).normalized;
			worldCnt.position = childObj.transform.TransformPoint(cnt.position);
			addOpenConnector(worldCnt);

		}

	}


	void AssembleParts(GameObject parentObj, SpaceShipConnector parentConnector, GameObject childObj, SpaceShipConnector childConnector, bool zRotOnly) {

        

		Vector3 childNormal = (childObj.transform.TransformDirection(childConnector.normal)).normalized;
        if ( parentConnector == null )
        {
            print("alex");
        }
		Vector3 parentNormal = parentConnector.normal.normalized;
		Vector3 parentPos = parentConnector.position;

		// ROTATE THE CHILD AROUDN THE GLOBAL Z AXIS TO MATCH THE PARENTMOUNT
		Vector3 childZ = (new Vector3(childNormal.x, childNormal.y, 0.0f)).normalized;
		Vector3 parentZ = (new Vector3(parentNormal.x, parentNormal.y, 0.0f)).normalized;
		bool zRotation = false;

		if (childZ.magnitude > 0.0f && parentZ.magnitude > 0.0f) {

			float angZ = Vector3.Angle(childZ, parentZ);
			Vector3 cross = Vector3.Cross(childZ, parentZ);
			float sign = Mathf.Sign(cross.z);


			//			print('   angZ', math.degrees(angZ))

			if (angZ > 0.0f && angZ < 180.0f) {

				childObj.transform.localRotation *= Quaternion.AngleAxis((180f - angZ * sign), Vector3.back);
				zRotation = true;
				//				child.matrix_world = rotZ * child.matrix_world

			}
			else if (angZ == 0.0f) {

				childObj.transform.localRotation = Quaternion.AngleAxis(180f, Vector3.back);
				zRotation = true;

			}

			//childNormal = (childMount.normal * child.matrix_world).normalized()

		}

		if (!zRotOnly && zRotation == false) {

			// ROTATE THE CHILD AROUND THE GLOBAL X AXIS TO MATCH THE PARENTMOUNT
			Vector3 childX = (new Vector3(0.0f, childNormal.y, childNormal.z)).normalized;
			Vector3 parentX = (new Vector3(0.0f, parentNormal.y, parentNormal.z)).normalized;

			if (childX.magnitude > 0.0f && parentX.magnitude > 0.0f) {

				float angX = Vector3.Angle(childX, parentX);
				Vector3 cross = Vector3.Cross(childX, parentX);
				float sign = Mathf.Sign(cross.x);

				if (angX > 0.0f && angX < 180.0f) {

					childObj.transform.localRotation *= Quaternion.AngleAxis((180f - angX * sign), Vector3.left);
					childNormal = (childObj.transform.TransformDirection(childConnector.normal)).normalized;

				}

			}

		}

		Vector3 childY = (new Vector3(childNormal.x, 0.0f, childNormal.z)).normalized;
		Vector3 parentY = (new Vector3(parentNormal.x, 0.0f, parentNormal.z)).normalized;

		if (!zRotOnly && childY.magnitude > 0.0f && parentY.magnitude > 0.0f) {

			float angY = Vector3.Angle(childY, parentY);
			Vector3 cross = Vector3.Cross(childY, parentY);
			float sign = -Mathf.Sign(cross.y);

			if (angY > 0.0f && angY < 180.0f) {

				childObj.transform.localRotation *= Quaternion.AngleAxis(180f - angY * sign, Vector3.up);
				childNormal = (childObj.transform.TransformDirection(childConnector.normal)).normalized;

			}

		}

		// SET CHILD POSITION
		Vector3 childPos = childObj.transform.TransformPoint(childConnector.position);
		childObj.transform.localPosition = parentPos - childPos;

	}


	GameObject CreateObject(SpaceShipPart part) {

		GameObject obj = Instantiate(part.obj);
		obj.SetActive(true);

        return obj;

	}


	void DebugConnectors(GameObject obj, SpaceShipPart part, int selected) {

		for(int i = 0; i < part.connectors.Count; ++i) {

			SpaceShipConnector c = part.connectors[i];
			float angle = Vector3.Angle(Vector3.up, c.normal);
			Vector3 axe = Vector3.Cross(Vector3.up, c.normal);
			Quaternion q = Quaternion.AngleAxis(angle, axe);

            if ( debug ) {
                GameObject dgo = (GameObject)Instantiate(debugConnectorObject, c.position, q);
                dgo.transform.SetParent(obj.transform);
                foreach (MeshRenderer r in dgo.GetComponentsInChildren<MeshRenderer>())
                    r.material.color = i == selected ? Color.yellow : Color.gray;
            }

		}

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
            Destroy(meshFilters[i].gameObject);
            i++;
        }
            
        meshFilter = obj.transform.GetComponent<MeshFilter>() as MeshFilter;
        if (meshFilter == null) meshFilter = obj.gameObject.AddComponent<MeshFilter>();

        meshRenderer = obj.transform.GetComponent<MeshRenderer>() as MeshRenderer;
        if (meshRenderer == null) meshRenderer = obj.gameObject.AddComponent<MeshRenderer>();
        meshRenderer.sharedMaterial = meshFilters[0].gameObject.GetComponent<MeshRenderer>().material;


        //Debug.Log(meshFilter);
        meshFilter.mesh = new Mesh();

        Debug.Log("combine.Length " + combine.Length);
        Debug.Log("vertexCount " + vertexCount);

        meshFilter.mesh.CombineMeshes(combine);
        transform.gameObject.SetActive(true);
    }

}