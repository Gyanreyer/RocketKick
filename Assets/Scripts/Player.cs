using UnityEngine;
using XInputDotNetPure;

//Basic class for players that stores all relevant info about a given player - GameManager has an array of these
public class Player : MonoBehaviour {

    private int score;//Score for number of rounds won
    private GameObject body;//GameObject for player's body
    private PlayerController controller;//Controller script for player
    private bool alive;//Whether player is alive and accessible
    public bool inGame;

    private int index;

    public int Score { get { return score; } }
    public GameObject Body { get { return body; } }
    public PlayerController Controller { get { return controller; } }
    public bool Alive { get { return alive; } }
    public int Index { get { return index; } }

    public Player(int ind)
    {
        index = ind;

        score = 0;

        body = null;
        controller = null;

        alive = false;
    }

    public void WinRound()
    {
        score++;
    }

    public void SpawnNewPlayer(Vector3 spawnPos, GameObject bodyGO)
    {
        if (!inGame) return;

        body = (GameObject)Instantiate(bodyGO, spawnPos, Quaternion.identity);

        controller = body.GetComponent<PlayerController>();

        controller.index = (PlayerIndex)index;

        alive = true;
    }

    public void Die()
    {
        Destroy(body);
        body = null;
        controller = null;

        alive = false;
    }

}
