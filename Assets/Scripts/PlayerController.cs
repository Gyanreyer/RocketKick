using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    private Rigidbody2D playerRB;
    private Vector3 velocity;
    public float movementSpeed = 4f;
    public float jumpStrength = 8f;

    public float gravity = 1f;

    private bool grounded = true;
    private Transform groundCheck;

    public float jumpDuration = 1f;
    private float jumpTimer;
    

	// Use this for initialization
	void Start () {
        playerRB = GetComponent<Rigidbody2D>();
        velocity = Vector3.zero;
    }
	
	// Update is called once per frame
	void FixedUpdate () {

        playerRB.velocity = new Vector2(Input.GetAxis("Left Stick X Axis P1") * movementSpeed,playerRB.velocity.y);
        //player controls go here
        //velocity.x = Input.GetAxis("Left Stick X Axis P1") * movementSpeed;

        if (Input.GetButton("A"))
        {
            Vector2 jumpForce = Vector2.zero;

            if (grounded)
            {
                jumpForce.y = jumpStrength*5;
                grounded = false;

                jumpTimer = 0;
            }
            else if(jumpTimer < jumpDuration)
            {
                jumpTimer += Time.deltaTime;
                jumpForce.y = jumpStrength;
            }

            playerRB.AddForce(jumpForce);
        }
        else
        {
            jumpTimer = jumpDuration;
        }

        //playerRB.velocity = velocity * Time.deltaTime;
        //transform.position += (velocity * Time.deltaTime);
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
