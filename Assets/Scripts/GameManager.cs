using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;
using XInputDotNetPure;

public class GameManager : MonoBehaviour
{

    enum GameState {
        Menu,
        Playing,
        Won
    };


    private GameState state;

    private List<Player> players;//List of players who joined game - FUTURE NOTE: ADD OPTION TO LEAVE GAME

    private int playersAlive;//Keeps track of number of players alive
    public int MAX_SCORE = 10;//Max score that players aim for to win

    public GameObject deathPartSys;//Particle system to spawn when player dies

    public Color[] textColors = new Color[4];//Colors for text associate w/ each player

    public GameObject[] playerPrefabs;

    public GameObject[] spawnPoints;//assigned in the FindSpawnpoints() method

    

    public Player[] AlivePlayers { get { return players.FindAll(p=>p.Alive).ToArray(); } }//Property returns array of all currently alive players


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

        //If in menu scene, get prompt text
        if (SceneManager.GetActiveScene().buildIndex == 0)
        {
            state = GameState.Menu;
        }

        players = new List<Player>(4);
        playersAlive = 0;

    }

    // Update is called once per frame
    void Update()
    {
        //If on main menu, get active controllers
        if (state == GameState.Menu)
        {
            GamePadState gpState;

            //Pressing R key allows you to spawn in a dummy player 2 for debugging if only have 1 controller
            if (Input.GetKeyDown(KeyCode.R) && !players.Exists(p => p.Index == 1))
            {
                players.Add(new Player(1, playerPrefabs[1], textColors[1]));

                GameObject.Find("PlayerStatus" + 2).GetComponent<Text>().text = "Player " + 2 + "\nReady!";
            }

            //If 2+ players, start game when someone presses A button
            if (players.Count > 1 && Input.GetButtonDown("A Button") || Input.GetKeyDown(KeyCode.Return))
            {
                loadScene(1);
                ResetPlayerScores();
                state = GameState.Playing;
            }

            //Check for players pressing start button and add them to game if they do - NOTE: ADD WAY FOR PLAYERS TO LEAVE W/ B BUTTON
            for (int i = 0; i < 4; i++)
            {
                gpState = GamePad.GetState((PlayerIndex)i);

                //Check if a player doesn't exist w/ this index, if not then pressing start can add them
                if (!players.Exists(p => p.Index == i))
                {
                    //Add player if press start
                    if (gpState.Buttons.Start == ButtonState.Pressed)
                    {
                        players.Add(new Player(i, playerPrefabs[i], textColors[i]));

                        GameObject.Find("PlayerStatus" + (i + 1)).GetComponent<Text>().text = "Player " + (i + 1) + "\nReady!\nPress B to Leave";
                    }
                }

                //If player w/ this index already exists, pressing B button removes them
                else if (gpState.Buttons.B == ButtonState.Pressed)
                {
                    players.RemoveAll(p => p.Index == i);

                    GameObject.Find("PlayerStatus" + (i + 1)).GetComponent<Text>().text = "Player " + (i + 1) + "\nPress Start to Join";
                }

            }

            //When more than one person is ready, show prompt that you can start game, otherwise hide it
            if (players.Count > 1)
            {
                GameObject.Find("StartPrompt").GetComponent<Text>().color = new Color(.77f, .42f,.48f,1);
            }
            else
            {
                GameObject.Find("StartPrompt").GetComponent<Text>().color = Color.clear;
            }
        }
        else if(state == GameState.Won)
        {
            if(Input.GetButtonDown("A Button"))
            { 
                loadScene(0);
                state = GameState.Menu;

                //Kill living player
                players.Find(p => p.Alive).Die();

                players.Clear();
            }
        }

        for (int i = 0; i < players.Count; i++)
        {
            //Vibrate player's controller and then decrease power over time, this is used for vibrating controller when player dies
            if (players[i].vibrationPower > 0)
            {
                GamePad.SetVibration((PlayerIndex)players[i].Index, players[i].vibrationPower, players[i].vibrationPower);
                players[i].vibrationPower -= Time.deltaTime;
            }
            else
            {
                players[i].vibrationPower = 0;
            }
        }
    }

    //Reset all players' scores
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

        //Load a scene with given index
    public void loadScene(int i)
    {
        SceneManager.LoadScene(i);
    }


    //Spawn players for all joined players
    public void spawnPlayers()
    {
        //Gather spawnpoints
        PopulateSpawnpoints();

        //Array to tell which spawnpoints are occupied
        bool[] spawnsUsed = new bool[spawnPoints.Length];

        int spawnIndex = 0;

        for (int i = 0; i < players.Count; i++)
        {
            players[i].vibrationPower = 0;//Stop vibration as a precaution

            //Finding a good spawnpoint
            spawnIndex = (int)Random.Range(0, spawnPoints.Length - 1);
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

    //Kill player with given number
    public void killPlayer(int number)
    {
        Player pToKill = players.Find(p => p.PlayerNum == number);//Get player we're killing

        Instantiate(deathPartSys,pToKill.Position,Quaternion.identity);//Make death particle system where they died

        pToKill.Die();//Destroy player w/ no delay - We should consider changing this eventually so maybe the player's body goes flying or something, that'd be pretty funny/cool

        pToKill.vibrationPower = .75f;//Vibrate controller to signify death

        playersAlive--;//One less player alive

        //If only one player left, increase that player's score and then respawn all for next round
        if (playersAlive == 1)
        {
            Player lastP = players.Find(p => p.Alive);

            lastP.WinRound();
            
            if(lastP.Score >= MAX_SCORE)
            {
                Text winText = GameObject.Find("WinText").GetComponent<Text>();

                winText.text = "Player " + lastP.PlayerNum + " wins!";
                winText.color = textColors[lastP.Index];

                GameObject.Find("ContinueText").GetComponent<Text>().color = textColors[lastP.Index];

                //Resolve win condition
                state = GameState.Won;
                //SceneManager.LoadScene(1);//Load game over scene, not set up now so I disabled it
                return;
            }

            lastP.Die(2);

            Invoke("spawnPlayers", 2.1f);
        }
    }

    /// <summary>
    /// Gives the camera an array of alive players
    /// </summary>
    void GiveAlivePlayersToCamera()
    {
        if (playersAlive == 0)
            return; //this here is to catch the whole "OnLevelWasLoaded" getting called twice thing
                    //more info here:http://answers.unity3d.com/questions/466880/is-onlevelwasloaded-supposed-to-be-called-twice-wh.html
                    //GameObject[] alivePlayerObjects = new GameObject[playersAlive];
                    //int alivePlayerCounter = 0;

        Player[] alivePlayers = players.FindAll(p => p.Alive).ToArray();

        GameObject[] alivePlayerObjects = new GameObject[alivePlayers.Length];
        int i = 0;

        foreach(Player p in alivePlayers)
        {
            alivePlayerObjects[i] = p.Controller.gameObject;
            i++;
        }


        Camera.main.GetComponent<dynamicCamera>().SetAlivePlayers(alivePlayerObjects);

        /*
        for (int i = 0; i < players.Count; ++i)
        {
            if (activePlayers[i] != null) 
            {
                alivePlayerObjects[alivePlayerCounter] = activePlayers[i];
                //++alivePlayerCounter;
            }
        }
        Camera.main.GetComponent<dynamicCamera>().SetAlivePlayers(alivePlayerObjects);*/
    }

    


    void OnLevelWasLoaded(int levelIndex)
    {

        if (levelIndex > 0)
        {     
            spawnPlayers();
            //GiveAlivePlayersToCamera();
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
