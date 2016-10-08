using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using XInputDotNetPure;

public class GameManager : MonoBehaviour
{

    public GameObject playerPrefab;

    //Dictionary<int,Player> players;
    List<Player> players;

    private int playersAlive = 0;
    public int MAX_SCORE = 5;

    GameObject promptText;

    public Material[] skins = new Material[4];

    public Color[] textColors = new Color[4];

    public GameObject[] spawnPoints;//assigned in the FindSpawnpoints() method

    // Initialization, executes before Start()
    void Awake()
    {
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

        players = new List<Player>(4);
    }

    // Update is called once per frame
    void Update()
    {
        //If on main menu, get active controllers
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            GamePadState gpState;

            if (Input.GetKeyDown(KeyCode.R) && !players.Exists(p => p.Index == 1))
            {
                players.Add(new Player(1, playerPrefab, skins[1],textColors[1]));

                GameObject.Find("PlayerStatus" + 2).GetComponent<Text>().text = "Player " + 2 + "\nReady!";
            }

            if (players.Count > 1 && Input.GetButtonDown("A Button"))
            {
                loadScene(2);
                ResetPlayerScores();
            }

            for (int i = 0; i < 4; i++)
            {
                gpState = GamePad.GetState((PlayerIndex)i);

                if (gpState.Buttons.Start == ButtonState.Pressed && !players.Exists(p => p.Index == i))
                {

                    players.Add(new Player(i, playerPrefab, skins[i],textColors[i]));

                    GameObject.Find("PlayerStatus" + (i + 1)).GetComponent<Text>().text = "Player " + (i + 1) + "\nReady!";
                }


            }

            //When more than one person is ready, you can start game
            if (players.Count > 1)
            {
                promptText.SetActive(true);
            }
        }

    }

    public void ResetPlayerScores()
    {
        foreach(Player p in players)
        {
            p.resetScore();
        }
    }

    /// <summary>
    /// Used for the dynamic camera to get the locations of the player objects
    /// </summary>
    /// <returns></returns>
    /*public GameObject[] getActivePlayers()
    {
        return activePlayers;
    }*/

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

        for (int i = 0; i < players.Count; i++)
        {
            //Finding a good spawnpoint
            spawnIndex = (int)Random.Range(0, spawnPoints.Length - 1);//used to get a random spawnpoint... we can change this later if you guys don't like random spawns
            do
            {
                if (++spawnIndex >= spawnPoints.Length)
                    spawnIndex = 0;
            } while (spawnsUsed[spawnIndex]);
            //Set spawn to used
            spawnsUsed[spawnIndex] = true;

            int pNum = players[i].PlayerNum;

            players[i].SpawnNewPlayer(spawnPoints[spawnIndex].transform.position);

        }

        //GiveAlivePlayersToCamera();

        playersAlive = players.Count;
    }

    public void killPlayer(int number)
    {
        players.Find(p => p.PlayerNum == number).Die();

        playersAlive--;

        //If only one player left, increase that player's score and then respawn for next round
        if (playersAlive <= 1)
        {
            for (int i = 0; i < players.Count; i++)
            {
                if (players[i].Alive)
                {
                    GameObject.Find("P" + (i + 1) + "Text").GetComponent<Text>().text = "Player " + (i + 1) + " : Winner!";
                    players[i].WinRound();

                    if(players[i].Score >= MAX_SCORE)
                    {
                        //Resolve win condition
                        //SceneManager.LoadScene(1);//Load game over scene
                        //return;
                    }
                }
            }

            Invoke("spawnPlayers", 2.1f);
        }
    }

    /// <summary>
    /// Gives the camera an array of alive players
    /// </summary>
    /*void GiveAlivePlayersToCamera()
    {
        if (playersAlive == 0)
            return; //this here is to catch the whole "OnLevelWasLoaded" getting called twice thing
                    //more info here:http://answers.unity3d.com/questions/466880/is-onlevelwasloaded-supposed-to-be-called-twice-wh.html
        GameObject[] alivePlayerObjects = new GameObject[playersAlive];
        int alivePlayerCounter = 0;
        for (int i = 0; i < activePlayers.Length; ++i)
        {
            if (activePlayers[i] != null) 
            {
                alivePlayerObjects[alivePlayerCounter] = activePlayers[i];
                ++alivePlayerCounter;
            }
        }
        Camera.main.GetComponent<dynamicCamera>().SetAlivePlayers(alivePlayerObjects);
    }*/


    void OnLevelWasLoaded(int levelIndex)
    {
        if (levelIndex != 0)
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
