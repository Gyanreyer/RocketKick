using UnityEngine;
using System.Collections;

public class DeathPartSys : MonoBehaviour {

    private ParticleSystem ps;

	//Get the particle system
	void Start () {
        ps = GetComponent<ParticleSystem>();
	}
	
	//Destroy this object once the particle system is done
	void Update () {
        if (!ps.IsAlive())
            Destroy(gameObject);
	}
}
