using UnityEngine;
using System.Collections;

public class ParallaxController : MonoBehaviour {

    Camera camera;
    Vector3 cameraVelocity;
    GameObject suns, backClouds, backMountains, frontClouds, frontMountains;
    public float sunSpeed, backCloudSpeed, backMountainSpeed, frontCloudSpeed, frontMountainSpeed;
	// Use this for initialization
	void Start () {
        camera = GameObject.Find("Main Camera").GetComponent<Camera>();
        suns = GameObject.Find("suns");
        backClouds = GameObject.Find("backClouds");
        backMountains = GameObject.Find("backMountains");
        frontClouds = GameObject.Find("frontClouds");
        frontMountains = GameObject.Find("frontMountains");
    }
	
	// Update is called once per frame
	void Update () {
        cameraVelocity = camera.velocity;
        float x = cameraVelocity.x;
        
        suns.transform.Translate(new Vector3((x / sunSpeed), 0f, 0f));
        backClouds.transform.Translate(new Vector3((x / backCloudSpeed), 0f, 0f));
        backMountains.transform.Translate(new Vector3((x / backMountainSpeed), 0f, 0f));
        frontClouds.transform.Translate(new Vector3((x / frontCloudSpeed), 0f, 0f));
        frontMountains.transform.Translate(new Vector3((x / frontMountainSpeed), 0f, 0f));


    }
}
