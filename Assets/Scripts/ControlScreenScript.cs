using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using XInputDotNetPure;


public class ControlScreenScript : MonoBehaviour {
    
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        for(int i = 0; i < 4; i++)
        {
            GamePadState gpState = GamePad.GetState((PlayerIndex)i);
            if(gpState.Buttons.B == ButtonState.Pressed)
            {
                SceneManager.LoadScene(0);
            }
        }
    }
}
