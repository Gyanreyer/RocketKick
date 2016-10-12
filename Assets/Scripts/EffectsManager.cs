using UnityEngine;
using System.Collections;

//Handles special effects like screen shake and slow-mo
public class EffectsManager : MonoBehaviour {

    //Intensity and duration of screen shake
    private float shakeIntensity;
    private float shakeDuration;

    private Vector3 originalPos;//Once dynamic cam is working this'll interfere, shouldn't be too hard to fix

    private DynamicCamera dynCam;

    private float playSpeed;//Speed at which game runs/does fixed updates for physics - cool beans

    private GameManager gm;

	// Use this for initialization
	void Start () {

        playSpeed = 1;

        dynCam = Camera.main.GetComponent<DynamicCamera>();

        //originalPos = Camera.main.transform.position;//Original pos to return camera to, not sure how this'll interact with dynamic camera but it'll do for now
        originalPos = dynCam.CamPos;

        gm = GameObject.Find("GameManager").GetComponent<GameManager>();
	}
	

	// Late update is called later after everything else has run normal update - want to apply these effects last
	void LateUpdate () {

        UpdateShake();

        CheckForSlowMo();
	}

    //Sets values for shake
    public void Shake(float intensity, float duration)
    {
        //originalPos = Camera.main.transform.position;
        originalPos = dynCam.CamPos;

        shakeIntensity = intensity;
        shakeDuration = duration;
    }

    //Determine whether to shake screen on update
    private void UpdateShake()
    {
        //If shake is in progress...
        if (shakeDuration > 0)
        { 
            Vector3 posMod = Random.onUnitSphere * shakeIntensity;//Get random vector multiplied by intensity

            Camera.main.transform.position += new Vector3(posMod.x, posMod.y, 0);//Modify cam position

            shakeDuration -= Time.deltaTime;//Decrement timer
        }
        else
        {
            //Camera.main.transform.position = originalPos;//Return to original pos
            Camera.main.transform.position = dynCam.CamPos;
        }
    }

    //If any players are kicking and near each other, enter slow mo
    void CheckForSlowMo()
    {
        float closestDist = float.MaxValue;//Stores shortest dist between 2 players
        float speed = 0;

        Player[] players = gm.AlivePlayers;

        //Compare all living players
        for (int i = 0; i < players.Length; i++)
        {
            for (int j = 0; j < gm.AlivePlayers.Length; j++)
            {
                if (i == j) continue;//Skip checking against self

                float dist = (players[i].Position - players[j].Position).magnitude;

                //If either player kicking and dist between is shorter than what's stored, store it as new closest dist along with the fastest speed between the two
                if (dist < closestDist && (players[i].Controller.Kicking || players[j].Controller.Kicking))
                {
                    closestDist = dist;
                    speed = Mathf.Max(players[i].Controller.CurrentSpeed,players[j].Controller.CurrentSpeed);
                }
            }
        }

        //If shortest dist is within radius, slow time based on how close they are and how fast they're moving, otherwise start moving timescale back toward 1
        if (closestDist < 2f)
            playSpeed = Mathf.Clamp((players[0].Controller.maxSpeed/(speed*4))*(closestDist/2), .1f, 1);//TEST VALUES AND TWEAK
        else if (playSpeed < 1)
            playSpeed += 2 * Time.deltaTime;
        else
            playSpeed = 1;

        //Set timescale 
        Time.timeScale = playSpeed;

    }
}
