using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using XInputDotNetPure;

public class GameManager : MonoBehaviour {

    public GameObject playerPrefab;

    bool[] players = new bool[4];
    public int playerCount = 0;

    private int playersAlive = 0;

    GameObject promptText;

    public Material[] skins = new Material[4];

    private GameObject[] activePlayers = new GameObject[4];
    int[] playerScores = new int[4];

    public GameObject[] spawnPoints;    //assigned in the FindSpawnpoints() method

    // Initialization, executes before Start()
    void Awake () {
        //If there's already a GM in the scene, destroy this one so no duplicates
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);//Don't destroy this object, it'll persist between scenes

        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            promptText = GameObject.Find("StartPrompt");
            promptText.SetActive(false);
        }

    }

    // Update is called once per frame
    void Update()
    {
        //If on main menu, get active controllers
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {

            for (int i = 0; i < 4; i++)
            {
                if (Input.GetButtonDown("Start Button P" + (i + 1)) && !players[i])
                {
                    players[i] = true;
                    playerCount++;

                    GameObject.Find("PlayerStatus" + (i + 1)).GetComponent<Text>().text = "Player " + (i + 1) + "\nReady!";
                }

                if (playerCount > 1 && Input.GetButtonDown("A Button P" + (i + 1)))
                {
                    loadScene(1);
                }
            }
            
            //When more than one person is ready, you can start game
            if (playerCount > 1)
            {
                promptText.SetActive(true);
            }
        }
        
	}

    public void loadScene(int i)
    {
        SceneManager.LoadScene(i);
    }


    public void spawnPlayers()
    {
        //Gather spawnpoints
        PopulateSpawnpoints();

        //Array to tell which spawnpoints are occupied
        bool[] spawnsUsed = new bool[spawnPoints.Length];

        int spawnIndex = 0;

        playersAlive = 0;

        for(int i = 0; i < 4; i++)
        {
            if(players[i])
            {
                int number = i + 1;

                GameObject p = Instantiate(playerPrefab);
                p.name = "P" + number;
                p.transform.FindChild("Feet").name = "Feet"+number;

                //Finding a good spawnpoint
                spawnIndex = (int)Random.Range(0, spawnPoints.Length - 1);//used to get a random spawnpoint... we can change this later if you guys don't like random spawns
                do
                {
                    ++spawnIndex;
                    if (spawnIndex >= spawnPoints.Length)
                        spawnIndex = 0;
                } while (spawnsUsed[spawnIndex]);
                //Set spawn to used
                spawnsUsed[spawnIndex] = true;
                //Udpate position based on that spawnpoint
                p.transform.position = spawnPoints[spawnIndex].transform.position;

                activePlayers[i] = p.transform.FindChild("Player").gameObject;
                activePlayers[i].name = "Player" + number;
                activePlayers[i].GetComponent<MeshRenderer>().material = skins[i];
                activePlayers[i].GetComponent<PlayerController>().playerNum = number;

                GameObject.Find("P" + number + "Text").GetComponent<Text>().color = new Color(255.0f, 255.0f, 255.0f, 255.0f);
                GameObject.Find("P" + number + "Text").GetComponent<Text>().text = "Player " + number + " : Alive!";

                playersAlive++;
            }
        }
    }

    public void killPlayer(int number)
    {
        playersAlive--;

        GameObject.Find("P" + number + "Text").GetComponent<Text>().text = "Player " + number + " : Eliminated!";


        Destroy(activePlayers[number-1].transform.parent.gameObject);
        activePlayers[number-1] = null;

        //If only one player left, increase that player's score and then respawn for next round
        if (playersAlive == 1)
        {
            for (int i = 0; i < 4; i++)
            {
                if (activePlayers[i])
                {
                    GameObject.Find("P" + (i+1) + "Text").GetComponent<Text>().text = "Player " + (i+1) + " : Winner!";
                    playerScores[i]++;
                    Destroy(activePlayers[i].transform.parent.gameObject,1);
                    activePlayers[i] = null;
                    break;
                }
            }

            Invoke("spawnPlayers",1.5f);
        }
        
    }


    void OnLevelWasLoaded(int levelIndex)
    {
        if(levelIndex != 0)
        {
            spawnPlayers();
        }
    }

    /// <summary>
    /// Populates the spawnPoints array, should be called right before spawning players (I call it in the spawnPlayers() method)
    /// </summary>
    void PopulateSpawnpoints()
    {
        spawnPoints = GameObject.FindGameObjectsWithTag("Spawn Point");
    }
}
