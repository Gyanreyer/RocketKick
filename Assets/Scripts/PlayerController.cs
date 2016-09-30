using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public int playerNum;

    private Rigidbody2D playerRB;
    public float movementSpeed = 4f;

    private Vector2 kickForce;
    private bool kick;
    private float chargeKickTimer;
    public float chargeLength;

    private bool kicking;

    public float maxKick;
    public float minKick;

	// Use this for initialization
	void Start () {
        playerRB = GetComponent<Rigidbody2D>();
    }
	
	// Update is called once per frame
	void Update () {

        //Get input from left stick and move position's x axis accordingly
        transform.position += new Vector3(Input.GetAxis("Left Stick X Axis P" + playerNum) * movementSpeed*Time.deltaTime,0,0);

        Vector2 rightStick = new Vector2(Input.GetAxis("Right Stick X Axis P" + playerNum), Input.GetAxis("Right Stick Y Axis P" + playerNum));

        if (rightStick.magnitude > 0.75)
        {  
            kickForce = rightStick.normalized;

            if (chargeKickTimer < chargeLength)
            {
                chargeKickTimer += Time.deltaTime;
            }

        }
        else if(kickForce.sqrMagnitude > 0 && !kick)
        {
            float kickMagnitude = (chargeKickTimer / chargeLength) * maxKick;

            if (kickMagnitude < minKick)
                kickMagnitude = minKick;

            kickForce *= kickMagnitude;
            chargeKickTimer = 0;


            kick = true;
        }

        if(transform.position.y < -7)
        {
            transform.position = new Vector3(0, -3, 0);
        }

    }

    //Fixed update for physics stuff
    void FixedUpdate()
    {
        if (kick)
        { 
            
            playerRB.AddForce(Vector2.ClampMagnitude(kickForce, maxKick),ForceMode2D.Impulse);
            kick = false;

            kickForce = Vector2.zero;
        }

        //playerRB.velocity += velocity; THIS CODE BROKE EVERYTHING
    }


}
