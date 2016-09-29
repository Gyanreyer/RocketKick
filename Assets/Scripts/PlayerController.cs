using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    private Rigidbody2D playerRB;
    private Vector3 velocity;
    public float movementSpeed = 4f;
    public float jumpStrength = 8f;

    public float gravity = 1f;

    private bool grounded = true;
    //private bool pressedLastFrame

    public float jumpDuration = 1f;
    private float jumpTimer;

    private float chargeTimer;

	// Use this for initialization
	void Start () {
        playerRB = GetComponent<Rigidbody2D>();
        velocity = Vector3.zero;
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        //Get input from left stick and put in x axis of rigidbody's velocity
        playerRB.velocity = new Vector2(Input.GetAxis("Left Stick X Axis P1") * movementSpeed,playerRB.velocity.y);


        //If A button initially pressed, start jump w/ burst of upward force - jumpReleased prevents from constantly jumping if you hold it down
        if (Input.GetButtonDown("A") && grounded)
        {
            playerRB.AddForce(new Vector2(0, jumpStrength*10));
            grounded = false;
            jumpTimer = 0;
        }

        //If A button is held, continue adding force for a bit so you can increase jump height by holding
        else if (Input.GetButton("A") && jumpTimer < jumpDuration && !grounded)
        {       
            playerRB.AddForce(new Vector2(0,jumpStrength));
            jumpTimer += Time.deltaTime;
        }

        //If jump button has been released, set jumpReleased accordingly
        else if (Input.GetButtonUp("A"))
        { 
            jumpTimer = jumpDuration;
        }
    }


    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Floor")
        {
            grounded = true;
            jumpTimer = 0;
        }
    }


}
