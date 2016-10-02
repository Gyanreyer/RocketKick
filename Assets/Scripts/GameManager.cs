using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public GameObject playerPrefab;

    bool[] players = new bool[4];
    public int playerCount = 0;

    private int playersAlive = 0;

    GameObject promptText;

    public Material[] skins = new Material[4];

    private GameObject[] activePlayers = new GameObject[4];
    int[] playerScores = new int[4];

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
        playersAlive = 0;

        for(int i = 0; i < 4; i++)
        {
            if(players[i])
            {
                int number = i + 1;

                GameObject p = Instantiate(playerPrefab);
                p.name = "P" + number;
                p.transform.FindChild("Feet").name = "Feet"+number;

                activePlayers[i] = p.transform.FindChild("Player").gameObject;
                activePlayers[i].name = "Player" + number;
                activePlayers[i].GetComponent<MeshRenderer>().material = skins[i];
                activePlayers[i].GetComponent<PlayerController>().setPlayerNum(number);

                playersAlive++;
            }
        }
    }

    public void killPlayer(int index)
    {
        playersAlive--;

        Destroy(activePlayers[index].transform.parent.gameObject);
        activePlayers[index] = null;

        //If only one player left, increase that player's score and then respawn for next round
        if (playersAlive == 1)
        {
            for (int i = 0; i < 4; i++)
            {
                if (activePlayers[i])
                {
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
}
