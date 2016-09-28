using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    private Rigidbody2D playerRB;
    private Vector2 movementVector;
    public float movementSpeed = 200.0f;

	// Use this for initialization
	void Start () {
        playerRB = GetComponent<Rigidbody2D>();
        Debug.Log(playerRB);
        movementVector = new Vector2(0.0f, 0.0f);	}
	
	// Update is called once per frame
	void Update () {
        //player controls go here
        movementVector.x = Input.GetAxis("Left Stick X Axis P1") * movementSpeed;
        Debug.Log(Input.GetAxis("Left Stick X Axis P1"));
        if(Input.GetButton("A"))
        {
            movementVector.y = 100.0f;
        }

        playerRB.AddForce(movementVector);
        movementVector = Vector2.zero;
    }
}
