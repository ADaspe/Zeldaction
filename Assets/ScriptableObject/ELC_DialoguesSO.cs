using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Objects", menuName = "ScriptableObjects/DialoguesSO", order = 1)]
public class ELC_DialoguesSO : ScriptableObject
{
    public string Name;
    public Sprite MiniaturePerso;
    public DialStruct[] Dialog;
}

[System.Serializable]
public class DialStruct
{
    public string DialLine;
    public bool RynSentence;
}
