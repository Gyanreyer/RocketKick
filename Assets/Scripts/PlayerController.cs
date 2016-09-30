using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public int playerNum;

    private Rigidbody2D playerRB;
    public float movementSpeed = 4f;
    public float jumpStrength = 1f;

    private bool grounded = true;

    public float jumpDuration = 1f;
    private float jumpTimer;

    private Vector2 kickForce;
    private bool kick;
    private float chargeKickTimer;
    public float chargeLength;

    public float maxKick;

    private Vector2 velocity;

	// Use this for initialization
	void Start () {
        playerRB = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {

        //Get input from left stick and put in x axis of rigidbody's velocity
        velocity = new Vector2(Input.GetAxis("Left Stick X Axis P" + playerNum) * movementSpeed,playerRB.velocity.y);

        /*
        //If A button initially pressed, start jump w/ burst of upward force - jumpReleased prevents from constantly jumping if you hold it down
        if (Input.GetButton("A P" + playerNum) && grounded)
        {
            velocity = new Vector2(playerRB.velocity.x,0);
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
        }*/

        Vector2 rightStick = new Vector2(Input.GetAxis("Right Stick X Axis P" + playerNum), Input.GetAxis("Right Stick Y Axis P" + playerNum));

        if (rightStick.magnitude > 0.75)
        {  
            kickForce = rightStick.normalized;

            if (chargeKickTimer < chargeLength)
            {
                chargeKickTimer += Time.deltaTime;
            }

        }
        else if(kickForce.sqrMagnitude > 0)
        {
            kickForce *= (chargeKickTimer / chargeLength) * maxKick;
            chargeKickTimer = 0;

            

            kick = true;
        }
    }

    void FixedUpdate()
    {
        playerRB.velocity = velocity;

        if (kick)
        {
            playerRB.AddForce(new Vector2(kickForce.x*10,kickForce.y),ForceMode2D.Impulse);
            kick = false;

            kickForce = Vector2.zero;
        }

    }


    void OnCollisionEnter2D(Collision2D other)
    {
        grounded = true;
    }


}
