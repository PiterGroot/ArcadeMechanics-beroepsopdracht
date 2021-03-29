using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Dialogue 
{
    public bool AutoPlay = false;   
    public string name;
    [TextArea(3, 10)]
    public List<string> Sentences = new List<string>();
    public string SoundFileName;
    public Vector2 PositionsOnScreen;
   
}


