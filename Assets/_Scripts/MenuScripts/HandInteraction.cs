using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class HandInteraction : MonoBehaviour {

   // public Rigidbody rb;
    public SteamVR_Action_Vibration hapticAction = SteamVR_Input.GetAction<SteamVR_Action_Vibration>("Haptic");
    public float throwForce = 1.5f;
   // private Hand hand;
    private SteamVR_Behaviour_Pose m_PoseBehaviour;

    //public SteamVR_Action_Vector2 actionDeleteMenuTool; //We do it as Vector2 not Boolean, because Oculus's GrapGrip input takes values from 0 to 1
    public SteamVR_Action_Boolean actionDeleteMenuTool = SteamVR_Input.GetAction<SteamVR_Action_Boolean>("Grab Grip");
    private Player player = null;
    private Coroutine btnAndTxtDeleteToolHintCoroutine;

    private int countCallingHint = 2; //times to appear the hint
    private bool isThisFirstTimeCalled;
    private float hintTimer = 15.0f;

    void Start()
    {

        isThisFirstTimeCalled = true;
        player = Player.instance;

        if (player == null)
        {
            Debug.LogError("<b>[SteamVR Interaction]</b> Teleport: No Player instance found in map.");
            Destroy(this.gameObject);
            return;
        }
    }

    private void Update()
    {
        if ( btnAndTxtDeleteToolHintCoroutine != null) // btnAndTxtDeleteToolHintCoroutine != null ==> the button doesn't used yet
        {
            hintTimer -= Time.deltaTime;

            if (hintTimer < 0)
            {
                DisableDeleteToolHint();
            }
        }
    }

    private void OnTriggerEnter(Collider col)
    { 
        if (col.gameObject.CompareTag("MenuTool") && col.gameObject.layer != LayerMask.NameToLayer("FanCollider"))
        {
            if (isThisFirstTimeCalled) {
                isThisFirstTimeCalled = false;
                Invoke("ShowDeleteToolHint", 5.0f);
            }
        }
    }

    private void OnTriggerStay(Collider col)
    {
        if (col.gameObject.CompareTag("MenuTool") && col.gameObject.layer != LayerMask.NameToLayer("FanCollider"))
        {
            
            if (SteamVR_Actions.default_GrabPinch.GetStateUp(SteamVR_Input_Sources.RightHand)) //Release
            {
                   ThrowObject(col);
            }

            if (SteamVR_Actions.default_GrabPinch.GetStateDown(SteamVR_Input_Sources.RightHand)) //Pressed
            {
                    GrabObject(col); 
            }

            if (SteamVR_Actions.default_GrabGrip.GetStateDown(SteamVR_Input_Sources.LeftHand))//actionDeleteMenuTool.GetStateDown(SteamVR_Input_Sources.LeftHand)) //Release
            {  
                DisableDeleteToolHint();

                Debug.Log("To Destroy");
                Destroy(col.gameObject); 
            }

        }
    }

    private void OnTriggerExit(Collider col)
    {
       // Debug.Log("I am at OnTriggerExit");

    }

    void GrabObject(Collider coli)
    {
        coli.transform.SetParent(gameObject.transform); //The object take the same transform of controller as a child
        coli.GetComponent<Rigidbody>().isKinematic = true; //To effect by gravity (Turn off physics)
        TriggerHapticPulse(2000); // Vibrate the controller

        //Debug.Log("You are touching down the trigger on the object");

    }

    void ThrowObject(Collider coli)
    {
        coli.transform.SetParent(null); //Unparent the controller from the object
        Rigidbody rb = coli.GetComponent<Rigidbody>();
        // rb.isKinematic = false; //Turn On physics


        //hand.GetTrackedObjectVelocity()  * throwForce;  //device.velocity
        // rb.velocity = m_PoseBehaviour.GetVelocity() * throwForce;  // set velocity based on controller movement
        //hand.GetTrackedObjectAngularVelocity();  //device.angularVelocity
        // rb.angularVelocity = m_PoseBehaviour.GetAngularVelocity();

        //Debug.Log("You have released the trigger ");
    }

    public void TriggerHapticPulse(ushort microSecondsDuration)
    {
        float seconds = (float)microSecondsDuration / 1000000f;
        hapticAction.Execute(0, seconds, 1f / seconds, 1, SteamVR_Input_Sources.Any);
    }

    public void TriggerHapticPulse(float duration, float frequency, float amplitude)
    {
        hapticAction.Execute(0, duration, frequency, amplitude, SteamVR_Input_Sources.Any);
    }


    //################################################# Hint Code for Delete Tool ##############################################################
 
    

    //-------------------------------------------------
    public void ShowDeleteToolHint()
    {
        Debug.Log(" ShowDeleteToolHint  is invoked");
        DisableDeleteToolHint();
        if (btnAndTxtDeleteToolHintCoroutine != null)
        {
            StopCoroutine(btnAndTxtDeleteToolHintCoroutine);
        }
        btnAndTxtDeleteToolHintCoroutine = StartCoroutine(DeleteToolHintCoroutine());      
    }

    //-------------------------------------------------
    public void DisableDeleteToolHint()
    {
        Debug.Log("Disable Delete Tool Hints is invoked");
        if (btnAndTxtDeleteToolHintCoroutine != null)
        {
            ControllerButtonHints.HideTextHint(player.leftHand, actionDeleteMenuTool);

            StopCoroutine(btnAndTxtDeleteToolHintCoroutine);
            btnAndTxtDeleteToolHintCoroutine = null;
        }

        CancelInvoke("ShowDeleteToolHint");
    }

    //-------------------------------------------------
    // Cycles through all the button hints on the controller
    //-------------------------------------------------
    private IEnumerator DeleteToolHintCoroutine()
    {
        Hand hand = player.leftHand;
        while (hintTimer > 0)
        {
            if (actionDeleteMenuTool.GetActive(hand.handType))
            {
                ControllerButtonHints.ShowTextHint(hand, actionDeleteMenuTool, "  Delete Tool");
            }
            else
            {
                ControllerButtonHints.HideButtonHint(hand, actionDeleteMenuTool);
            }

            yield return new WaitForSeconds(3.0f);

            yield return null;
        }
    }

}
