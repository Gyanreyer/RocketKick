using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelButton : MonoBehaviour {

    public int numSelectedBy;

    public GameObject selectionIndicator;

    private LevelSelectMenu levSelMenu;

    //private Color[] colors;
    public Image[] indicators;
    private List<Color> colors;
    private List<Color> lockedInColors;

    // Use this for initialization
    void Awake() {
        numSelectedBy = 0;

        colors = new List<Color>(4);

        for(int i = 0; i < indicators.Length; i++)
        {
            indicators[i].color = Color.clear;
        }

        levSelMenu = GameObject.Find("LevelMenuManager").GetComponent<LevelSelectMenu>();

        
    }

    public void SelectByPlayer(int i)
    {
        colors.Add(levSelMenu.playerColors[i]*.75f);

        indicators[colors.Count - 1].color = colors[colors.Count - 1];

        numSelectedBy++;
    }

    public void DeselectByPlayer(int i)
    {
        colors.Remove(levSelMenu.playerColors[i]*.75f);

        numSelectedBy--;

        for(int j = 0; j < 4; j++)
        {
            if (j < colors.Count)
                indicators[j].color = colors[j];
            else
                indicators[j].color = Color.clear;
        }
    }

    public void LockInVote(int i)
    {
        int ind = colors.IndexOf(levSelMenu.playerColors[i]*.75f);

        colors[ind] = levSelMenu.playerColors[i];

        indicators[ind].color = colors[ind];
    }
}
