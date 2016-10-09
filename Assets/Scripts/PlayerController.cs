﻿using UnityEngine;
using System.Collections;
using XInputDotNetPure;

public class PlayerController : MonoBehaviour {

    public int playerNum;

    public AudioClip[] sfx;
    private AudioSource audio;

    //Player rigidbody for movement/forces/body collision
    private Rigidbody2D playerRB; 

    public float movementForce;//Magnitude of force applied for moving w/ left stick
    public float maxSpeed;//Maximum magnitude for velocity
    public float walkSpeed;
        
    //Kicking stuff
    private Vector2 kickForce;//Vector for kick force to apply
    private bool timeToKick;//True if should apply force to kick, false if not
    private float chargeKickTimer;//Timer for how long kick has been charged     
    private int kicksLeft;//Keeps track of number of kicks player has left before have to reset
    private float kickDuration;//Duration of time that player will remain in kick state after kick - starts at length of charge and counts down to 0

    private Vector2 lastKickDirection;//Use to keep track of direction of last kick, so if start moving in direction significantly different then stop kicking

    public float vibrationPower;//Power of vibration


    public int maxNumKicks = 3;//Max number of kicks before have to touch ground again and reset
    public float chargeLength;//Time to reach max charge
    public float maxKick;//Max magnitude of kick force (when fully charged)
    public float minKick;//Min magnitude of kick force (when quickly tap)

    public GameObject feet;//GameObject for this player's feet, collider on feet is used to determine whether a kick is a kill
    private GameObject directionIndicator;//GameObject for direction indicator that shows what direction aiming in

    private ParticleSystem chargePartSys;//Particle system for charging up kick
    private ParticleSystem trailPartSys;//Particle system for trail while kicking

    private GameManager gm;//GameManager for game

    private bool deflecting;//Whether player is deflecting off other player, this is necessary because otherwise one player can change direction
                            //before other and then other player gets registered as kicking them in the back

    private bool inAir;//Whether in the air or not
    private bool kicking;//Whether kicking or not, mostly just used for external access

    private GamePadState gpState;//State of gamepad each frame, used to get input
    private GamePadState prevGpState;

    public PlayerIndex index;//Controller index of player


    public bool Kicking { get { return kicking; } }
    public float CurrentSpeed { get { return playerRB.velocity.magnitude; } }

    private Sprite sprite;
    private SpriteRenderer spriteRen;
    private Animator animator; //added for sprites
    private int spriteState; //0 for idle, 1 for run, 2 for kick
    private bool flipped;

    //Set player's number and index
    public void SetNum(int ind)
    {
        index = (PlayerIndex)ind;
        playerNum = ind+1;
    }

	// Use this for initialization
	void Start () {

        //Hook up all the objects/components need to keep track of
        playerRB = GetComponent<Rigidbody2D>();
        chargePartSys = transform.GetChild(0).gameObject.GetComponent<ParticleSystem>();
        trailPartSys = GetComponent<ParticleSystem>();
        audio = GetComponent<AudioSource>();
        sprite = GetComponent<Sprite>();
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
        directionIndicator = transform.FindChild("DirectionIndicator").gameObject;
        directionIndicator.SetActive(false);

        Physics2D.IgnoreCollision(GetComponent<BoxCollider2D>(),feet.GetComponent<BoxCollider2D>());//Ignores physics collisions between feet and player bodies so they can intersect

        kicksLeft = maxNumKicks;//Initialize kicksLeft to default

        animator = GetComponentInChildren<Animator>();
        spriteRen = GetComponentInChildren<SpriteRenderer>();
        Debug.Log(animator.ToString());

    }
	
	// Update is called once per frame
	void Update () {

        //If you fall off map, die
        if (transform.position.y < -7)
        {
            Die();
            return;
        }

        prevGpState = gpState;//Store gamepad state from previous frame
        gpState = GamePad.GetState(index);//Get current state of gamepad

        ChargeKick();///Get input from right stick to charge kick  

        UpdateKickStatus();//Update whether kicking or not

        //Update layer that player is on so can jump through bottom of platforms - the way this works right now isn't super great, consider tweaks
        if (playerRB.velocity.y > 0)
        {
            gameObject.layer = 9;
        }
        else
        {
            gameObject.layer = 8;
        }

        GamePad.SetVibration(index, vibrationPower, vibrationPower);//Set vibration on controller - THIS WILL KEEP GOING FOREVER UNLESS SET BACK TO 0

        //Decrease controller vibration power over time
        if (vibrationPower > 0)
            vibrationPower -= 2*Time.deltaTime;
        else
            vibrationPower = 0;

        ControlAnimations();

        
    }


    //Fixed update for physics stuff
    void FixedUpdate()
    {
        //Magnitude for movement force, get slight boost if player is stationary to get them moving
        float moveForceMag = movementForce * (playerRB.velocity.magnitude > 1 ? 1f : 10f);

        //Add force for left stick movement - I really wish walking could be less floaty, we could try creating our own force/velocity/etc variables but that's something for later
        playerRB.AddForce(new Vector2(gpState.ThumbSticks.Left.X * moveForceMag, 0));


        //If it's time to kick, apply kick force
        if (timeToKick && kicksLeft > 0)
        {
            playerRB.velocity = Vector2.zero;//Stop current velocity so direction change will be obvious

            playerRB.AddForce(kickForce,ForceMode2D.Impulse);//Add kick force

            timeToKick = false;//No longer time to kick
            kickForce = Vector2.zero;//Reset kick force vector

            if(inAir)
                kicksLeft--;
        }

                
        //Clamp velocity to walk speed
        playerRB.velocity = Vector2.ClampMagnitude(playerRB.velocity, kicksLeft < maxNumKicks || kickDuration > 0 ? maxSpeed:walkSpeed);

        //Keep feet at same position as body, will offset its collider based on kick direction
        feet.transform.position = transform.position;
    }


    //Get input from right stick to charge kick
    void ChargeKick()
    {
        //If no kicks left, reset stuff and return early
        if (kicksLeft <= 0)
        {
            kickForce = Vector2.zero;
            chargeKickTimer = 0;

            return;
        }

        Vector2 rightStick = new Vector2(gpState.ThumbSticks.Right.X, gpState.ThumbSticks.Right.Y);//Store x and y of right thumbstick for easy access

        //Determine whether input means should be charging - trigger stuff is probably temporary, I'm not a fan
        if (rightStick.magnitude > 0.75)
        {
            kickForce = rightStick.normalized;//Set direction of kick force as normalized vector of right stick

            //Increase charge if not at max
            if (chargeKickTimer < chargeLength)
            {
                chargeKickTimer += Time.deltaTime;

                if(!audio.isPlaying) audio.PlayOneShot(sfx[0], .3f);
            }

            //If the kick force has a direction, draw the direction indicator to visualize it
            if (rightStick.sqrMagnitude > 0)
            {
                directionIndicator.SetActive(true);
                directionIndicator.transform.eulerAngles = new Vector3(0, 0, (Mathf.Rad2Deg * Mathf.Atan2(rightStick.y, rightStick.x)) - 90);
            }

            vibrationPower = .1f + .1f * chargeKickTimer / chargeLength;//How hard controller should vibrate, based on how high charged up


            if (chargeKickTimer > chargeLength)
            {
                vibrationPower += .1f;
            }

            

        }
        //If not getting charge input and kickforce hasn't been reset, that means it's been released and it's time to kick!
        else if (kickForce.sqrMagnitude > 0 && !timeToKick)
        {
            lastKickDirection = kickForce;//Store initial normalized direction of kick

            //Multiply directional kickForce by magnitude to get final force vector to apply, mag is based on how long charged and clamped to minKick if too low
            kickForce *= Mathf.Clamp((chargeKickTimer / chargeLength) * maxKick, minKick, maxKick);

            kickDuration = chargeKickTimer;//Set kick duration to last as long as kick was charged up, this will count down to 0

            timeToKick = true;//Indicate that it's time to apply kick force

            directionIndicator.SetActive(false);//Hide direction indicator

            chargeKickTimer = 0;//Reset charge kick timer

            AudioSource audio = GetComponent<AudioSource>();

            audio.Stop();
            audio.PlayOneShot(sfx[1]);

        }


        //Set charge particle system emission rate based on charge - apparently C# has vars and this is the new way to set emission rate, not gonna question
        var emission = chargePartSys.emission;
        var rate = emission.rate;

        if (chargeKickTimer > 0)
        {
            if (chargeKickTimer < chargeLength)
            {
                //Set color, speed, and size of particles based on charge
                chargePartSys.startColor = new Color(1, 1f / chargeKickTimer, 0.25f);
                chargePartSys.startSpeed = 1 + (chargeKickTimer / 2f);
                chargePartSys.startSize = .1f + .1f * (chargeKickTimer / chargeLength);
            }
            //Visually indicate when reached full charge w/ color and size change
            else
            {
                chargePartSys.startColor = new Color(1, 0.25f, .25f);
                chargePartSys.startSize = .3f;
            }

            rate.constantMax = (60 * chargeKickTimer / chargeLength);
        }
        else
            rate.constantMax = 0;

        emission.rate = rate;//Set rate        
    }

    void UpdateKickStatus()
    {
        BoxCollider2D footCollider = feet.GetComponent<BoxCollider2D>();//Get collider on feet

        var em = trailPartSys.emission;

        //If kick is still in progress and player is still generally moving in same direction as initial kick, reflect that
        if (kickDuration > 0 && playerRB.velocity.sqrMagnitude > 0 && Vector2.Dot(playerRB.velocity.normalized, lastKickDirection) > .5f)
        {
            kicking = true;//We are kicking

            //Enable foot collider and trail particles
            footCollider.enabled = true;

            //Enable trail particles emission
            em.enabled = true;

            kickDuration -= Time.deltaTime;//Decrement kick duration
            footCollider.offset = playerRB.velocity.normalized * .5f;//Offset foot collider for kicking people with

        }
        //Otherwise disable kick and hide relevant stuff
        else
        {
            kicking = false;//No longer kicking

            //Disable trail particles emission
            em.enabled = false;

            //Disable foot collider
            footCollider.offset = Vector2.zero;
            footCollider.enabled = false;
        }

    }


    //Kill this player
    public void Die()
    {
        gm.killPlayer(playerNum);//Kill this player via the game manager
    }
    

    //Called when other collider hits this one
    void OnCollisionEnter2D(Collision2D other)
    {
        //If collided with other player's feet, they have kicked you, determine if you die or deflect
        if(other.gameObject.tag == "Feet")
        {
            if (deflecting) return;//Return early if already marked as deflecting

            
            GameObject otherPlayerBody = other.gameObject.GetComponent<Feet>().playerBody;//Get other player's GO from Feet script            

            PlayerController otherController = otherPlayerBody.GetComponent<PlayerController>();

            vibrationPower = .75f* Mathf.Max(playerRB.velocity.magnitude,otherController.playerRB.velocity.magnitude) / maxSpeed;//Set power of vibration based on how fast fastest of the two was moving
            otherController.vibrationPower = vibrationPower;

            GameObject.Find("EffectsManager").GetComponent<EffectsManager>().Shake(vibrationPower / 4, .3f);
            

            //Check if should deflect
            if (CheckDeflect(otherController.playerRB))
            {
                Deflect();
                otherController.Deflect();
            }
            //If not deflecting, you're dead
            else
            {
                Die();
            }
        }
        //Reset stuff if landed on floor or wall
        else if (other.gameObject.tag == "Floor" || other.gameObject.tag == "Wall")
        {
            inAir = false;
            kicksLeft = maxNumKicks-1;
        }

    }

    //Called when collider stops colliding with you
    void OnCollisionExit2D(Collision2D other)
    {
        //If another player's feet have exited that means you're all good on deflecting, reset it to false
        if(other.gameObject.tag == "Feet")
        {
            deflecting = false;
        }
        //If you left the ground or wall. you're in the air
        else if(other.gameObject.tag == "Floor" || other.gameObject.tag == "Wall")
        {
            inAir = true;
        }
    }

    //Returns whether should deflect or not
    public bool CheckDeflect(Rigidbody2D otherPlayer)
    {
        //Returns true if velocities are significantly in opposite directions (1 = same dir, 0 = perp, -1 = opposite)
        return Vector2.Dot(playerRB.velocity.normalized, otherPlayer.velocity.normalized) < -0.2f;
    }

    //Deflect this player
    public void Deflect()
    {
        playerRB.velocity *= -.8f;//Mirror velocity w/ 20% speed loss
        deflecting = true;//Set deflecting to true so no accidental killings after velocities are flipped
        lastKickDirection *= -1;//Mirror last kick direction so keep kicking

        
    }

    //Hnadles the state changing of the animations
    private void ChangeAnimationState()
    {
        
        Debug.Log(animator.ToString());
        if(spriteState == 0)
        {
            animator.SetInteger("State", 0);
        }
        if(spriteState == 1)
        {
            animator.SetInteger("State", 1);
        }
        if(spriteState == 2)
        {
            animator.SetInteger("State", 2);
        }
    }

    //This guy takes the velocity and turns it into animations
    private void ControlAnimations()
    {
        //state 0: idle, 1: run, 2: kick
        //control animations


        if (playerRB.velocity == Vector2.zero)
        {
            spriteState = 0;
        }
        if (playerRB.velocity.x > 0)
        {
            //if flipped re flip to the right direction
            if (flipped)
            {
                spriteRen.flipX = false;
                flipped = false;
            }

            spriteState = 1;
            
        }
        if (playerRB.velocity.x < 0)
        {
            spriteState = 1;

            //if not flipped
            if (!flipped)
            {
                spriteRen.flipX = true;
                flipped = true;
            }
        }

        if (kicking)
            spriteState = 2;

        //call for state change on animator
        ChangeAnimationState();
    }

    private void PickSprite()
    {
        spriteRen.sprite.Equals(gm.sprites[playerNum - 1]);
    }
}
