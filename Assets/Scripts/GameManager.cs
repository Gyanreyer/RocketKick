using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using UnityEngine.UI;

public class GameManager : MonoBehaviour {

    public GameObject playerPrefab;

    bool[] players = new bool[4];
    public int playerCount = 0;

    GameObject promptText;

	// Initialization, executes before Start()
	void Awake () {
        //If there's already a GM in the scene, destroy this one so no duplicates
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);//Don't destroy this object, it'll persist between scenes


        promptText = GameObject.Find("StartPrompt");
        promptText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {

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
        for(int i = 0; i < 4; i++)
        {
            if(players[i])
            {
                int number = i + 1;

                GameObject p = Instantiate(playerPrefab);
                p.name = "P" + number;
                p.transform.FindChild("Feet").name = "Feet"+number;
                p.transform.FindChild("Player").name = "Player"+number;

                p.GetComponentInChildren<PlayerController>().setPlayerNum(number);
            }

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
