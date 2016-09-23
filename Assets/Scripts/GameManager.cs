using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class GameManager : MonoBehaviour {

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
	
	}

    public void loadScene(int i)
    {
        SceneManager.LoadScene(i);
    }
}
