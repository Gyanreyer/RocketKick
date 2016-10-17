using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using XInputDotNetPure;

public class CharacterSelectManager : MonoBehaviour {

    private GameManager gm;

    public Text[] statusText;
    public Image[] playerSprites;

    public Color[] playerColors;

    private Player[] players;

    private GamePadState[] prevStates;

    private int[] colorIndex;

    private float countdownToStart;
    private int playerCount;

    private bool allReady;

    public Text countdownText;


	// Use this for initialization
	void Start () {
        gm = GameObject.Find("GameManager").GetComponent<GameManager>();

        prevStates = new GamePadState[4];
        players = new Player[4];
        colorIndex = new int[4];

        for(int i = 0; i < 4; i++)
        {
            colorIndex[i] = i;
            playerSprites[i].GetComponent<Animator>().speed = 0;

            Image[] arrows = playerSprites[i].GetComponentsInChildren<Image>();

            for(int j = 0; j < arrows.Length; j++)
            {
                arrows[j].enabled = false;
            }

        }

        countdownToStart = 3;

        countdownText.enabled = false;

	}


	// Update is called once per frame
	void Update () {

        if (Input.GetKeyDown(KeyCode.Return))
        {
            if (players[1] == null)
                Join(1);

            players[1].ready = true;
            statusText[1].text = "Player 1\nReady! This player was added with debug";
        }

        for (int i = 0; i < 4; i++)
        {
            GamePadState gpState = GamePad.GetState((PlayerIndex)i);

            //Navigate back to main menu if no players and someone presses B
            if (playerCount == 0 && gpState.Buttons.B == ButtonState.Pressed && prevStates[i].Buttons.B != ButtonState.Pressed)
            {
                SceneManager.LoadScene(0);
            }


            if (players[i] == null && gpState.Buttons.Start == ButtonState.Pressed)
            {
                Join(i);
            }

            else if (players[i] != null)
            {

                if (gpState.Buttons.B == ButtonState.Pressed && prevStates[i].Buttons.B != ButtonState.Pressed)
                {
                    if (players[i].ready)
                    {
                        players[i].ready = false;
                        statusText[i].text = "Player 1\nPress A when Ready\nPress B to Leave";
                    }

                    else
                    {
                        players[i] = null;
                        statusText[i].text = "Player 1\nPress Start to Join";
                        playerSprites[i].GetComponent<Animator>().speed = 0;
                        playerSprites[i].color = new Color(.2f,.2f,.2f,.3f);

                        Image[] arrows = playerSprites[i].GetComponentsInChildren<Image>();

                        for (int j = 0; j < arrows.Length; j++)
                        {
                            arrows[j].enabled = false;
                        }


                        playerCount--;
                    }
                }
                else if (gpState.Buttons.A == ButtonState.Pressed && prevStates[i].Buttons.A != ButtonState.Pressed)
                {
                    if (!players[i].ready) players[i].ready = true;
                    statusText[i].text = "Player 1\nReady! Press B to Unready";
                }

                else if ((gpState.DPad.Left == ButtonState.Pressed && prevStates[i].DPad.Left != ButtonState.Pressed) || (gpState.ThumbSticks.Left.X < -.5f && prevStates[i].ThumbSticks.Left.X >= -.5f))
                {
                    colorIndex[i]--;

                    if (colorIndex[i] < 0)
                        colorIndex[i] = playerColors.Length + colorIndex[i];

                    players[i].ChangeColor(playerColors[colorIndex[i]]);
                    playerSprites[i].color = playerColors[colorIndex[i]];

                    playerSprites[i].transform.FindChild("Left Arrow").GetComponent<Animator>().Play("Select");
                }

                else if ((gpState.DPad.Right == ButtonState.Pressed && prevStates[i].DPad.Right != ButtonState.Pressed) || (gpState.ThumbSticks.Left.X > .5f && prevStates[i].ThumbSticks.Left.X <= .5f))
                {
                    colorIndex[i] = (colorIndex[i] + 1) % playerColors.Length;

                    players[i].ChangeColor(playerColors[colorIndex[i]]);
                    playerSprites[i].color = playerColors[colorIndex[i]];

                    playerSprites[i].transform.FindChild("Right Arrow").GetComponent<Animator>().Play("Select");
                }
            }

            prevStates[i] = gpState;
        }


        allReady = true;

        for(int i = 0; i < 4; i++)
        {
            if (players[i] == null) continue;

            if (!players[i].ready)
            {
                allReady = false;
                break;
            }
        }

        if (allReady && playerCount >= 2)
        {
            countdownToStart -= Time.deltaTime;

            countdownText.enabled = true;
            countdownText.text = ((int)countdownToStart + 1).ToString();
        }
        else
        {
            countdownToStart = 3;

            countdownText.enabled = false;
        }

        if(countdownToStart <= 0)
        {
            gm.AddPlayers(players);
            SceneManager.LoadScene(2);
        }
    }

    private void Join(int i)
    {
        players[i] = new Player(i, gm.playerPrefab, playerColors[colorIndex[i]]);

        statusText[i].text = "Player 1\nPress A When Ready\nPress B to Leave";

        playerSprites[i].color = playerColors[colorIndex[i]];
        playerSprites[i].GetComponent<Animator>().speed = 1;

        Image[] arrows = playerSprites[i].GetComponentsInChildren<Image>();

        for (int j = 0; j < arrows.Length; j++)
        {
            arrows[j].enabled = true;
        }

        playerCount++;
    }
}
