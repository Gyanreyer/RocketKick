using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public int playerNum;

    private Rigidbody2D playerRB;
    private Vector3 velocity;
    public float movementSpeed = 4f;
    public float jumpStrength = 1f;

    private bool grounded = true;

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
        playerRB.velocity = new Vector2(Input.GetAxis("Left Stick X Axis P" + playerNum) * movementSpeed,playerRB.velocity.y);


        //If A button initially pressed, start jump w/ burst of upward force - jumpReleased prevents from constantly jumping if you hold it down
        if (Input.GetButtonDown("A P" + playerNum) && grounded)
        {
            playerRB.velocity = new Vector2(playerRB.velocity.x,0);
            playerRB.AddForce(new Vector2(0, jumpStrength), ForceMode2D.Impulse);
            grounded = false;
            jumpTimer = 0;
        }

        //If A button is held, continue adding force for a bit so you can increase jump height by holding
        else if (Input.GetButton("A P" + playerNum) && jumpTimer < jumpDuration && !grounded)
        {       
            playerRB.AddForce(new Vector2(0,jumpStrength*3));
            jumpTimer += Time.deltaTime;
        }

        //If jump button has been released, set jumpReleased accordingly
        if (Input.GetButtonUp("A P" + playerNum))
        {
            jumpTimer = jumpDuration;
        }
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        grounded = true;
    }


}
