using UnityEngine;
using System.Collections;

//Handles special effects like screen shake and slow-mo
public class EffectsManager : MonoBehaviour {

    //Intensity and duration of screen shake
    private float shakeIntensity;
    private float shakeDuration;

    private Vector3 originalPos;//Once dynamic cam is working this'll interfere, shouldn't be too hard to fix

    private float playSpeed;//Speed at which game runs/does fixed updates for physics - cool beans

    private GameManager gm;

	// Use this for initialization
	void Start () {

        playSpeed = 1;

        originalPos = Camera.main.transform.position;//Original pos to return camera to, not sure how this'll interact with dynamic camera but it'll do for now

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
        originalPos = Camera.main.transform.position;

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
            Camera.main.transform.position = originalPos;//Return to original pos
        }
    }

    //If any players are kicking and near each other, enter slow mo
    void CheckForSlowMo()
    {
        float dist = float.MaxValue;//Stores shortest dist between 2 players

        //Compare all living players
        for (int i = 0; i < gm.AlivePlayers.Length; i++)
        {
            for (int j = 0; j < gm.AlivePlayers.Length; j++)
            {
                if (i == j) continue;

                //If either player kicking, store dist btwn if shorter than one currently stored
                if (gm.AlivePlayers[i].Controller.Kicking || gm.AlivePlayers[j].Controller.Kicking)
                    dist = Mathf.Min(dist, (gm.AlivePlayers[i].Position - gm.AlivePlayers[j].Position).magnitude); 
            }
        }

        //If shortest dist is within radius, slow time based on how close they are, otherwise start moving timescale back toward 1
        if (dist < 2f)
            playSpeed = Mathf.Clamp(dist / 2, .1f, 1);
        else if (playSpeed < 1)
            playSpeed += 2 * Time.deltaTime;
        else
            playSpeed = 1;

        //Set timescale 
        Time.timeScale = playSpeed;

    }
}
