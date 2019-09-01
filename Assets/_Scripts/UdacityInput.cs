using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class UdacityInput : MonoBehaviour {


   // public SteamVR_TrackedObject trackedObject;
   // public SteamVR_Controller.Device device;

    // Teleporter Variable
    private LineRenderer laser;
    public GameObject teleportAimerObject;
    public Vector3 teleportLocation; //public Vector3 offset;
    public GameObject player;
    public LayerMask laserMask;
    public SteamVR_Action_Single squeezeAction;
    //public SteamVR_Action_Boolean m_BooleanAction;

    public static float yNudgeAmount = 1f; // specific to teleportAimerObject height
    private static readonly Vector3 yNudgeVector = new Vector3(0f, yNudgeAmount, 0f);


    //Dash Variable
    public float dashSpeed = 20f;
    public bool isDashing;
    private float lerpTime;
    private Vector3 dashStartPosition;


    //Walking Variable
    public Transform playerCam;
    public float moveSpeed = 4f;
    private Vector3 movementDirection;


    // Use this for initialization
    void Start()
    {
      //  trackedObject = GetComponent<SteamVR_TrackedObject>();
        laser = GetComponentInChildren<LineRenderer>();
    }

    void setLaserStart(Vector3 startPos)
    {
        laser.SetPosition(0, startPos); //Set a start point of LineRenderer to position of our controller
    }

    void setLaserEnd(Vector3 endPos)
    {
        laser.SetPosition(1, endPos); //Set the end of the laser (endpoint of lineRenderer) to the hit point (location our ray hit).
    }

    // Update is called once per frame
    void Update()
    {


        // device = SteamVR_Controller.Input((int)trackedObject.index);
        
        //To Walk
        if (SteamVR_Actions.default_GrabPinch.GetStateDown(SteamVR_Input_Sources.Any))
        {
            movementDirection = playerCam.transform.forward; //To move to where you looking, so we take the head direction
            movementDirection = new Vector3(movementDirection.x, 0, movementDirection.y); // We assume the floor always at y=0
            movementDirection *=  moveSpeed * Time.deltaTime;
            player.transform.position += movementDirection; //To apply above change in position every frame
        }

        if (isDashing){
            lerpTime += Time.deltaTime * dashSpeed; //increasing lerpTime by the value dashspeed each fram
            //dashStartPosition: Current position
            //teleportLocation: Distanation position
            //lerpTime: progress precent (between 0 and 1)
            player.transform.position = Vector3.Lerp(dashStartPosition, teleportLocation, lerpTime);
            if(lerpTime>= 1) //The player has reached their intended location
            {
                isDashing = false;
                lerpTime = 0;

            }
        }
        else
        {
        //  if (device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
        if (SteamVR_Actions.default_GrabPinch.GetStateDown(SteamVR_Input_Sources.Any))
            {
                laser.gameObject.SetActive(true);
                teleportAimerObject.SetActive(true);

                setLaserStart(gameObject.transform.position);

                //To determin the laser's end point & Teleport location ==> Use RayCast
                RaycastHit hit;


                //The range of ray is 15 meters
                //Output is hit object, so we can read information from ray
                //laserMask is layer to determin which layers it can collide with (laserMask is the layer that is detected by ray)
                /*This if return true if there is another object our ray can collide with (if the laser hits something)
                  withen 15 meters of our controller's forward direction */
                if (Physics.Raycast(transform.position, transform.forward, out hit, 15, laserMask))
                {
                    teleportLocation = hit.point; //This save where the laser hit
                    print("In the IF of Hit");
                }
                else
                {

                    teleportLocation = transform.position + 15 * transform.forward;
                    print("In the Else");


                    RaycastHit groundRay;
                    //Go to ground (down) 17 meters [Paycast], after 15 meters forward straight  direction      
                    if (Physics.Raycast(teleportLocation, -Vector3.up, out groundRay, 17, laserMask))
                    {
                        teleportLocation.y = groundRay.point.y; //If we hit the ground, teleport there
                        print("In the Else, in the If of groundRay");
                    }
                }
                setLaserEnd(teleportLocation);
                // aimer
                /*Below sentence: Move cylinder to the hit position (raycast hit location)
                 if 15 meters ray hits nothing, then move our indicator 50 meters forward in relation to the controller 
                 and set the indicator on the ground and to detect where the ground is, we need to cast a second ray straight down
                 from our first ray's max. point. 
                 This allow us to have multiaple different height levels within our game, so becaus of this downwards ray cast, we
                 can detect where the ground is even if the initial ray is pointed upwards. 
                 */
                //(+ 1f) ==> to move cylinder up (add hight nudge factor in order the object doen't hang through the floor)    
                // teleportAimerObject.transform.position = new Vector3(teleportLocation.x, teleportLocation.y + 1f, teleportLocation.z) 
                teleportAimerObject.transform.position = teleportLocation + yNudgeVector;
            }

            //if (device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            //if (m_BooleanAction[SteamVR_Input_Sources.Any].stateDown)
            if (SteamVR_Actions.default_GrabPinch.GetStateUp(SteamVR_Input_Sources.Any))
            {
                laser.gameObject.SetActive(false);
                teleportAimerObject.SetActive(false);
                // player.transform.position = teleportLocation; //Move player when trigger is released
                dashStartPosition = player.transform.position; // Don't move instantly, instead of that animate to distenation

                isDashing = true;

            }
        }

       
    }
}