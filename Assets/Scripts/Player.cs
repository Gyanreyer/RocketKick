using UnityEngine;
using UnityEngine.UI;

//Basic class for players that stores all relevant info about a given player - GameManager has an array of these
public class Player : MonoBehaviour
{

    private int score;//Score for number of rounds won
    private GameObject mainObject;//GameObject for player's body
    private PlayerController controller;//Controller script for player
    private bool alive;//Whether player is alive and accessible

    private int index;//Index for player

    private GameObject prefab;//Prefab to spawn instances of
    private Material skin;//Material for player

    private Text scoreText;//UI text for this player's score
    private Color textColor;//Color for this player's text

    public float vibrationPower;//Power to vibrate controller with

    //Properties
    public int Score { get { return score; } }
    public GameObject GO { get { return mainObject; } }
    public PlayerController Controller { get { return controller; } }
    public bool Alive { get { return alive; } }
    public int Index { get { return index; } }
    public int PlayerNum { get { return index + 1; } }
    public Vector3 Position { get { return controller.transform.position; } }

    //Constructor sets things up
    public Player(int ind, GameObject pb, Material mat, Color c)
    {
        index = ind;
        prefab = pb;

        skin = mat;
        textColor = c;

        score = 0;

        alive = false;
    }

    //Win round by adding to score and then destroy after 2 seconds
    public void WinRound()
    {
        score++;

        scoreText.text = score.ToString();

        Die(2);
    }

    //Spawn a new player given 
    public void SpawnNewPlayer(Vector3 spawnPos)
    {
        scoreText = GameObject.Find("P" + PlayerNum + "Text").GetComponent<Text>();//Get text for keeping score

        //Make score visible if not
        if (scoreText.color.a == 0)
        {
            scoreText.color = new Color(scoreText.color.r, scoreText.color.g, scoreText.color.b, 1f);
        }
        
        //Fade score text to full color
        scoreText.CrossFadeColor(textColor, .1f, true, true);    

        //Store main object by instantiating prefab
        mainObject = (GameObject)Instantiate(prefab, spawnPos, Quaternion.identity);
        mainObject.name = "P" + PlayerNum;

        //Set appropriate skin material for player body
        mainObject.transform.FindChild("Player").GetComponent<MeshRenderer>().material = skin;

        //Update children's names to include player num
        for (int i = 0; i < mainObject.transform.childCount; i++)
        {
            mainObject.transform.GetChild(i).name += PlayerNum;
        }

        //Store controller
        controller = mainObject.GetComponentInChildren<PlayerController>();
        controller.SetNum(index);

        alive = true;
    }

    //Die immediately and fade out score text partially
    public void Die()
    {
        Die(0);

        scoreText.CrossFadeColor(new Color(textColor.r*.5f,textColor.g*.5f,textColor.b*.5f,.75f), .1f, true, true);
    }

    //Destroy object with given delay
    public void Die(float delay)
    {
        Destroy(mainObject, delay);

        alive = false;
    }

    //Reset the score
    public void resetScore()
    {
        score = 0;
    }

}