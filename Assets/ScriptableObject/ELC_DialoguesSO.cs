using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Objects", menuName = "ScriptableObjects/DialoguesSO", order = 1)]
public class ELC_DialoguesSO : ScriptableObject
{
    public string Name;
    public Sprite MiniaturePerso;
    public bool AutoSkip;
    public List<MultiDialogs> Dialog;
    public List<MultiDialogs> RandomDialog;
}

[System.Serializable]
public class DialStruct
{
    public string DialLine;
    public string Sound;
    public bool RynSentence;
}

[System.Serializable]
public class MultiDialogs
{
    public DialStruct[] Dialogs;
    public bool ThereIsEvent;
}


