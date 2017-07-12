using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boid {
    //fields
    public Vector3 pos, vel, acc, ali, coh, sep; //pos, velocity, and acceleration in a vector datatype
    public Vector3 velNormalized;
    public Vector3 randomVector = Vector3.zero;
    float lastTime;
    float interval;
    

    Vector3 alignement = Vector3.zero;
    int alignementCount = 0;

    Vector3 cohesion = Vector3.zero;
    int cohesionCount = 0;

    Flock flock;

    //GameObject prefab;

    //constructors
    public Boid(Flock flock , Vector3 inPos) {

        this.flock = flock;

        pos = inPos;

        vel = new Vector3(Random.Range(-1, 1), Random.Range(-1, 1), Random.Range(1, -1));
        velNormalized = Vector3.Normalize(vel);
        acc = new Vector3(0, 0, 0);
       

        //this.prefab = prefab;

        lastTime = Time.time;
        randomVector = Random.insideUnitSphere;

        interval = Random.Range(5, 10);

    }

   public void Add(Vector3 add)
    {
        acc += add;
    }

    public void AddAlignement(Vector3 add ) {
        alignementCount++;
        alignement += add;

      }

    public void AddCohesion(Vector3 add)
    {
        cohesionCount++;
        cohesion += add;

    }


    public void Update()
    {
       
        if (flock.time - lastTime  > interval)
        {
            lastTime = flock.time;
            interval = Random.Range(5, 10);
            randomVector = Random.insideUnitSphere ;
            
            
        }
        Add(randomVector * flock.randomAmount);

        // alignement
        if (alignementCount > 0)
        {
            acc = acc + (alignement / alignementCount);
        }


        // Cohesion
        if (cohesionCount > 0)
        {
            cohesion = (cohesion / cohesionCount);
            acc = acc + (cohesion - pos) * flock.cohesionAmount;
        }

        //limit 
        acc = Vector3.ClampMagnitude(acc, flock.maxSteerForce);
        
        vel += acc;

        vel = Vector3.ClampMagnitude(vel, flock.maxSpeed);

        pos += vel;

       // prefab.transform.position = pos;
       // prefab.transform.rotation = Quaternion.LookRotation(vel);

        // CLEAR

        acc = Vector3.zero;

        alignement = Vector3.zero;
        alignementCount = 0;

        cohesion = Vector3.zero;
        cohesionCount = 0;

        velNormalized = Vector3.Normalize(vel);

    }
}


public class Flock {

    public List<Boid> boids;

    int count;

    public float neighborhoodRadius;

    public float randomAmount = 0.1f;
    public float repulseAmount = 0.1f;
    public float repulseRadius = 1f;
    public float alignementAmount = 0.1f;
    public Vector3 globalAlignement = Vector3.zero;
    public float cohesionAmount = 0.1f;

    public float maxSpeed = 4; //maximum magnitude for the velocity vector
    public float maxSteerForce = .1f; //maximum magnitude of the steering vector
    public float time = 0;


    public Flock(int count)
    {

        boids = new List<Boid>();

        for (int i = 0; i < count; i++)
        {
            //GameObject gO = Instantiate(prefab);
            // gO.transform.parent = transform;
            Boid boid = new Boid(this, Random.insideUnitSphere * 4f);
            //Debug.Log(boid.pos);
            boids.Add(boid);
        }

        this.count = count;

    }

    public void Add(Vector3 pos, Vector3 vel)
    {
        Boid boid = new Boid(this,pos);
        boid.vel = vel;
        boids.Add(boid);
        count++;
    }

    public void Clear()
    {
        boids = new List<Boid>();
        count = 0;
    }


    public void Update()
    {
        float neighborhoodRadiusPow2 = neighborhoodRadius * neighborhoodRadius;
        float repulseRadiusPow2 = repulseRadius * repulseRadius;

        for (int i = 0; i < boids.Count; i++)
        {
            Boid boidI = boids[i];
            for (int j = i + 1; j < boids.Count; j++)
            {

                
                Boid boidJ = boids[j];

                Vector3 posI = boidI.pos;
                Vector3 posJ = boidJ.pos;

                Vector3 subtraction = posI - posJ;

                float d = Vector3.SqrMagnitude(subtraction);

                if (d <= repulseRadiusPow2)
                {
                    Vector3 repulse = subtraction;
                    //repulse.Normalize();

                    // SEPERATION
                    boidI.Add(repulse * repulseAmount);
                    boidJ.Add(repulse * -repulseAmount);

                }

                if (d <= neighborhoodRadiusPow2)
                {


                    // ALIGNEMENT   
                    boidI.AddAlignement(boidJ.velNormalized * alignementAmount); //boidI.AddAlignement(Vector3.Normalize(boidJ.vel) * alignementAmount);
                    boidJ.AddAlignement(boidI.velNormalized * alignementAmount);

                    // COHESION
                    boidI.AddCohesion(posJ);
                    boidJ.AddCohesion(posI);

                }


            }

            boidI.Add(globalAlignement);

            boidI.Update();
        }
    
    }

    public void Center()
    {
        Vector3 center = Vector3.zero;
        for (int i = 0; i < boids.Count; i++)
        {
            center += boids[i].pos;

        }
        center = center / boids.Count;

        for (int i = 0; i < boids.Count; i++)
        {
            boids[i].pos = boids[i].pos - center;

        }
    }

}

public class Boids3D : MonoBehaviour {

   
    public int count;
    

    public float neighborhoodRadius;

    public float randomAmount = 0.1f;
    public float repulseAmount = 0.1f;
    public float repulseRadius = 1f;
    public float alignementAmount = 0.1f;
    public Vector3 globalAlignement = Vector3.zero;
    public float cohesionAmount = 0.1f;

   
   public float maxSpeed = 4; //maximum magnitude for the velocity vector
    public float maxSteerForce = .1f; //maximum magnitude of the steering vector

    public GameObject prefab;
    private List<GameObject> gOList;


    Flock flock;

    public bool center = false;


    // Use this for initialization
    void Start() {
        flock = new Flock(count);
        gOList = new List<GameObject>();

        for (int i = 0; i < count; i++)
        {
            GameObject gO = Instantiate(prefab);
            gOList.Add(gO);

            // gO.transform.parent = transform;
            //boids.Add(new Boid(this, Random.insideUnitSphere * 4f));
        }

    }

    // Update is called once per frame
    void Update()
    {

        flock.neighborhoodRadius = neighborhoodRadius;

        flock.randomAmount = randomAmount;
        flock.repulseAmount = repulseAmount;
        flock.repulseRadius = repulseRadius;
        flock.alignementAmount = alignementAmount;
        flock.globalAlignement = globalAlignement;
        flock.cohesionAmount = cohesionAmount;


        flock.maxSpeed = maxSpeed; //maximum magnitude for the velocity vector
        flock.maxSteerForce = maxSteerForce; //maximum magnitude of the steering vector


        flock.Update();

       if (center ) flock.Center();//


       for ( int i =0; i < count; i++ )
        {
            gOList[i].transform.position = flock.boids[i].pos;
            gOList[i].transform.rotation = Quaternion.LookRotation(flock.boids[i].vel) ;

        }

    }


  
        
}



 
