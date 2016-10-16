using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using XInputDotNetPure;

public class LevelSelectMenu : MonoBehaviour {

    public LevelButton[] levelButtons;
    public Text timerText;

    //private Player[] players;
    public Color[] playerColors;
    private int[] selections;//index is for each player, value is the level selected

    private GameManager gm;

    private GamePadState[] prevStates;

    bool votesLocked;
    bool levelPicked;

    private float countdownTimer;


	// Use this for initialization
	void Start () {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        Player[] players = gm.AllPlayers;

        playerColors = new Color[players.Length];

        for(int i = 0; i < players.Length; i++)
        {
            playerColors[i] = players[i].Color;
        }

        selections = new int[players.Length];

        prevStates = new GamePadState[players.Length];

        //levelButtons = GameObject.Find("Canvas").GetComponentsInChildren<LevelButton>();

        for(int i = 0; i < playerColors.Length; i++)
        {
            selections[i] = 0;
            levelButtons[0].SelectByPlayer(i);
        }

        countdownTimer = 30;
	}
	
	// Update is called once per frame
	void Update () {


        if (levelPicked) return;

        for(int i = 0; i < playerColors.Length; i++)
        {
            if (selections[i] < 0) continue;//Player locked in when selection set to -1

            GamePadState gpState = GamePad.GetState((PlayerIndex)i);

            if (gpState.ThumbSticks.Left.X < -.5f && prevStates[i].ThumbSticks.Left.X >= -.5f) ChangeSelection(i, -1);

            else if (gpState.ThumbSticks.Left.X > .5f && prevStates[i].ThumbSticks.Left.X <= .5f) ChangeSelection(i, 1);

            else if (gpState.ThumbSticks.Left.Y < -.5f && prevStates[i].ThumbSticks.Left.Y >= -.5f) ChangeSelection(i,3);

            else if (gpState.ThumbSticks.Left.Y > .5f && prevStates[i].ThumbSticks.Left.Y <= .5f) ChangeSelection(i, -3);

            else if (gpState.Buttons.A == ButtonState.Pressed && prevStates[i].Buttons.A != ButtonState.Pressed)
            {
                levelButtons[selections[i]].LockInVote(i);

                selections[i] = -1;
            }


            prevStates[i] = gpState;

        }

        
        

        //Lock in all votes if timer is down
        if(countdownTimer <= 0)
        {
            for(int i = 0; i < selections.Length; i++)
            {
                if(selections[i] >= 0)
                {
                    levelButtons[selections[i]].LockInVote(i);
                    selections[i] = -1;
                }
            }

            timerText.text = 0.ToString(); ;
        }
        else
        {
            timerText.text = ((int)countdownTimer + 1).ToString();

            countdownTimer -= Time.deltaTime;
        }




        votesLocked = true;

        for(int i = 0; i < selections.Length; i++)
        {
            if (selections[i] >= 0)
            {
                votesLocked = false;
                break;
            }  
        }


        if(votesLocked)
        {
            List<int> levelsToPickFrom = new List<int>();

            for(int i = 0; i < levelButtons.Length; i++)
            {
                if(levelButtons[i].numSelectedBy > 0)
                    levelsToPickFrom.Add(i);
            }

            int highestVoteAmt = 0;

            for(int i = 0; i < levelsToPickFrom.Count; i++)
            {
                if(levelButtons[levelsToPickFrom[i]].numSelectedBy < highestVoteAmt)
                {
                    levelsToPickFrom.Remove(i);
                }                
            }


            int selectedLevel;

            if(levelsToPickFrom.Count == 1)
            {
                selectedLevel = levelsToPickFrom[0];
            }
            else
            {
                selectedLevel = levelsToPickFrom[Random.Range(0, levelsToPickFrom.Count - 1)];
            }


            //If the selection is the last button, this will randomly select a level
            if (selectedLevel == levelButtons.Length - 1)
                selectedLevel = Random.Range(0,selectedLevel-1);

            levelButtons[selectedLevel].GetComponent<Outline>().enabled = true;

            gm.StartGame(selectedLevel);

            levelPicked = true;

        }

        //Shortcuts to directly load a level
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            gm.StartGame(0);
            levelPicked = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            gm.StartGame(1);
            levelPicked = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            gm.StartGame(2);
            levelPicked = true;
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            gm.StartGame(3);
            levelPicked = true;
        }

    }

    private void ChangeSelection(int playerInd, int changeAmt)
    {
        levelButtons[selections[playerInd]].DeselectByPlayer(playerInd);

        selections[playerInd]=(selections[playerInd]+changeAmt)%levelButtons.Length;

        if (selections[playerInd] < 0) selections[playerInd] = levelButtons.Length - Mathf.Abs(selections[playerInd]);

        levelButtons[selections[playerInd]].SelectByPlayer(playerInd);
    }
}
