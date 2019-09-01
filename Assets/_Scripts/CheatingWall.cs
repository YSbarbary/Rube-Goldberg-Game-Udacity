using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class CheatingWall : MonoBehaviour {

    private Throwable th;
    private GameObject createdBallObj;
    private CreatingBall createdBall;


    // Use this for initialization
    void Start()
    {
        createdBallObj = GameObject.Find("CreatingBall");
        if (createdBallObj != null)
        {
            createdBall = createdBallObj.GetComponent<CreatingBall>();
        }
    }

    private void OnTriggerExit(Collider col)
    {
        
        if (col.CompareTag("Ball"))
        {
            th = col.gameObject.GetComponentInParent<Throwable>();
            if (th.IsAttached())
            {
                WaitTime();
                //Instantiate(th.BallCreatorObject, th.BallCreatorPosition.position, th.BallCreatorPosition.rotation);
                Instantiate(createdBall.BallObject, createdBall.transform.position, createdBall.transform.rotation);
                Destroy(col.gameObject);
                
            }
        }

    }
 /*   private void OnTriggerEnter(Collider col)
    {
      
    }*/

    IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(1); //Wait 1 second then destroy it
    }
}
