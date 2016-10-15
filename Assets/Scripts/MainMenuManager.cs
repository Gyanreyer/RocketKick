using UnityEngine;
using UnityEngine.SceneManagement;
using XInputDotNetPure;

public class MainMenuManager : MonoBehaviour {

    public MenuButton[] buttons = new MenuButton[3];
    
    enum MenuButtons
    {
        start,
        options,
        quit
    }

    GameManager gm;


    GamePadState[] prevState = new GamePadState[4];

    int selectedButton;

	// Use this for initialization
	void Start () {
        selectedButton = 0;

        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
	}
	
	// Update is called once per frame
	void Update () {

        
        if(!buttons[selectedButton].selected)
        {
            buttons[selectedButton].select();
        }

        for (int i = 0; i < 4; i++)
        {
            GamePadState gpState = GamePad.GetState((PlayerIndex)i);

            if ((gpState.DPad.Up == ButtonState.Pressed && prevState[i].DPad.Up != ButtonState.Pressed) || (gpState.ThumbSticks.Left.Y > .5f && prevState[i].ThumbSticks.Left.Y <=.5f))
            {
                buttons[selectedButton].deselect();

                selectedButton--;

                selectedButton = selectedButton >= 0 ? selectedButton : 3-Mathf.Abs(selectedButton);

                buttons[selectedButton].select();

            }
            else if((gpState.DPad.Down == ButtonState.Pressed && prevState[i].DPad.Down != ButtonState.Pressed) || (gpState.ThumbSticks.Left.Y < -.5f && prevState[i].ThumbSticks.Left.Y >= -.5f))
            {
                buttons[selectedButton].deselect();

                selectedButton = (selectedButton + 1) % 3;

                buttons[selectedButton].select();

            }



            if(gpState.Buttons.A == ButtonState.Pressed)
            {
                if (selectedButton == 2)
                    Application.Quit();

                else
                    SceneManager.LoadScene(selectedButton+1);
            }





            prevState[i] = gpState;
        }


	}
}
