using UnityEngine;
using System;


public class TofCameraController : MonoBehaviour
{

    Fade fade;

    public Camera targetCamera;
	
	[Range(0,2)]
	public int mouseButton;

    public float RayCastMaxDistance = 100f;

    public bool autoOrbitDistance = false;
    public float orbitDistance = 30f;
    float initialOrbitDistance = 30f;



    public float mouseSensitivity = 3f;
    public float keySensitivity = 0.25f;



	//public Vector3 inputSpeed = new Vector3 (30.0f, 15, 15);
	//public float inputSmooth = 5;
	
	public enum Mode
	{
		Orbit,
		Pan}
	;
	public Mode mode = Mode.Pan;


	public Transform orbitTarget;
	public bool orbitTargetLock = true;
	
	
	//private float mouseXDelta;
	//private float mouseYDelta;
	
	//private float distance = 5.0f;
	
	/*
	public float yMinLimit = -20f;
	public float yMaxLimit = 80f;
	
	public float distanceMin = .5f;
	public float distanceMax = 100f;
	*/
	//private Vector3 currentInput = Vector3.zero;
	
	//private float targetTime;
	private Vector3 planeAlignedForward;
	//private Vector3 planeAlignedR;
	
	
	public bool planeLockPanMove = false;
	//private float heightOffset;


	bool going;
	public float goTime = 1;
	public float goMaxSpeed = 10f;
	public float goParabolicHeight = 0f;

	//Vector3 position;
	//Vector3 goingStartPosition;
	//Quaternion goingStartRotation;
	///Vector3 goingTargetPosition;
	//Quaternion goingTargetRotation;
	//float goingStartTime;
	//float goingDistance;
	//public float goDistanceStop = 0.1f;


	Vector3 velocity;


	//float HALFPI = Mathf.PI * 0.5f;


    //private Vector3 targetPosition;
    //private Quaternion targetQuaternion;

    private GameObject target;

    [System.Serializable]
    public class Noise : System.Object
    {
        public Vector3 amount = new Vector3(0f,0f,0f);
        public Vector3 speed = new Vector3(0.01f, 0.01f, 0.01f);
        Vector3 time;
        Vector3 value;

        Noise()
        {
            System.Random random = new System.Random();
            time = new Vector3((float)random.NextDouble(), (float)random.NextDouble(), (float)random.NextDouble());
        }

        public Vector3 next()
        {
           time += speed;
            value.x = (Mathf.PerlinNoise(0, time.x) - 0.5f) * amount.x;
            value.y = (Mathf.PerlinNoise(0, time.y) - 0.5f) * amount.y;
            value.z = (Mathf.PerlinNoise(0, time.z) - 0.5f) * amount.z;
         
            return value;
        }

        public Vector3 get()
        {
            return value;
        }

        

    }

    public Noise rotationShake;
    public Noise positionShake;

    public float rotationTime = 2.5f;


    // Use this for initialization
    void Start () {

        initialOrbitDistance = orbitDistance;

        fade = new Fade();

        if (targetCamera == null) targetCamera = Camera.main;

        if (target == null) target = new GameObject();
        target.name = "CameraControllerTarget";
        target.transform.position = targetCamera.transform.position;
        target.transform.rotation = targetCamera.transform.rotation;




        //Vector3 angles = transform.eulerAngles;
        //targetInput = Vector3.zero;




        // Make the rigid body not change rotation
        /*
        if (rigidbody)
            rigidbody.freezeRotation = true;
*/

        //if ( target != null ) distance = Vector3.Distance(transform.position, orbitTarget.position);
    }


	

	

    void LateUpdate()
    {

      

      



            if (Input.GetMouseButtonDown(mouseButton) && GUIUtility.hotControl == 0)
            {

                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, RayCastMaxDistance))
                {
                    if (hit.collider.transform != orbitTarget)
                    {
                        //targetTime = 0;

                    }

                SetOrbitTarget(hit.collider.transform);

                }
                //currentInput = Vector3.zero;
            }
           


            //currentInput.x = Mathf.Lerp(currentInput.x, targetInput.x, Time.deltaTime * inputSmooth);
            //currentInput.y = Mathf.Lerp(currentInput.y, targetInput.y, Time.deltaTime * inputSmooth);
            //currentInput.z = Mathf.Lerp(currentInput.z, targetInput.z, Time.deltaTime * inputSmooth);


            // BUTTON HELD DOWN
            if (Input.GetMouseButton(mouseButton) && GUIUtility.hotControl == 0)
            {
                if (orbitTarget != null )
                {

                    Orbit(Input.GetAxis("Mouse X")*mouseSensitivity, -Input.GetAxis("Mouse Y")*mouseSensitivity, Input.GetAxis("Vertical")*keySensitivity);
                   
                }
                else
                {



                    Fly(Input.GetAxis("Mouse X") * mouseSensitivity, -Input.GetAxis("Mouse Y") * mouseSensitivity, Input.GetAxis("Horizontal") * keySensitivity, Input.GetAxis("Vertical") * keySensitivity, Input.GetAxis("Mouse ScrollWheel"));



                }
           // BUTTTON NOT HELD DOWN
            } else {

                if ( orbitTargetLock == false )  orbitTarget = null;
                Fly(0,0,Input.GetAxis("Horizontal") * keySensitivity, Input.GetAxis("Vertical") * keySensitivity, Input.GetAxis("Mouse ScrollWheel"));


            }
                 

            targetCamera.transform.position = Vector3.SmoothDamp(targetCamera.transform.position, target.transform.position, ref velocity, goTime, goMaxSpeed);

        targetCamera.transform.Translate(positionShake.next(), Space.Self);

            targetCamera.transform.rotation = Quaternion.Slerp(targetCamera.transform.rotation, target.transform.rotation, Time.deltaTime * rotationTime);

        

        targetCamera.transform.Rotate(rotationShake.next(), Space.Self);



    }

    public void SetOrbitTargetWithInitialDistance(Transform newTarget)
    {
        orbitTarget = newTarget;
        Vector3 heading = target.transform.position - orbitTarget.position;
        heading.Normalize();
        

        heading = heading * initialOrbitDistance;

        target.transform.position = orbitTarget.position + heading;
        Orbit(0, 0, 0);
    }
 

    public void SetOrbitTarget(Transform newTarget)
    {
        orbitTarget = newTarget;
        Vector3 heading = target.transform.position - orbitTarget.position;
        heading.Normalize();
        if ( autoOrbitDistance )
        {
            Renderer renderer = newTarget.GetComponent<Renderer>();
            if ( renderer != null )
            {
                Vector3 extents = renderer.bounds.extents;
                orbitDistance = Mathf.Max(extents.x, Mathf.Max(extents.y, extents.z)) * 2;
            } else
            {
                Collider collider = newTarget.GetComponent<Collider>();
                Vector3 extents = collider.bounds.extents;
                orbitDistance = Mathf.Max(extents.x, Mathf.Max(extents.y, extents.z)) * 2;
            }
           
        } 

        heading = heading * orbitDistance;

        target.transform.position = orbitTarget.position + heading;
        Orbit(0, 0, 0);
    }

    public void Set(Vector3 camPosition, Vector3 targetPosition)
    {
        targetCamera.transform.position = camPosition;
        //targetCamera.transform.rotation = rotation;
        target.transform.position = targetPosition;
        target.transform.rotation = Quaternion.LookRotation(targetPosition - camPosition);
        targetCamera.transform.rotation = target.transform.rotation;

    }

    void Orbit(float xRotation, float yRotation, float vertical)
    {
        //DISABLED HEIGHT OFFSET MODIFICATION: //heightOffset += targetInput.z;
       // Vector3 heightOffsetVector = new Vector3(0, heightOffset, 0);

        // MOVE TOWARDS/AWAY TARGET
        Vector3 towards = orbitTarget.position - target.transform.position ;
        towards.Normalize();
        target.transform.Translate(towards * vertical, Space.World); //target.transform.Translate(Vector3.forward * Input.GetAxis("Vertical") * inputSpeed.z * 0.5f, targetCamera.transform);

        // USING TARGET.TRANSFOM AS THE RELATIVE POINT INSTEAD OF THE CAMERA (NOT SURE THIS IS RIGHT)
        target.transform.position = RotateAroundPoint(target.transform.position, orbitTarget.transform.position , Quaternion.Euler(target.transform.TransformDirection(yRotation, xRotation, 0)));
        //target.transform.position = RotateAroundPoint(target.transform.position, orbitTarget.transform.position + heightOffsetVector, Quaternion.Euler(targetCamera.transform.TransformDirection(currentInput.y, currentInput.x, 0)));

        // USING TARGET.TRANSFOM AS THE RELATIVE POINT INSTEAD OF THE CAMERA (NOT SURE THIS IS RIGHT)
        target.transform.rotation = Quaternion.LookRotation(orbitTarget.transform.position - target.transform.position );//target.transform.rotation = Quaternion.LookRotation(orbitTarget.transform.position - targetCamera.transform.position + heightOffsetVector);

    }

    void Fly(float xRotation, float yRotation, float horizontal, float vertical, float upward)
    {
        // we need some axis derived from targetCamera but aligned with floor plane

        Vector3 forward = target.transform.TransformDirection(Vector3.forward);
        forward.y = 0f;
        forward = forward.normalized;
        Vector3 right = new Vector3(forward.z, 0.0f, -forward.x);

      
            Vector3 rot = (xRotation * Vector3.up + yRotation * right);
            target.transform.Rotate(rot, Space.World);
        
        

        if (planeLockPanMove)
        {
            Vector3 trans = horizontal * right + vertical * forward + upward * Vector3.up;
            target.transform.Translate(trans, Space.World);
        }
        else
        {

            Vector3 trans = horizontal * targetCamera.transform.right + vertical * targetCamera.transform.forward + upward * targetCamera.transform.up;
            target.transform.Translate(trans, Space.World);

        }
    }
	
	public static Vector3 RotateAroundPoint (Vector3 point, Vector3 pivot, Quaternion angle)
	{
		Vector3 finalPos = point - pivot;
		//Center the point around the origin
		finalPos = angle * finalPos;
		//Rotate the point.
		
		finalPos += pivot;
		//Move the point back to its original offset.
		
		return finalPos;
	}
	
	public static float ClampAngle (float angle, float min, float max)
	{
		if (angle < -360F)
			angle += 360F;
		if (angle > 360F)
			angle -= 360F;
		return Mathf.Clamp (angle, min, max);
	}


    public void OnGUI()
    {
        fade.OnGUI();
    }

    public void FadeTo(Color newScreenOverlayColor, float fadeDuration)
    {
        fade.StartFade(newScreenOverlayColor, fadeDuration, null);
    }

    public void FadeToThen(Color newScreenOverlayColor, float fadeDuration, Action onFadeDoneAction)
    {
        fade.StartFade(newScreenOverlayColor, fadeDuration, onFadeDoneAction);
    }

    public void UnFade(float fadeDuration)
    {
        fade.UnFade(fadeDuration);  
    }


    ///// CAMERA FADE
    public class Fade
    {
        private GUIStyle m_BackgroundStyle = new GUIStyle();        // Style for background tiling
        private Texture2D m_FadeTexture;                // 1x1 pixel texture used for fading
        private Color m_CurrentScreenOverlayColor = new Color(0, 0, 0, 0);  // default starting color: black and fully transparrent
        private Color m_TargetScreenOverlayColor = new Color(0, 0, 0, 0);   // default target color: black and fully transparrent
        private Color fadeStartColor = new Color(0, 0, 0, 0);
        //private Color m_DeltaColor = new Color(0, 0, 0, 0);     // the delta-color is basically the "speed / second" at which the current color should change
        private int m_FadeGUIDepth = -1000;             // make sure this texture is drawn on top of everything
        public Action m_OnFadeFinish = null;
        float fadeDuration;
        float fadeStartTime;


        // initialize the texture, background-style and initial color:
        public Fade()
        {
            m_FadeTexture = new Texture2D(1, 1);
            m_BackgroundStyle.normal.background = m_FadeTexture;
            SetScreenOverlayColor(m_CurrentScreenOverlayColor);

            // TEMP:
            // usage: use "SetScreenOverlayColor" to set the initial color, then use "StartFade" to set the desired color & fade duration and start the fade
            //SetScreenOverlayColor(new Color(0,0,0,1));
            //StartFade(new Color(1,0,0,1), 5);
            
        }


        // draw the texture and perform the fade:
        public void OnGUI()
        {
         

                float relativeTime;
                if ( fadeDuration <= 0 )
                {
                    relativeTime = 1;
                } else
                {
                    relativeTime = Mathf.Min( (Time.time - fadeStartTime) / fadeDuration,1f);
                }
           
               

                SetScreenOverlayColor(Color.Lerp(fadeStartColor, m_TargetScreenOverlayColor, relativeTime));

                

                /*
                // if the difference between the current alpha and the desired alpha is smaller than delta-alpha * deltaTime, then we're pretty much done fading:
                if (Mathf.Abs(m_CurrentScreenOverlayColor.a - m_TargetScreenOverlayColor.a) < Mathf.Abs(m_DeltaColor.a) * Time.deltaTime)
                {
                    m_CurrentScreenOverlayColor = m_TargetScreenOverlayColor;
                    SetScreenOverlayColor(m_CurrentScreenOverlayColor);
                    m_DeltaColor = new Color(0, 0, 0, 0);
                }
                else
                {
                    // fade!
                    SetScreenOverlayColor(m_CurrentScreenOverlayColor + m_DeltaColor * Time.deltaTime);
                }
                */


            

            // only draw the texture when the alpha value is greater than 0:
            if (m_CurrentScreenOverlayColor.a > 0)
            {
                GUI.depth = m_FadeGUIDepth;
                GUI.Label(new Rect(-10, -10, Screen.width + 10, Screen.height + 10), m_FadeTexture, m_BackgroundStyle);
            }
        }


        // instantly set the current color of the screen-texture to "newScreenOverlayColor"
        // can be usefull if you want to start a scene fully black and then fade to opague
        public void SetScreenOverlayColor(Color newScreenOverlayColor)
        {
            m_CurrentScreenOverlayColor = newScreenOverlayColor;
            m_FadeTexture.SetPixel(0, 0, m_CurrentScreenOverlayColor);
            m_FadeTexture.Apply();
        }


        // initiate a fade from the current screen color (set using "SetScreenOverlayColor") towards "newScreenOverlayColor" taking "fadeDuration" seconds
        public void StartFade(Color newScreenOverlayColor, float fadeDuration, Action onFadeFinish)
        {
            m_OnFadeFinish = onFadeFinish;

            this.fadeDuration = fadeDuration;

            m_TargetScreenOverlayColor = newScreenOverlayColor;
            fadeStartColor = m_CurrentScreenOverlayColor;

            fadeStartTime = Time.time;
             
            /*
            if (fadeDuration <= 0.0f)       // can't have a fade last -2455.05 seconds!
            {
                SetScreenOverlayColor(newScreenOverlayColor);
            }
            else                    // initiate the fade: set the target-color and the delta-color
            {
                m_TargetScreenOverlayColor = newScreenOverlayColor;
                m_DeltaColor = (m_TargetScreenOverlayColor - m_CurrentScreenOverlayColor) / fadeDuration;
            }
            */
        }

        public void UnFade(float fadeDuration)
        {
            this.fadeDuration = fadeDuration;

            if ((Time.time - fadeStartTime) < fadeDuration) fadeDuration = (Time.time - fadeStartTime);
            StartFade(new Color(0, 0, 0, 0), fadeDuration, null);
        }



    }


}