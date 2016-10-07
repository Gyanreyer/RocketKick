using UnityEngine;
using XInputDotNetPure;

//Basic class for players that stores all relevant info about a given player - GameManager has an array of these
public class Player : MonoBehaviour {

    private int score;//Score for number of rounds won
    private GameObject mainObject;//GameObject for player's body
    private PlayerController controller;//Controller script for player
    private bool alive;//Whether player is alive and accessible

    private int index;//Index for player

    private GameObject prefab;//Prefab to spawn instances of
    private Material skin;//Material for player

    //Properties
    public int Score { get { return score; } }
    public GameObject GO { get { return mainObject; } }
    public PlayerController Controller { get { return controller; } }
    public bool Alive { get { return alive; } }
    public int Index { get { return index; } }
    public int PlayerNum { get { return index + 1; } }

    //Constructor sets things up
    public Player(int ind, GameObject pb, Material mat)
    {
        index = ind;
        prefab = pb;

        skin = mat;

        score = 0;

        alive = false;
    }

    //Win round by adding to score and then dying on delay
    public void WinRound()
    {
        score++;
        Die(2);
    }

    //Spawn a new player given 
    public void SpawnNewPlayer(Vector3 spawnPos)
    {
        mainObject = (GameObject)Instantiate(prefab, spawnPos, Quaternion.identity);
        mainObject.name = "P"+PlayerNum;  

        mainObject.transform.FindChild("Player").GetComponent<MeshRenderer>().material = skin;

        for(int i = 0; i < mainObject.transform.childCount; i++)
        {
            mainObject.transform.GetChild(i).name += PlayerNum;
        }

        controller = mainObject.GetComponentInChildren<PlayerController>();
        controller.SetNum(index);

        alive = true;
    }

    public void Die()
    {
        Destroy(mainObject);

        alive = false;
    }
    
    public void Die(float delay)
    {
        Destroy(mainObject, delay);

        alive = false;
    }

}
