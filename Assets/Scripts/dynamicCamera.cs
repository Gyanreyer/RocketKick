using UnityEngine;
using System.Collections;

public class DynamicCamera : MonoBehaviour
{
    private Vector3 centerFocus;
    //    public GameObject centerFocusObj;
    private GameManager gameMan;
    private Player[] alivePlayers;
    private Vector3 camPos;
    public float scrollSpeed = 5;   //this number can be played with
    public float zoomSpeed = 0.018f;    //definitely need a small number for this
    private float xViewDist;//half the amount of horizontal distance the camera sees
    private float yViewDist;//'' but for the vertical distance
    private float farthestAbsY, farthestAbsX;
    public float extraCameraDist = 0.5f; //extra area to be added on to the camera, added to the farthest player X/Y
    public float camZoomDeadZone = 2;   //needed to prevent the camera from spazzing in and out

    public Vector3 CamPos { get { return camPos; } }

    void Start()
    {
        centerFocus = Vector3.zero;
        camPos = Vector3.zero;
        gameMan = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    void Update()
    {
        alivePlayers = gameMan.AlivePlayers;
        if (alivePlayers.Length > 1)
        {
            ///Camera Position
            //Find the average between all alive players
            centerFocus = Vector3.zero;
            for (int i = 0; i < alivePlayers.Length; ++i)
            {
                centerFocus += alivePlayers[i].LocalPosition;
            }
            centerFocus /= (float)alivePlayers.Length;
            //Move towards the average
            camPos = new Vector3(Mathf.MoveTowards(Camera.main.transform.position.x, centerFocus.x, scrollSpeed * Time.deltaTime), Mathf.MoveTowards(Camera.main.transform.position.y, centerFocus.y, scrollSpeed * Time.deltaTime), -10);
            ///Camera Zoom
            //I basically just store these values so I don't have to type them out a bunch later. It's just values from the camera center to the edge of the camera view
            yViewDist = Camera.main.orthographicSize;
            xViewDist = yViewDist * Screen.width / Screen.height;
            //Here I look for the farthest X and Y of the players and later base the camera zoom off that
            farthestAbsX = 0;
            farthestAbsY = 0;
            for (int i = 0; i < alivePlayers.Length; ++i)
            {
                if (Mathf.Abs(centerFocus.y - alivePlayers[i].Position.y) > farthestAbsY)
                    farthestAbsY = Mathf.Abs(centerFocus.y - alivePlayers[i].Position.y);
                if (Mathf.Abs(centerFocus.x - alivePlayers[i].Position.x) > farthestAbsX)
                    farthestAbsX = Mathf.Abs(centerFocus.x - alivePlayers[i].Position.x);
            }
            Debug.DrawLine(Vector3.zero, new Vector3(centerFocus.x + farthestAbsX, centerFocus.y, 0), Color.red);
            Debug.DrawLine(Vector3.zero, new Vector3(centerFocus.x, centerFocus.y + farthestAbsY, 0), Color.red);
            //Check to see if you need zoom in X direction or Y direction
            if (farthestAbsX > farthestAbsY)
            {
                //This check here is to prevent the camera from spazzing in and out once it hits its dead zone (the number can be played with a bit in the inspector)
                if (Mathf.Abs(xViewDist - farthestAbsX * Screen.width / Screen.height) > camZoomDeadZone)
                {
                    //Add a bit to let the farthest player see more away from the center
                    farthestAbsX += extraCameraDist;
                    //Check to see if you need to move in or out
                    if (xViewDist > farthestAbsX * Screen.width / Screen.height)
                        Camera.main.orthographicSize -= zoomSpeed;
                    else if (xViewDist < farthestAbsX * Screen.width / Screen.height)
                        Camera.main.orthographicSize += zoomSpeed;
                    //don't zoom at all if you've hit the goal, hence the second if after the else
                }
            }
            else
            {
                if (Mathf.Abs(yViewDist - farthestAbsY) > camZoomDeadZone)
                {
                    farthestAbsY += extraCameraDist;
                    if (yViewDist > farthestAbsY)
                        Camera.main.orthographicSize -= zoomSpeed;
                    else if (yViewDist < farthestAbsY)
                        Camera.main.orthographicSize += zoomSpeed;
                }
            }
        }
        else
        {
            //This SHOULD work, but for some reason gives an index out of bounds exception
            //camPos = gameMan.AlivePlayers[0].LocalPosition;
            camPos = new Vector3(0, 0, -10);
            Camera.main.orthographicSize = 7;
        }
        Camera.main.transform.position = camPos;
    }

    /// <summary>
    /// Should be called each time the players spawn in, AFTER they've spawned in, and also whenever a player dies
    /// </summary>
    /// /// <param name="activePlayers">An array of ALIVE players only</param>
    public void SetAlivePlayers(Player[] activePlayers)
    {
        alivePlayers = activePlayers;
    }
}
