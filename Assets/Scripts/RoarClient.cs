using UnityEngine;
using System.Collections;
using UnityEngine.Networking;
using System.Collections.Generic;
using System;




public class RoarClient : MonoBehaviour
{

    [Serializable]
    public class SpaceRoarMessage
    {

        public int id;
        public string message;
        public SpaceRoarMessageData data;

    }


    [Serializable]
    public class SpaceRoarMessageData
    {

        public int ships_count;
        public SpaceRoarMessageShip[] ships;

    }


    [Serializable]
    public class SpaceRoarMessageShip
    {

        // Position
        public float X;
        public float Y;
        public float Z;

        // Rotation
        public float H; // Yaw
        public float W; // Pitch

        public float V; // Speed

        public string PPP; // Country code
        public int no; // Ship number
        public string T; // Size
        public string U; // Uniformity
        public string F;
        public string D;
        public string A;
        public string P;
        public string C;

        public float c;
        public float p;

    }

    public string server;

    

    private List<GameObject> ships = new List<GameObject>();
    private SpaceShipGeneratorScript generator;
    private int last_event_id = -1;


    // Use this for initialization
    void Start()
    {

        generator = this.gameObject.GetComponent<SpaceShipGeneratorScript>();
        //StartCoroutine(PollEvent());

    }
    /*
    void GetEvent()
    {
        StartCoroutine(PollEvent());
    }
    */

    public IEnumerator PollEvent()
    {

        string uri = this.server + "/api/event/poll";
        Debug.Log("Polling " + uri);

       

            UnityWebRequest req = UnityWebRequest.Get(uri);
            yield return req.Send();

        if (req.isError)
        {

            Debug.Log(req.error);


        } else {

            SpaceRoarMessage msg = null;
            Debug.Log(req.downloadHandler.text);

            try
            {

                msg = JsonUtility.FromJson<SpaceRoarMessage>(req.downloadHandler.text);

            }
            catch (Exception e)
            {

                Debug.LogException(e);
                Debug.Log("Invalid JSON response");

            }

            if (msg != null && msg.id != last_event_id && msg.data.ships != null)
            {


                ParseEvent(msg);


            } else {
                Debug.Log("Missing Data");
            }

            

        }

           

            

    }


    void ParseEvent(SpaceRoarMessage msg)
    {
        Debug.Log("Parsing Event");

        clearShips();
        last_event_id = msg.id;

        // Average
        float avgSpeed = 0;
        Vector3 avgPos = new Vector3();

        foreach (SpaceRoarMessageShip ship in msg.data.ships)
        {

            avgPos.x += ship.X;
            avgPos.y += ship.Y;
            avgPos.z += ship.Z;

            avgSpeed += ship.V;

        }

        int shipCount = msg.data.ships_count;
        avgPos /= shipCount;
        avgSpeed /= shipCount;

        foreach (SpaceRoarMessageShip ship in msg.data.ships)
        {
            /*
            GameObject shipEntity = generator.GenerateShip(false);
            shipEntity.transform.position = new Vector3(ship.X, ship.Y, ship.Z) - avgPos;
            ships.Add(shipEntity);
            */

        }
    }

    void clearShips()
    {

        foreach (GameObject ship in ships)
            Destroy(ship);

        ships.Clear();

    }

}
