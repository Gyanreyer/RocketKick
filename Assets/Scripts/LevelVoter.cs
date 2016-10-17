using UnityEngine;
using System.Collections;

public class LevelVoter{

    public int index;
    public bool locked;
    public int selection;
    public Color color;

    public LevelVoter(int i,Color c)
    {
        selection = 0;
        locked = false;

        index = i;
        color = c;
    }
}
