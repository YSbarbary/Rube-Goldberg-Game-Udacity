using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;
//using UnityEditor;

public class PlayingBall : MonoBehaviour
{

    public Throwable th;
    public CreatingBall createdBall; 


    // public float lifeTime = 300f; // 5 minut
    // private int count = 0;


    // Use this for initialization
    void Start()
    {
        th = GetComponent<Throwable>();
        createdBall = GetComponent<CreatingBall>();
    }


    // Update is called once per frame
    void Update()
    {

        /* if (lifeTime > 0)
         {
             lifeTime -= Time.deltaTime;
             if (lifeTime <= 0)
             {
                 Destruction();
             }
         }*/

    }


    void OnCollisionEnter(Collision col)
    {
       
        if (col.gameObject.CompareTag("Floor"))
        {
            Debug.Log("The Ball at Floor (OnCollisionEnter)");

            GameObject[] gobj = GameObject.FindGameObjectsWithTag("Ball");
            if (gobj == null || gobj.Length == 0 || gobj.Length == 3) //The 3 balls is balls in Box
            {
                Debug.Log("Creating Ball in PlayingBall.cs");
                // Instantiate(th.BallCreatorObject, th.BallCreatorPosition.position, th.BallCreatorPosition.rotation);
                Instantiate(createdBall.BallObject, createdBall.transform.position, createdBall.transform.rotation); 
            }

            WaitTime();
            Destruction();


            // foreach(star in this.gameObject.CompareTag("Star"))
            //  col.gameObject.SetActive(true);

            MainGameLogic.instace.collectedStarsCount = 0;
            foreach (GameObject star in MainGameLogic.instace.StarsList)
            {
                star.SetActive(true);//Make it true (Active)
            }
   
        }

        else if (col.gameObject.CompareTag("Star"))
        {
            //check
            if (th.IsAttached())
            {
                Debug.Log("The player cheat");
                //player is cheating
                // EditorUtility.DisplayDialog("Cheating Alert", "Please don't cheat.", "Ok");
               // MainGameLogic.instace.cheatingAlert.transform.position = new Vector3(Player.instance.transform.position.x, Player.instance.transform.position.y, Player.instance.transform.position.z + 1);
                MainGameLogic.instace.cheatingAlert.SetActive(true); //!game.cheatingAlert.gameObject.activeSelf
                StartCoroutine(DelayDeactivate());
                
            }
            else
            {
                Debug.Log("The Ball collide Star ");

                //disable star item
                col.gameObject.SetActive(false); //Make it false (unActive) !col.gameObject.activeSelf
                MainGameLogic.instace.collectedStarsCount++;

            }
        }

    }

    IEnumerator WaitTime()
    {
        yield return new WaitForSeconds(5); //Wait 5 seconds then destroy it
    }

    void OnBecameInvisible()
    {
        Debug.Log("Invisible Ball");
        Destruction();
    }

    void Destruction()
    {
        Destroy(this.gameObject);
    }

    IEnumerator DelayDeactivate()
    {
        yield return new WaitForSeconds(3);
        MainGameLogic.instace.cheatingAlert.SetActive(false); //Deactivate !game.cheatingAlert.gameObject.activeSelf
    }
}
