using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {

    public GameObject playerPrefab;

    bool[] players = new bool[4];
    public int playerCount = 0;

	// Initialization, executes before Start()
	void Awake () {
        //If there's already a GM in the scene, destroy this one so no duplicates
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject);
            return;
        }

        DontDestroyOnLoad(gameObject);//Don't destroy this object, it'll persist between scenes
    }
	
	// Update is called once per frame
	void Update () {

        for(int i = 1; i <= 4; i++)
        {
            if (Input.GetButtonDown("Start Button P" + i) && !players[i])
            {
                players[i] = true;
                playerCount++;
            }
        }
        


	
	}

    public void loadScene(int i)
    {
        SceneManager.LoadScene(i);
    }
}
