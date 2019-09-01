using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;


public class WMR_Input : MonoBehaviour {

    //[SteamVR_DefaultAction("squeeze")];
    
    public SteamVR_Action_Single squeezeAction;
   // public SteamVR_Action_Boolean m_BooleanAction;
    public SteamVR_Action_Vector2 touchPadAction;

    //public SteamVR_Input_Sources teleoprtHandType = hand.handType

    // Update is called once per frame
    void Update()
    {
        //SteamVR_Input._default.inActions.Teleport.GetStateDown(SteamVR_Input_Sources.Any);
        if (SteamVR_Actions.default_Teleport.GetStateDown(SteamVR_Input_Sources.Any))
        {
            print("Teleport Down");
        }
        if (SteamVR_Actions.default_GrabPinch.GetStateUp(SteamVR_Input_Sources.Any))
        {
            print("Grab Pinch UP");
        }

        float triggerValue = squeezeAction.GetAxis(SteamVR_Input_Sources.Any);
        if (triggerValue > 0.0f)
        {
            print(triggerValue);
        }

        Vector2 touchpadValue = touchPadAction.GetAxis(SteamVR_Input_Sources.Any);
        if(touchpadValue != Vector2.zero)
        {
            print(touchpadValue);
        }


    }

    }
