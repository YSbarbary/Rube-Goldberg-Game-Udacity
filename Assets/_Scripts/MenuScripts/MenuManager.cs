using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class MenuManager : MonoBehaviour {

    /* For Menu we use these below Scripts:
     * 1. RMF_RadialMenu
     * 2. RMF_RadialMenuElement  
     * 3. HandInteraction (grab tools and delete them)
     * 4. MenuManager (Active/Deactive Menu and its Hint) */


    //Variables
    public SteamVR_Action_Boolean m_MenuAction = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Menu");
    public GameObject menuObject;
    public static MenuManager instace; //Do MenuManager as a Singlton class
    private Player player = null;
    private bool firstTimeOpenMenu ;
    public bool switchHint = false;
    public bool grabItemsHint = false;
    public RMF_RadialMenu radialMenuObject;
    
    
    /*
    //Swipe  //***********************For Udacity Menu***********************************
    public float swipeSum;
    public float touchLast;
    public float touchCurrent;
    public float distance;
    public bool hasSwipedLeft;
    public bool hasSwipedRight;
    public ObjectMenuManager objectMenuManager;
    private bool oculus;
    public SteamVR_Action_Boolean m_IsTouch;
    */

    // Use this for initialization
    void Start () {
        instace = this; //Do MenuManager as a Singlton class
        menuObject.SetActive(!menuObject.activeSelf); //False !menuObject.activeSelf


        //---------------For Menu Hint -----------------------------
        firstTimeOpenMenu = true;
        player = Player.instance;
        
        if (player == null)
        {
            Debug.LogError("<b>[SteamVR Interaction]</b> Teleport: No Player instance found in map.");
            Destroy(this.gameObject);
            return;
        }

        Invoke("ShowHints", 5.0f);


        /*
        //**************************************************************For Udacity Menu***********************************
        touchLast = 0;
        //  rb = GetComponent<Rigidbody>();
        if (UnityEngine.XR.XRDevice.model.Contains("Oculus"))
        {
            oculus = true;
        }
        else
        {
            oculus = false;
        }
*/
    }

    // Update is called once per frame
    void Update()
    {
        //Deactive & Active Menu Button

        if (m_MenuAction[SteamVR_Input_Sources.RightHand].stateDown)
        {
           // Debug.Log("menu button is pressed down!");

            if (firstTimeOpenMenu)
            {
                // HideHint();
                // CancelMenuHint();
                firstTimeOpenMenu = false;
                DisableHints();

                switchHint = true;
                grabItemsHint = true;

                Invoke("ShowSwitchGrapHints", 1.0f);

            }
            
            if (menuObject.activeSelf)
            {
                menuObject.SetActive(false);
            }
            else { menuObject.SetActive(true); }
        }


        /*
        //********************************************************For Udacity Menu***********************************

        if (m_IsTouch[SteamVR_Input_Sources.Any].stateDown)
        {
            //This sets the first touchLast, so the distance is correctly computed as 0 & not the axis value of our finger when we initially press down

            // touchLast = SteamVR_Actions.default_TouchpadTouch.GetAxis(EVRButtonId. k_EButton_SteamVR_Touchpad).x;
            //touchLast = Input.GetAxis("Horizontal");
            touchLast = SteamVR_Actions.default_TouchpadTouch.GetAxis(SteamVR_Input_Sources.LeftHand).x;
        }

        if (m_IsTouch[SteamVR_Input_Sources.Any].state)
        {
            //Where the finger is located

            // touchCurrent = SteamVR_Actions.default_TouchpadTouch.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad).x;
            // touchCurrent = Input.GetAxis("Horizontal");
            touchCurrent = SteamVR_Actions.default_TouchpadTouch.GetAxis(SteamVR_Input_Sources.LeftHand).x;


            if (oculus)
            {

                swipeSum = touchCurrent;
            }
            else
            {
                distance = touchCurrent - touchLast; //Moves between two frames, the distance is where my finger's location (so we know which is the last fram)

                touchLast = touchCurrent;
                //swipeSum += distance: Keeps track of the total amount our finger has moved, so that we can trigger the swipe action after a certain amount of total distance
                swipeSum += distance;
            }
            //HasSwipedRight & HasSwipedLeft: to prevent the accident that happend due to that we can trigger multiple swipes with one journey across the touchpad
            if (!hasSwipedRight)
            {
                if (swipeSum > 0.5f)
                {
                    touchLast = 0;
                    Debug.Log("hasSwipeRight swipeSum = " + swipeSum);
                    swipeSum = 0;
                    SwipeRight();
                    hasSwipedRight = true;
                    hasSwipedLeft = false;
                }
            }
            if (!hasSwipedLeft)
            {
                if (swipeSum < -0.5f)
                {
                    touchLast = 0;
                    Debug.Log("hasSwipeLeft swipeSum = " + swipeSum);
                    swipeSum = 0;
                    SwipeLeft();
                    hasSwipedRight = false;
                    hasSwipedLeft = true;
                }
            }
        }
        if (m_IsTouch[SteamVR_Input_Sources.Any].stateUp)
        {
            swipeSum = 0;
            touchLast = 0;
            touchCurrent = 0;
            hasSwipedRight = false;
            hasSwipedLeft = false;

        }
        if (SteamVR_Actions.default_Teleport.GetStateDown(SteamVR_Input_Sources.LeftHand))
        {
            //Spawn object currently selected by Menu
            SpawnObject();
        }*/
    }

    /*
    void SpawnObject()
    {
        objectMenuManager.SpawnCurrentObject();
    }

    void SwipeRight()
    {
        objectMenuManager.MenuRight();


    }
    void SwipeLeft()
    {
        objectMenuManager.MenuLeft();

    }
*/

    //---------------------------------------------------------------------------- Delete Hint Code ---------------------------
 
    private Coroutine btnAndTxtHintCoroutine;
    private Coroutine SwitchHintCoroutine;
    private Coroutine GrabHintCoroutine;

    //-------------------------------------------------
    public void ShowHints()
    {
        DisableHints();
     
            if (btnAndTxtHintCoroutine != null)
            {
                StopCoroutine(btnAndTxtHintCoroutine);
                btnAndTxtHintCoroutine = null;
            }
            btnAndTxtHintCoroutine = StartCoroutine(MenuButtonHints(m_MenuAction, "Menu"));
       
    }
    public void ShowSwitchGrapHints()
    {
        DisableSwitchGrabHints();

            if (SwitchHintCoroutine != null)
            {
                StopCoroutine(SwitchHintCoroutine);
                SwitchHintCoroutine = null;
            }
            if (GrabHintCoroutine != null)
            {
                StopCoroutine(GrabHintCoroutine);
                GrabHintCoroutine = null;
            }
        
            SwitchHintCoroutine = StartCoroutine(MenuButtonHints(radialMenuObject.actionThumbstickIsPressed, "Switch Items"));
            GrabHintCoroutine = StartCoroutine(MenuButtonHints(radialMenuObject.actionConfirm, "Grab Item"));

    }
    //-------------------------------------------------
    public void DisableHints()
    {
        if (btnAndTxtHintCoroutine != null)
        {
           // ControllerButtonHints.HideTextHint(player.leftHand, m_MenuAction);
            ControllerButtonHints.HideTextHint(player.rightHand, m_MenuAction);
            
            StopCoroutine(btnAndTxtHintCoroutine);
            btnAndTxtHintCoroutine = null;
        }
        CancelInvoke("ShowHints"); 
    }

    public void DisableSwitchGrabHints()
    {
        //Debug.Log("Disable Switch And Grap Hints is invoked");
        if (SwitchHintCoroutine != null && !switchHint)
        {
            // ControllerButtonHints.HideTextHint(player.leftHand, m_MenuAction);
            ControllerButtonHints.HideTextHint(player.rightHand, radialMenuObject.actionThumbstickIsPressed);

            StopCoroutine(SwitchHintCoroutine);
            SwitchHintCoroutine = null;
        }
        if (GrabHintCoroutine != null && !grabItemsHint)
        {
            // ControllerButtonHints.HideTextHint(player.leftHand, m_MenuAction);
            ControllerButtonHints.HideTextHint(player.rightHand, radialMenuObject.actionConfirm);

            StopCoroutine(GrabHintCoroutine);
            GrabHintCoroutine = null;
        }

        if (!switchHint && !grabItemsHint)
        {
            CancelInvoke("ShowSwitchGrapHints");
        }
    }
    //-------------------------------------------------
    // Cycles through all the button hints on the controller
    //-------------------------------------------------
    private IEnumerator MenuButtonHints(SteamVR_Action_Boolean theAction, string hintText)
    {
        Hand hand = player.rightHand;
        while (true)
        {
          //  foreach (Hand hand in player.hands)
           // {
              
             // if(hand.handType == SteamVR_Input_Sources.LeftHand || hand.handType == SteamVR_Input_Sources.RightHand)
              //  {
                    if (theAction.GetActive(hand.handType))
                    {
                        ControllerButtonHints.ShowTextHint(hand, theAction, hintText);
   
                    }
                    else
                    {
                        ControllerButtonHints.HideButtonHint(hand, theAction); //HideAllButtonHints(hand);
                    }
              // }
          //   }

            yield return new WaitForSeconds(3.0f);

            yield return null;
        }
    }


   
}
