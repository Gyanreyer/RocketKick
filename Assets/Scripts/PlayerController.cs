using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public int playerNum = 0;

    private Rigidbody2D playerRB;
    public float movementSpeed = 4f;

    private Vector2 kickForce;
    private bool kick;
    private float chargeKickTimer;
    public float chargeLength;

    private bool kicking;

    public float maxKick;
    public float minKick;

    public int maxNumKicks = 3;
    private int kicksLeft;

    private GameObject feet;

	// Use this for initialization
	void Start () {
        playerRB = GetComponent<Rigidbody2D>();

        kicksLeft = maxNumKicks; 

        if(playerNum!=0)
            feet = GameObject.Find("Feet" + playerNum);
    }

    public void setPlayerNum(int i)
    {
        playerNum = i;
        feet = GameObject.Find("Feet" + i);
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

            kicksLeft--;
        }

        if(transform.position.y < -7)
        {
            transform.position = new Vector3(0, -3, 0);
        }

        
        if (playerNum != 0)
        {
            feet.transform.position = transform.position;

            BoxCollider2D footCollider = feet.GetComponent<BoxCollider2D>();
            if (kicksLeft <= maxNumKicks)
            {
                footCollider.enabled = true;
                footCollider.offset = playerRB.velocity.normalized * .5f;
            }
            else
            {
                footCollider.enabled = false;
            }
        }

    }

    //Fixed update for physics stuff
    void FixedUpdate()
    {
        if (kick && kicksLeft > 0)
        {
            playerRB.velocity = Vector2.zero;

            playerRB.AddForce(Vector2.ClampMagnitude(kickForce, maxKick),ForceMode2D.Impulse);
            kick = false;

            kickForce = Vector2.zero;
        }
    }

    void OnCollisionStay2D(Collision2D other)
    {
        if(other.gameObject.tag == "Floor")
        {
            kicksLeft = maxNumKicks;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "Feet" && other.gameObject.name.CompareTo("Feet"+playerNum) != 0)
        {
            Debug.Log(playerNum + " died");
        }
    }


}
