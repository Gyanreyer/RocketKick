using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class MenuButton : MonoBehaviour {

    public bool selected;

    private Text buttonText;

    public Color origColor;
    public Color selColor;

	// Use this for initialization
	void Start () {
        buttonText = GetComponent<Text>();
        buttonText.color = origColor;
	}
	
    public void select()
    {
        selected = true;
        buttonText.color = selColor;
    }

    public void deselect()
    {
        selected = false;
        buttonText.color = origColor;
    }
}
