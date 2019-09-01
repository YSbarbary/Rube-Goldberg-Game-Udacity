using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeleportTargetInteraction : MonoBehaviour {

    public float jumpForce = 7;


    // Use this for initialization
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ball")
        {

            other.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * jumpForce, ForceMode.Impulse);


        }
    }

       

}
