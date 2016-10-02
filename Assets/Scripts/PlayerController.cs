using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public int playerNum = 0;

    private Rigidbody2D playerRB;
    public float movementSpeed = 4f;

    private Vector2 kickForce;
    private bool timeToKick;
    private bool kicking;
    private float chargeKickTimer;
    public float chargeLength;

    public float maxKick;
    public float minKick;

    public int maxNumKicks = 3;
    private int kicksLeft;

    private float kickDuration;

    public float maxForce;

    public int livesLeft = 5;

    private GameObject feet;
    private ParticleSystem chargePartSys;
    private ParticleSystem trailPartSys;

    private GameManager gm;

	// Use this for initialization
	void Start () {
        playerRB = GetComponent<Rigidbody2D>();
        chargePartSys = transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
        trailPartSys = GetComponent<ParticleSystem>();

        kicksLeft = maxNumKicks;

        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    public void setPlayerNum(int i)
    {
        playerNum = i;
        feet = GameObject.Find("Feet" + i);
    }
	
	// Update is called once per frame
	void Update () {

        //Get input from left stick and move position's x axis accordingly
       // transform.position += new Vector3(Input.GetAxis("Left Stick X Axis P" + playerNum) * movementSpeed*Time.deltaTime,0,0);
        

        Vector2 rightStick = new Vector2(Input.GetAxis("Right Stick X Axis P" + playerNum), Input.GetAxis("Right Stick Y Axis P" + playerNum));

        if (rightStick.magnitude > 0.75)
        {  
            kickForce = rightStick.normalized;

            if (chargeKickTimer < chargeLength)
            {
                chargeKickTimer += Time.deltaTime;
            }

        }
        else if(kickForce.sqrMagnitude > 0 && !timeToKick)
        {
            float kickMagnitude = (chargeKickTimer / chargeLength) * maxKick;

            if (kickMagnitude < minKick)
                kickMagnitude = minKick;

            kickDuration = chargeKickTimer;

            kickForce *= kickMagnitude;
            chargeKickTimer = 0;

            timeToKick = true;
            kicking = true;

            kicksLeft--;
            
        }

        if(transform.position.y < -7)
        {
            transform.position = new Vector3(0, -3, 0);
        }


        if (feet)
        {
            feet.transform.position = transform.position;

            BoxCollider2D footCollider = feet.GetComponent<BoxCollider2D>();
            if (kickDuration > 0 && playerRB.velocity.sqrMagnitude > 0)
            {
                trailPartSys.enableEmission = true;


                kickDuration -= Time.deltaTime;
                footCollider.enabled = true;
                footCollider.offset = playerRB.velocity.normalized * .5f;
            }
            else
            {
                trailPartSys.enableEmission = false;

                footCollider.enabled = false;
                kicking = false;
            }
        }

        //This is apparently deprecated now but how the hell else do you set emission rate when emission.rate is read-only       
        chargePartSys.emissionRate = (40*chargeKickTimer / chargeLength);

        if (chargeKickTimer >= chargeLength)
            chargePartSys.startColor = new Color(1, 0.5f, 0.25f, 1);

        else
            chargePartSys.startColor = Color.yellow;


    }

    //Fixed update for physics stuff
    void FixedUpdate()
    {

        playerRB.AddForce(new Vector2(Input.GetAxis("Left Stick X Axis P" + playerNum) * (Mathf.Abs(playerRB.velocity.magnitude) > 1? movementSpeed:movementSpeed*10), 0));

        if (timeToKick && kicksLeft > 0)
        {
            playerRB.velocity = Vector2.zero;

            playerRB.AddForce(Vector2.ClampMagnitude(kickForce, maxKick),ForceMode2D.Impulse);
            timeToKick = false;

            kickForce = Vector2.zero;
        }

        playerRB.velocity = Vector2.ClampMagnitude(playerRB.velocity,maxForce);

    }

    void Die()
    {
        transform.position = new Vector3(0,5,0);
        playerRB.velocity = Vector2.zero;

        kicksLeft = maxNumKicks;
        kicking = false;

        livesLeft--;

    }


    void OnCollisionStay2D(Collision2D other)
    {
        if(other.gameObject.tag == "Floor")
        {
            kicksLeft = maxNumKicks;
            kicking = false;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //If you collide with feet and they're not your own, you died
        if(other.gameObject.tag == "Feet" && other.gameObject.name != feet.name)
        {
            gm.killPlayer(playerNum-1);
        }
    }


}
