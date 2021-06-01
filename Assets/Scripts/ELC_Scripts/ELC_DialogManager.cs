using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ELC_DialogManager : MonoBehaviour
{
    public ELC_DialoguesSO CurrentDialSO;
    private ELC_SwitchCamFocus camSwitchScript;
    public GameObject CharacterGO;
    public Text textZone;
    public GameObject DialogGameObject;
    public GameObject ContinueButton;
    private int CurrentDialIndex;
    public bool isRandomDialog;
    public int CurrentRandomIndex;
    public float timeToWaitForeachCharInSentence;

    private void Awake()
    {
        camSwitchScript = this.GetComponent<ELC_SwitchCamFocus>();
    }

    public void StartNewDialog(ELC_DialoguesSO dialSO, bool randomDial)
    {
        DialogGameObject.SetActive(true);
        camSwitchScript.SwitchCamFocus(CharacterGO.transform);
        CurrentDialSO = dialSO;
        isRandomDialog = randomDial;

        if(isRandomDialog)
        {
            CurrentRandomIndex = Random.Range(0, CurrentDialSO.RandomDialog.Count);
        }
        
        Write(0);
    }

    private void Write(int dialIndex)
    {
        if (!isRandomDialog)
        {
            textZone.text = CurrentDialSO.Dialog[dialIndex].DialLine;

            float timeToWait = 0;
            if (isRandomDialog) timeToWait = CurrentDialSO.RandomDialog[CurrentRandomIndex].Dialogs[dialIndex].DialLine.Length * timeToWaitForeachCharInSentence;
            else timeToWait =  CurrentDialSO.Dialog[dialIndex].DialLine.Length * timeToWaitForeachCharInSentence;

            Invoke("WriteNextSentence", timeToWait);
        }
        else textZone.text = CurrentDialSO.RandomDialog[CurrentRandomIndex].Dialogs[dialIndex].DialLine;
    }

    public void WriteNextSentence()
    {
        if(CurrentDialIndex < CurrentDialSO.RandomDialog[CurrentRandomIndex].Dialogs.Length - 2 || CurrentDialIndex < CurrentDialSO.Dialog.Length - 2)
        {
            CurrentDialIndex++;
            Write(CurrentDialIndex);
        }
        else if (CurrentDialIndex == CurrentDialSO.RandomDialog[CurrentRandomIndex].Dialogs.Length - 2 || CurrentDialIndex == CurrentDialSO.Dialog.Length - 2)
        {
            LastSentence();
        }
        else
        {
            EndConversation();
        }
    }

    private string[] DecomposeSentence(int dialIndex)
    {
        string[] characters = new string[CurrentDialSO.Dialog[dialIndex].DialLine.Length];

        for (int i = 0; i < CurrentDialSO.Dialog[dialIndex].DialLine.Length; i++)
        {
            characters[i] = CurrentDialSO.Dialog[dialIndex].DialLine[i].ToString();
        }
        return characters;
    }

    private void LastSentence()
    {
        CurrentDialIndex++;
        Write(CurrentDialIndex);
        Debug.Log("Dernier dialogue");
    }

    private void EndConversation()
    {
        DialogGameObject.SetActive(false);
        CurrentDialSO = null;
        CurrentDialIndex = 0;
        CurrentRandomIndex = 0;
        isRandomDialog = false;
        camSwitchScript.CancelCamFocus();
        Debug.Log("End Conversation");
    }
}
