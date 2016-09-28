using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public GameObject player;
    public Rigidbody2D playerRB;

	// Use this for initialization
	void Start () {
        playerRB = player.GetComponent<Rigidbody2D>();
	}
	
	// Update is called once per frame
	void Update () {
	    
        //player controls go here
        

	}
}
