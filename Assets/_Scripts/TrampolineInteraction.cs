using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;


public class TrampolineInteraction : MonoBehaviour {

   // public Rigidbody rb;
    //Animator anim;
    public bool customSpeed;
    public Vector2 customVelocity;
    public float multiplier = 10f;

    public float speed = 5 ;
    public float jumpForce = 7;

    bool onTop;
    public GameObject bouncer;
    
    Vector2 velocity;


 //   public Throwable th;

    // Use this for initialization
    void Start () {
        //  rb = GetComponent<Rigidbody>();
       // th = GetComponent<Throwable>();
    }
	
	// Update is called once per frame
	void Update () {
        //speed = rb.velocity.magnitude;


       // float moveHorizontal = Input.GetAxis("Horizontal");
       // float moveVertical = Input.GetAxis("Vertical");
       // Vector3 movement = new Vector3(moveHorizontal, 0, moveVertical);
       // rb.AddForce(movement * speed);
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "Ball")
        {

            //other.gameObject.GetComponent<Rigidbody>().AddForce (Vector3.up * jumpForce, ForceMode.Impulse);


            speed = other.gameObject.GetComponent<Rigidbody>().velocity.magnitude;
           
            other.gameObject.GetComponent<Rigidbody>().AddForce(Vector3.up * speed * 5, ForceMode.Impulse);  
        }

        /*    if (onTop)
            {
                anim.SetBool("isStepped", true);
                bouncer = other.gameObject;
            }
               */
        // if (other.gameObject.tag == "Player")
        // {
        //other.gameObject.rigidbody2D.velocity = Vector2.zero;

        /* other.gameObject.GetComponent<Rigidbody>().velocity = Vector2.zero;

         //other.gameObject.rigidbody2D.AddForce(transform.forward * forceAmount, ForceMode2D.Impulse);
         //transform.up
         //If you want to have fixed force consider changing rigidbody2D.velocity directly
         //other.gameObject.rigidbody2D.AddForce(transform.forward * 2500, ForceMode2D.Impulse);
         other.gameObject.GetComponent<Rigidbody>().AddForce(transform.forward * 2500, ForceMode.Impulse);

           // other.gameObject.GetComponent<SimplePlayer0>().BaseSpeed = velocity.x;
            other.gameObject.GetComponent<Rigidbody>().velocity = velocity.x;

         // }
         */
    }
    /*
    void Jump()
    {

        if (customSpeed)
            velocity = customVelocity;
        else
            velocity = transform.up * multiplier;


        bouncer.GetComponent<Rigidbody2D>().velocity = velocity;

    }
*/
    /*
        void OnTriggerEnter2D()
        {
            onTop = true;
        }
        void OnTriggerExit2D()
        {
            onTop = false;
            anim.SetBool("isStepped", false);

        }

        void OnTriggerStay2D()
        {
            onTop = true;
        }
    */


}
