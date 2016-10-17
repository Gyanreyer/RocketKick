using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using XInputDotNetPure;

public class LevelSelectMenu : MonoBehaviour {

    public LevelButton[] levelButtons;
    public Text timerText;

    private GameManager gm;

    private GamePadState[] prevStates;

    int numLocked;

    private float countdownTimer;

    public LevelVoter[] voters;

    private float levelSelectedCountdown;

	// Use this for initialization
	void Start () {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        Player[] players = gm.AllPlayers;

        voters = new LevelVoter[players.Length];

        for(int i = 0; i < players.Length; i++)
        {
            voters[i] = new LevelVoter(i,players[i].Color);
            levelButtons[0].SelectByPlayer(voters[i]);
        }

        prevStates = new GamePadState[4];

        countdownTimer = 30;
        levelSelectedCountdown = 3;
	}
	
	// Update is called once per frame
	void Update () {

        for(int i = 0; i < voters.Length; i++)
        {
            GamePadState gpState = GamePad.GetState((PlayerIndex)i);

            if (gpState.ThumbSticks.Left.X < -.5f && prevStates[i].ThumbSticks.Left.X >= -.5f) ChangeSelection(i, -1);
            

            if (gpState.ThumbSticks.Left.X > .5f && prevStates[i].ThumbSticks.Left.X <= .5f) ChangeSelection(i, 1);

            if (gpState.ThumbSticks.Left.Y < -.5f && prevStates[i].ThumbSticks.Left.Y >= -.5f) ChangeSelection(i,3);

            if (gpState.ThumbSticks.Left.Y > .5f && prevStates[i].ThumbSticks.Left.Y <= .5f) ChangeSelection(i, -3);

            if (gpState.Buttons.A == ButtonState.Pressed && prevStates[i].Buttons.A != ButtonState.Pressed && !voters[i].locked)
            {
                levelButtons[voters[i].selection].LockInVote(voters[i]);
                numLocked++;

                voters[i].locked = true;
            }
            if(gpState.Buttons.B == ButtonState.Pressed && prevStates[i].Buttons.B != ButtonState.Pressed)
            {
                if(voters[i].locked)
                {
                    levelButtons[voters[i].selection].UnlockVote(voters[i]);
                    voters[i].locked = false;
                    numLocked--;

                    levelSelectedCountdown = 3;
                }

                else if(numLocked <= 0)
                {
                    SceneManager.LoadScene(1);
                }
            }

            prevStates[i] = gpState;

        }
        

        //Lock in all votes if timer is down
        if(countdownTimer <= 0)
        {
            for(int i = 0; i < voters.Length; i++)
            {
                if(!voters[i].locked)
                {                  
                    levelButtons[levelButtons.Length-1].LockInVote(voters[i]);
                    voters[i].locked = true;
                    numLocked++;
                }
            }

            timerText.text = 0.ToString();
        }
        else
        {
            timerText.text = ((int)countdownTimer + 1).ToString();

            countdownTimer -= Time.deltaTime;
        }

        if(numLocked == voters.Length)
        {
            levelSelectedCountdown -= Time.deltaTime;


            if(levelSelectedCountdown <= 0)
            {
                List<int> levelsToPickFrom = new List<int>();

                for (int i = 0; i < levelButtons.Length; i++)
                {
                    if (levelButtons[i].numSelectedBy > 0)
                        levelsToPickFrom.Add(i);
                }

                int highestVoteAmt = 0;

                for (int i = 0; i < levelsToPickFrom.Count; i++)
                {
                    if (levelButtons[levelsToPickFrom[i]].numSelectedBy < highestVoteAmt)
                    {
                        levelsToPickFrom.Remove(i);
                    }
                }


                int selectedLevel;

                if (levelsToPickFrom.Count == 1)
                {
                    selectedLevel = levelsToPickFrom[0];
                }
                else
                {
                    selectedLevel = levelsToPickFrom[Random.Range(0, levelsToPickFrom.Count - 1)];
                }


                //If the selection is the last button, this will randomly select a level
                if (selectedLevel == levelButtons.Length - 1)
                    selectedLevel = Random.Range(0, selectedLevel - 1);

                levelButtons[selectedLevel].GetComponent<Outline>().enabled = true;

                gm.StartGame(selectedLevel);

            }  

        }

        //Shortcuts to directly load a level
        if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            gm.StartGame(0);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            gm.StartGame(1);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha3))
        {
            gm.StartGame(2);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha4))
        {
            gm.StartGame(3);
        }
        else if (Input.GetKeyDown(KeyCode.Alpha5))
        {
            gm.StartGame(4);
        }

    }

    private void ChangeSelection(int playerInd, int changeAmt)
    {
        if (voters[playerInd].locked) return;

        levelButtons[voters[playerInd].selection].DeselectByPlayer(voters[playerInd]);

        voters[playerInd].selection = (voters[playerInd].selection + changeAmt) % levelButtons.Length;

        if (voters[playerInd].selection < 0) voters[playerInd].selection = levelButtons.Length - Mathf.Abs(voters[playerInd].selection);

        levelButtons[voters[playerInd].selection].SelectByPlayer(voters[playerInd]);

    }


}
