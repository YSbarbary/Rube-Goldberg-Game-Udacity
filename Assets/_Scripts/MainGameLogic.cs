using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR.InteractionSystem;

public class MainGameLogic : MonoBehaviour {

    public List<GameObject> StarsList;
    public int collectedStarsCount = 0;
    public static int nextLevelnumber = 1; //We start loading from level 2
    public GameObject cheatingAlert;
    public GameObject winingAlert;
    // public Player player; // There is a Player.instance
    public static MainGameLogic instace; //Do MainGameLogic as a Singlton class

    // Use this for initialization
    void Start() {
        instace = this; //Do MainGameLogicas a Singlton class
                        // player = Player.instance; //GetComponent<Player>();
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
