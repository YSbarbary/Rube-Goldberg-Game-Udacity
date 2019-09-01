using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;

public class ObjectMenuManager : MonoBehaviour {
    public List<GameObject> objectList; //handled automatically at start (menu object "icons")
    public List<GameObject> objectPrefabList; //set manually in inspector & MUST match order of scene menu objects (prefab to instantiate for each type) 
    public int currentObject = 0; // current selection index number




    // Use this for initialization
    void Start () {
        currentObject = 0;
        foreach (Transform child in transform)
        {
            objectList.Add(child.gameObject); //Hold the menu object in our scene
        }
		
	}

   /* // Update is called once per frame
    void Update()
    {

    }*/

    public void MenuLeft()
    {
        objectList[currentObject].SetActive(false); //Disable menu object is currently showing

        currentObject--; //Go perevious position 

        /* The menu will be like this: item 2, item 0 , item 1, item 2, item 0 ...
       (currentObject < 0), which means it's:
       if after subraction takes us to number outside of our list, we have to go and loop around the other side of it.   
         */
        if (currentObject < 0) 
        {
            currentObject = objectList.Count - 1;
        }
       

            objectList[currentObject].SetActive(true);
    }
    public void MenuRight()
    {
        objectList[currentObject].SetActive(false);
        currentObject++; //Go next position
        if (currentObject > objectList.Count - 1)
        {
            currentObject = 0;
        }
       

            objectList[currentObject].SetActive(true);

    }

    public void SpawnCurrentObject()
    {
        Instantiate(objectPrefabList[currentObject], objectList[currentObject].transform.position, objectList[currentObject].transform.rotation);
    }

}
