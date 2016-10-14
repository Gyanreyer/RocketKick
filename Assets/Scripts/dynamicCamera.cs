using UnityEngine;
using System.Collections;

public class DynamicCamera : MonoBehaviour
{
    private Vector3 centerFocus;
    private GameManager gameMan;
    private Player[] alivePlayers;
    private Vector3 camPos;
    /// <summary>
    /// Just copy the camera's OrthographicSize here - it'll reset to this every time a round ends.
    /// </summary>
    public float defaultSize;
    /// <summary>
    /// Scroll speed of the camera. Bigger levels will need faster movement.
    /// </summary>
    public float scrollSpeed = 5;
    /// <summary>
    /// Zoom speed of the camera. Bigger levels will need faster zoom.
    /// </summary>
    public float zoomSpeed = 0.018f;
    private float xViewDist;//half the amount of horizontal distance the camera sees
    private float yViewDist;//'' but for the vertical distance
    private float farthestAbsY, farthestAbsX;
    /// <summary>
    /// Extra padding area to be added on to the camera, added to the farthest player X/Y
    /// </summary>
    public float extraCameraDist = 0.5f;
    /// <summary>
    /// Needed to prevent the camera from spazzing in and out. 2 is a decent default but this should be played with if necessary.
    /// </summary>
    public float camZoomDeadZone = 2;
    /// <summary>
    /// The EffectsManager can use this property.
    /// </summary>
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
            //Add extra camera padding
            farthestAbsX += extraCameraDist;
            farthestAbsY += extraCameraDist;
            //Check to see if you need zoom in X direction or Y direction
            if (farthestAbsX > farthestAbsY)
            {
                //This check here is to prevent the camera from spazzing in and out once it hits its dead zone (the number should be played with a bit in the inspector... 2 is USUALLY good)
                if (Mathf.Abs(xViewDist - farthestAbsX * Screen.width / Screen.height) > camZoomDeadZone)
                    ZoomTowards(xViewDist, farthestAbsX);
            }
            else
            {
                if (Mathf.Abs(yViewDist - farthestAbsY) > camZoomDeadZone)
                    ZoomTowards(yViewDist, farthestAbsY);
            }
        }
        else
        {
            //If only one player remains, move back towards the center and zoom out slowly
            centerFocus = new Vector3(0, 0, -10);
            ZoomTowards(yViewDist, defaultSize);
        }
        //Move towards the focus point
        camPos = new Vector3(Mathf.MoveTowards(Camera.main.transform.position.x, centerFocus.x, scrollSpeed * Time.deltaTime), Mathf.MoveTowards(Camera.main.transform.position.y, centerFocus.y, scrollSpeed * Time.deltaTime), -10);
        //Update camera position for EffectsManager
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

    /// <summary>
    /// Zooms the camera size in or out
    /// </summary>
    /// <param name="xOrYDist">The X or Y view which will be used in testing to zoom in or out</param>
    /// <param name="distToZoomTowards">The desired distance that xOrYDist should eventually become</param>
    private void ZoomTowards(float xOrYDist, float distToZoomTowards)
    {
        //Check to see if you need to move in or out
        if (xOrYDist > distToZoomTowards)
            Camera.main.orthographicSize -= zoomSpeed;
        else if (xOrYDist < distToZoomTowards)
            Camera.main.orthographicSize += zoomSpeed;
    }
}
