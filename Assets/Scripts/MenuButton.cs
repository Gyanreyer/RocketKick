using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour {

    public bool selected;

    private Text buttonText;

	// Use this for initialization
	void Start () {
        buttonText = GetComponent<Text>();
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    public void select()
    {
        selected = true;
        buttonText.color *= 1.2f;//= new Color(buttonText.color.r*2,buttonText.color.g*2,buttonText.color.b*2);
    }

    public void deselect()
    {
        selected = false;
        buttonText.color /= 1.2f; //= new Color(buttonText.color.r / 2, buttonText.color.g / 2, buttonText.color.b / 2);
    }
}
