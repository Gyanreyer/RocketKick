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

    public GameObject feet;
    private ParticleSystem chargePartSys;
    private ParticleSystem trailPartSys;
    private GameObject directionIndicator;

    private GameManager gm;

    public bool deflecting;

	// Use this for initialization
	void Start () {
        playerRB = GetComponent<Rigidbody2D>();
        chargePartSys = transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
        trailPartSys = GetComponent<ParticleSystem>();

        kicksLeft = maxNumKicks;

        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        directionIndicator = transform.FindChild("DirectionIndicator").gameObject;
        directionIndicator.SetActive(false);

        Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(),feet.GetComponent<BoxCollider2D>());
    }
	
	// Update is called once per frame
	void Update () {

        //Get input from left stick and move position's x axis accordingly       

        Vector2 rightStick = new Vector2(Input.GetAxis("Right Stick X Axis P" + playerNum), Input.GetAxis("Right Stick Y Axis P" + playerNum));

        if (rightStick.magnitude > 0.75 && kicksLeft > 0)
        {  
            kickForce = rightStick.normalized;

            if (chargeKickTimer < chargeLength)
            {
                chargeKickTimer += Time.deltaTime;
            }

            directionIndicator.SetActive(true);
            directionIndicator.transform.eulerAngles = new Vector3(0, 0, (Mathf.Rad2Deg * Mathf.Atan2(rightStick.y,rightStick.x))-90);
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

            directionIndicator.SetActive(false);

        }

        if(transform.position.y < -7)
        {
            transform.position = new Vector3(0, -3, 0);
        }



        feet.transform.position = transform.position;

        BoxCollider2D footCollider = feet.GetComponent<BoxCollider2D>();
        if (kickDuration > 0 && playerRB.velocity.sqrMagnitude > 0)
        {
            trailPartSys.enableEmission = true;


            kickDuration -= Time.deltaTime;
            footCollider.offset = playerRB.velocity.normalized * .5f;
        }
        else
        {
            trailPartSys.enableEmission = false;

            footCollider.offset = Vector2.zero;
            kicking = false;

        }

        //This is apparently deprecated now but how the hell else do you set emission rate when emission.rate is read-only       
        chargePartSys.emissionRate = (60*chargeKickTimer / chargeLength);

        chargePartSys.startColor = new Color(1, 1f / (chargeKickTimer * 2), 0.25f, 1);
        chargePartSys.startSpeed = 1 + (chargeKickTimer / 2f);
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

    public void Die()
    {
        gm.killPlayer(playerNum - 1);

    }


    void OnCollisionStay2D(Collision2D other)
    {
        if(other.gameObject.tag == "Floor")
        {
            kicksLeft = maxNumKicks;
        }
    }

    void OnCollisionEnter2D(Collision2D other)
    {
        if(other.gameObject.tag == "Feet")
        {
            //Checks if should reflect, if not then just die
            if (CheckDeflect(other.gameObject.GetComponent<Feet>().playerBody))
            {
                playerRB.velocity *= -.8f;
                other.gameObject.GetComponent<Rigidbody2D>().velocity *= -.8f;
                deflecting = true;
            }
            else
            {
                Die();
            }
        }
    }

    void OnCollisionExit2D(Collision2D other)
    {
        if(other.gameObject.tag == "Feet")
        {
            deflecting = false;
        }
    }

    public bool CheckDeflect(GameObject otherPlayer)
    {
        //Returns true if velocities are significantly in opposite directions or other player is already deflecting
        return Vector2.Dot(playerRB.velocity.normalized, otherPlayer.GetComponent<Rigidbody2D>().velocity.normalized) < -0.3f ||
            otherPlayer.GetComponent<PlayerController>().deflecting;
    }

}
