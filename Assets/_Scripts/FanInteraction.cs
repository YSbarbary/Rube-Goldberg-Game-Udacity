using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.InteractionSystem;

public class FanInteraction : MonoBehaviour {


  //  public Transform ParentFan;
    private float FanPower = 10f; // how strongly the fan will blow sideways (x axis), 5 by default 
   // public float score = 10;
   // public GameObject theFan;

    public float distance;
    public float surfaceArea;
    public float appliedForce;
    public GameObject FanBladesEffect;


    // Use this for initialization
    void Start () {
       // GetComponent<Interactable>().gameObject.SetActive(false);
       
       /*Vector3 fanCollSize = this.gameObject.GetComponent<BoxCollider>().size; //theFan.collider.transform.localScale;
        if (fanCollSize.Equals(new Vector3(0,0,0))) {
            surfaceArea = 0.01f * 0.005f * 0.05f; 
        }
        else
        {
            surfaceArea = fanCollSize.x * fanCollSize.y * fanCollSize.z;

        }
        */

     //   FanPower = FanPower * -ParentFan.localScale.x; // multiple by the direction the fan is facing to direct the air
    }
	
	// Update is called once per frame
	void Update () {
        FanBladesEffect.transform.Rotate(0, 0, 50);
    }


    void OnTriggerEnter(Collider other)
    {
        
        /*if (other.gameObject.CompareTag("Ball"))
        { //Get fan transform (parent of fanCollision)
          // other.gameObject.transform.SetParent(this.transform);   
        }
        //   other.transform.position = new Vector3(0, 0, 0);
        // this.score += 5;

        //Debug.Log(other.name + " entered fan trigger area");
        */

        if (other.gameObject.tag == "Ball")
        {
            // other.gameObject.GetComponent<Rigidbody>().AddForce(new Vector2(FanPower, 25));

            /*
             * Factor one:  distance * distance:
              The force it applies after the Inverse-Square Law. It means that the force it applies to another object is divided by the distance multiplied with itself.
              
            *Factor two: 1.0 + :
              When the distance is 0. When the distance is below 1, the force will be more than the intended maximum. 
 When the distance gets very small, the force can become extremely strong. This could lead to quite glitchy behavior. 
 To fix this issue, add 1 to your distance calculation, or cap the distance at 1.
  
            Factor three: surfaceArea:
  Wind drag means that objects with a large surface exposed to the wind direction receive more force from the wind than those with a smaller surface. 
  When you want to model this, you also need to multiply the force with the surface area of the object which is pushed.
  */
            //distance = System.Math.Abs(other.gameObject.GetComponent<Rigidbody>().transform.position.x - this.transform.position.x);
            //appliedForce = FanPower / (1.0f + distance * distance) * surfaceArea;
            //other.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * appliedForce, ForceMode.Force);


            Rigidbody rb = other.gameObject.GetComponentInParent<Rigidbody>(); //InParent, Because the throwball ball is a sphere child of parent that has the scripts
                                                                               // FanPower += rb.velocity.x;
                                                                               //rb.AddForce(this.transform.forward * FanPower, ForceMode.Force);
            rb.AddForce(- this.transform.forward * FanPower, ForceMode.Impulse);
        }
    }

    /*
    void OnTriggerStay(Collider other)
    {
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Ball")
        {
           // other.gameObject.transform.SetParent(null);
           // DontDestroyOnLoad(other.gameObject);
        }

    }*/

  

}
