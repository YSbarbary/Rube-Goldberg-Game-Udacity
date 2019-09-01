using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.SceneManagement;

public class AchievingTheGoal : MonoBehaviour {
    bool targetIsTouchedAndWin = false;

    // Use this for initialization
    void Start () {

        AudioListener[] aL = FindObjectsOfType<AudioListener>();

        Debug.Log("Count of  AudioListener[] aL is :  "+ aL.Length);

            for (int i = 0; i < aL.Length; i++)
            {
                //Ignore the first AudioListener in the array 
                if (i == (aL.Length -1))
                    continue;

                //Destroy 
                DestroyImmediate(aL[i]);
            }
     
    }
	
	// Update is called once per frame
	void Update () {
	}

    private void OnTriggerEnter(Collider col)
    {
        if (!targetIsTouchedAndWin)
        {
            LoadNewScene(col);
        }
    }
    private void OnTriggerStay(Collider col)
    {
        if (!targetIsTouchedAndWin) {
        LoadNewScene(col);
        }
    }

    private void OnTriggerExit(Collider col)
    {
        if (!targetIsTouchedAndWin)
        {
            LoadNewScene(col);
        }

    }

    void LoadNewScene(Collider col)
    {
        if (col.CompareTag("Ball") && MainGameLogic.instace.collectedStarsCount == MainGameLogic.instace.StarsList.Count)
        {
            targetIsTouchedAndWin = true;
            // You Win message
            Debug.Log("You win!");

            //Loading new Level
            MainGameLogic.nextLevelnumber++;
            if (MainGameLogic.nextLevelnumber < 5)
            {
                string levelName = "Scene" + MainGameLogic.nextLevelnumber;
                Debug.Log("The level name is:" + levelName);
                SteamVR_LoadLevel.Begin(levelName, false, 2.5f, 0, 0, 0, 1);
                //SteamVR_LoadLevel.Begin(levelName, true, 2.5f, 0, 0, 0, 1);
            }
            else
            {
                MainGameLogic.instace.winingAlert.SetActive(true); 
                StartCoroutine(DelayDeactivate());
            }

        }
    }

    IEnumerator DelayDeactivate()
        {
            yield return new WaitForSeconds(8);
            MainGameLogic.instace.winingAlert.SetActive(false); //Deactivate !game.cheatingAlert.gameObject.activeSelf
        }

    
  /*  void OnEnable()
    {
        //Tell our 'OnLevelFinishedLoading' function to start listening for a scene change as soon as this script is enabled.
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }

    void OnDisable()
    {
        //Tell our 'OnLevelFinishedLoading' function to stop listening for a scene change as soon as this script is disabled. Remember to always have an unsubscription for every delegate you subscribe to!
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        Debug.Log("Level Loaded");
        Debug.Log(scene.name);
        Debug.Log(mode);
    }*/
    /*
    void OnCollisionEnter(Collision col)
    {
    }
    private void OnCollisionExit(Collision col)
    {
    }*/
}
