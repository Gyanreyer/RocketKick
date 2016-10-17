using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class LevelButton : MonoBehaviour {

    public int numSelectedBy;

    public GameObject selectionIndicator;

    private LevelSelectMenu levSelMenu;

    //private Color[] colors;
    public Image[] indicators;
    private List<LevelVoter> voters;

    // Use this for initialization
    void Awake() {
        numSelectedBy = 0;

        voters = new List<LevelVoter>(4);

        for(int i = 0; i < indicators.Length; i++)
        {
            indicators[i].color = Color.clear;
        }

        levSelMenu = GameObject.Find("LevelMenuManager").GetComponent<LevelSelectMenu>();

        
    }

    public void SelectByPlayer(LevelVoter v)
    {
        if (voters.Contains(v)) return;

        voters.Add(v);

        indicators[voters.Count - 1].color = v.color* .75f;

        numSelectedBy++;
    }

    public void DeselectByPlayer(LevelVoter v)
    {
        voters.Remove(v);

        numSelectedBy--;

        for(int j = 0; j < 4; j++)
        {
            if (j < voters.Count)
                indicators[j].color = voters[j].color * (voters[j].locked ? 1 : .75f);
            else
                indicators[j].color = Color.clear;
        }
    }

    public void LockInVote(LevelVoter v)
    {
        int ind = voters.IndexOf(v);//voters.FindIndex(voter => voter.index == v.index);

        voters[ind].locked = true;

        indicators[ind].color = voters[ind].color;
    }

    public void UnlockVote(LevelVoter v)
    {
        int ind = voters.IndexOf(v);

        voters[ind].locked = false;

        indicators[ind].color = voters[ind].color * .75f;
    }
}
