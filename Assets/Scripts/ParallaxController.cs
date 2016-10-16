using UnityEngine;
using System.Collections;

public class ParallaxController : MonoBehaviour
{

    Camera camera;
    Vector3 cameraVelocity;
    GameObject suns, backClouds, backMountains, frontClouds, frontMountains;
    private Vector3 sunStartPos, backClouStartPos, backMounStartPos, frontMountStartPos, frontCloudStartPos;
    private float sunSpeed, backCloudSpeed, backMountainSpeed, frontCloudSpeed, frontMountainSpeed;
    private Vector3 prevPosition;
    // Use this for initialization
    void Start()
    {
        camera = Camera.main;
        suns = GameObject.Find("suns");
        backClouds = GameObject.Find("backClouds");
        backMountains = GameObject.Find("backMountains");
        frontClouds = GameObject.Find("frontClouds");
        frontMountains = GameObject.Find("frontMountains");

        sunSpeed = 3500.0f;
        backCloudSpeed = 3100.0f;
        backMountainSpeed = 2900.0f;
        frontCloudSpeed = 2000.0f;
        frontMountainSpeed = 1700.0f;
        prevPosition = camera.transform.position;

        sunStartPos = suns.transform.position;
        backClouStartPos = backClouds.transform.position;
        backMounStartPos = backMountains.transform.position;
        frontCloudStartPos = frontClouds.transform.position;
        frontMountStartPos = frontMountains.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        if (camera.transform.position.x != prevPosition.x)
        {
            float x = camera.transform.position.x;
            Debug.Log("Camera x:" + x);
            Debug.Log("Sunspeed:" + sunSpeed + "," + "Backcloudspeed" + backCloudSpeed);
            suns.transform.position = new Vector3((x / sunSpeed) + suns.transform.position.x, suns.transform.position.y, suns.transform.position.z);
            backClouds.transform.position = new Vector3((x / backCloudSpeed) + backClouds.transform.position.x, backClouds.transform.position.y, backClouds.transform.position.z);
            backMountains.transform.position = new Vector3((x / backMountainSpeed) + backMountains.transform.position.x, backMountains.transform.position.y, backMountains.transform.position.z);
            frontClouds.transform.position = new Vector3((x / frontCloudSpeed) + frontClouds.transform.position.x, frontClouds.transform.position.y, frontClouds.transform.position.z);
            frontMountains.transform.position = new Vector3((x / frontMountainSpeed) + frontMountains.transform.position.x, frontMountains.transform.position.y, frontMountains.transform.position.z);
        }
        prevPosition = camera.transform.position;
    }

    public void RoundOver()
    {
        suns.transform.position = sunStartPos;
        backClouds.transform.position = backClouStartPos;
        backMountains.transform.position = backMounStartPos;
        frontClouds.transform.position = frontCloudStartPos;
        frontMountains.transform.position = frontMountStartPos;
    }
}
