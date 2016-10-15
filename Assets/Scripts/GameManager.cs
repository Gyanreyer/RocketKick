using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
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

    public Color[] playerColors = new Color[4];//Colors for text associate w/ each player

    public GameObject playerPrefab;

    public GameObject[] spawnPoints;//assigned in the FindSpawnpoints() method

    

    public Player[] AlivePlayers { get { return players.FindAll(p=>p.Alive).ToArray(); } }//Property returns array of all currently alive players
    public Player[] AllPlayers { get { return players.ToArray(); } }

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
        if(state == GameState.Won)
        {
            if(Input.GetButtonDown("A Button"))
            { 
                //Kill living player
                Player player = players.Find(p => p.Alive);
                Destroy(player.mainObject);

                players.Clear();

                SceneManager.LoadScene(0);
                state = GameState.Menu;
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
    private void ResetPlayerScores()
    {
        foreach(Player p in players)
        {
            p.resetScore();
        }
    }

    public void StartGame(int scene)
    {
        ResetPlayerScores();
        state = GameState.Playing;

        StartCoroutine(LoadScene(3+scene,3));//Load scene with 3 second delay
    }

    //Load a scene with given index, delay for given amt of time
    IEnumerator LoadScene(int i, float delay)
    {
        yield return new WaitForSeconds(delay);

        SceneManager.LoadScene(i);
    }

    public void AddPlayers(Player[] ps)
    {
        for(int i = 0; i < ps.Length; i++)
        {
            if (ps[i] != null)
            {
                players.Add(ps[i]);
            }
        }

        
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

            players[i].mainObject = (GameObject)Instantiate(playerPrefab,spawnPoints[spawnIndex].transform.position,Quaternion.identity);
            players[i].SetupPlayer();

        }


        playersAlive = players.Count;
    }

    //Kill player with given number
    public void killPlayer(int number)
    {
        Player pToKill = players.Find(p => p.PlayerNum == number);//Get player we're killing

        Instantiate(deathPartSys,pToKill.Position,Quaternion.identity);//Make death particle system where they died

        Destroy(pToKill.mainObject);
        pToKill.DieBeforeEnd();//Destroy player w/ no delay - We should consider changing this eventually so maybe the player's body goes flying or something, that'd be pretty funny/cool

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
                winText.color = playerColors[lastP.Index];

                GameObject.Find("ContinueText").GetComponent<Text>().color = playerColors[lastP.Index];

                //Resolve win condition
                state = GameState.Won;
                //SceneManager.LoadScene(1);//Load game over scene, not set up now so I disabled it
                return;
            }

            Destroy(lastP.mainObject,2);
            lastP.Die();

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


        Camera.main.GetComponent<DynamicCamera>().SetAlivePlayers(AlivePlayers);
    }

    


    void OnLevelWasLoaded(int levelIndex)//This is now deprecated, figure out new way of doing it?
    {
        if (levelIndex > 2)
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
