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

    private Text scoreText;
    private Color textColor;

    public float vibrationPower;

    //Properties
    public int Score { get { return score; } }
    public GameObject GO { get { return mainObject; } }
    public PlayerController Controller { get { return controller; } }
    public bool Alive { get { return alive; } }
    public int Index { get { return index; } }
    public int PlayerNum { get { return index + 1; } }

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

    //Win round by adding to score and then dying on delay
    public void WinRound()
    {
        score++;

        scoreText.text = score.ToString();

        Die(2);
    }

    //Spawn a new player given 
    public void SpawnNewPlayer(Vector3 spawnPos)
    {
        scoreText = GameObject.Find("P" + PlayerNum + "Text").GetComponent<Text>();

        if (scoreText.color.a == 0)
        {
            scoreText.color = new Color(scoreText.color.r, scoreText.color.g, scoreText.color.b, 1f);
        }
        
        scoreText.CrossFadeColor(textColor, .1f, true, true);    

        mainObject = (GameObject)Instantiate(prefab, spawnPos, Quaternion.identity);
        mainObject.name = "P" + PlayerNum;

        mainObject.transform.FindChild("Player").GetComponent<MeshRenderer>().material = skin;

        for (int i = 0; i < mainObject.transform.childCount; i++)
        {
            mainObject.transform.GetChild(i).name += PlayerNum;
        }

        controller = mainObject.GetComponentInChildren<PlayerController>();
        controller.SetNum(index);

        alive = true;
    }

    public void Die()
    {
        Die(0);

        scoreText.CrossFadeColor(new Color(textColor.r*.5f,textColor.g*.5f,textColor.b*.5f,.75f), .1f, true, true);
    }

    public void Die(float delay)
    {
        Destroy(mainObject, delay);

        alive = false;
    }

    public void resetScore()
    {
        this.score = 0;
    }

}