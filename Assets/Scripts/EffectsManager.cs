using UnityEngine;
using System.Collections;

public class EffectsManager : MonoBehaviour {

    private float shakeIntensity;
    private float shakeDuration;
    private Vector3 originalPos;//Once dynamic cam is working this'll interfere, shouldn't be too hard to fix


	// Use this for initialization
	void Start () {
        originalPos = Camera.main.transform.position;
	}
	
	// Late update is called later after everything else has run normal update - hopefully will help not interfere with dynamic cam
	void LateUpdate () {
	
        if(shakeDuration > 0)
        { 

            Vector3 posMod = Random.onUnitSphere;

            Camera.main.transform.position += new Vector3(posMod.x*shakeIntensity,posMod.y*shakeIntensity,0);

            shakeDuration -= Time.deltaTime;
        }
        else
        {
            originalPos.z = -10;
            Camera.main.transform.position = originalPos;
        }

	}

    public void Shake(float intensity, float duration)
    {
        originalPos = transform.position;

        shakeIntensity = intensity;
        shakeDuration = duration;



    }
}
