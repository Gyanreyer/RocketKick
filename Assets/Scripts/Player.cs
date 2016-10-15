using UnityEngine;
using UnityEngine.UI;
using XInputDotNetPure;

//Basic class for players that stores all relevant info about a given player - GameManager has an array of these
public class Player
{
    public bool ready;


    private int score;//Score for number of rounds won
    public GameObject mainObject;//GameObject for player's body
    private PlayerController controller;//Controller script for player
    private bool alive;//Whether player is alive and accessible

    private int index;//Index for player

    private GameObject prefab;//Prefab to spawn instances of

    private Text scoreText;//UI text for this player's score
    private Color color;//Color for this player's text

    public float vibrationPower;//Power to vibrate controller with

    //Properties
    public int Score { get { return score; } }
    public PlayerController Controller { get { return controller; } }
    public bool Alive { get { return alive; } }
    public int Index { get { return index; } }
    public int PlayerNum { get { return index + 1; } }
    public Vector3 Position { get { return controller.transform.position; } }
    public Vector3 LocalPosition { get { return controller.transform.localPosition; } }
    public Color Color { get { return color; } }

    //Constructor sets things up
    public Player(int ind, GameObject prfb, Color c)
    {
        index = ind;
        prefab = prfb;

        color = c;

        score = 0;

        alive = false;
        ready = false;
    }

    //Win round by adding to score and then destroy after 2 seconds
    public void WinRound()
    {
        score++;

        scoreText.text = score.ToString();
    }

    //Spawn a new player given 
    public void SetupPlayer()
    {
        scoreText = GameObject.Find("P" + PlayerNum + "Text").GetComponent<Text>();//Get text for keeping score

        //Make score visible if not
        if (scoreText.color.a == 0)
        {
            scoreText.color = new Color(scoreText.color.r, scoreText.color.g, scoreText.color.b, 1f);
        }
        
        //Fade score text to full color
        scoreText.CrossFadeColor(color, .1f, true, true);    

        //Store main object by instantiating prefab
       // mainObject = (GameObject)Instantiate(prefab, spawnPos, Quaternion.identity);


        //Store controller
        controller = mainObject.GetComponentInChildren<PlayerController>();
        controller.index = (PlayerIndex)index;
        controller.playerNum = index + 1;

        mainObject.GetComponent<SpriteRenderer>().color = color;

        alive = true;
    }

    //Die immediately and fade out score text partially
    public void DieBeforeEnd()
    {
        scoreText.CrossFadeColor(new Color(color.r*.5f,color.g*.5f,color.b*.5f,.75f), .1f, true, true);

        Die();
    }

    //Destroy object with given delay
    public void Die()
    {
        mainObject = null;

        alive = false;
    }

    //Reset the score
    public void resetScore()
    {
        score = 0;
    }

    public void ChangeColor(Color c)
    {
        color = c;
    }

}