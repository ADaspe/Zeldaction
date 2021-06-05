using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ELC_DialogManager : MonoBehaviour
{
    public ELC_DialoguesSO CurrentDialSO;
    [HideInInspector]
    public ELC_SwitchCamFocus camSwitchScript;
    public AudioManager SoundMana;
    public GameObject CharacterGO;
    [HideInInspector]
    public ELC_Dialog dialScript;
    public Text textZone;
    public GameObject DialogGameObject;
    public GameObject ContinueButton;
    public GameObject[] ElementsToDisable;
    public Sprite PortraitRyn;
    public Image image;
    public int CurrentLineIndex;
    public int CurrentDialogIndex;
    public bool isRandomDialog;
    public int CurrentRandomIndex;
    public float timeToWaitForeachCharInSentence;

    private void Awake()
    {
        camSwitchScript = this.GetComponent<ELC_SwitchCamFocus>();
    }

    public void StartNewDialog(ELC_DialoguesSO dialSO, bool randomDial)
    {
        foreach (GameObject item in ElementsToDisable)
        {
            item.SetActive(false);
        }
        DialogGameObject.SetActive(true);
        camSwitchScript.SwitchCamFocus(CharacterGO.transform);
        CurrentDialSO = dialSO;
        isRandomDialog = randomDial;
        CurrentLineIndex = 0;
        CurrentDialogIndex = 0;
        if (isRandomDialog)
        {
            CurrentRandomIndex = Random.Range(0, CurrentDialSO.RandomDialog.Count);
        }

        Write(0);
    }

    private void Write(int dialIndex)
    {
        if (!isRandomDialog)
        {
            textZone.text = CurrentDialSO.Dialog[CurrentDialogIndex].Dialogs[dialIndex].DialLine;
            if(CurrentDialSO.Dialog[CurrentDialogIndex].Dialogs[dialIndex].Sound.Length != 0) SoundMana.Play(CurrentDialSO.Dialog[CurrentDialogIndex].Dialogs[dialIndex].Sound);


            if (CurrentDialSO.Dialog[CurrentDialogIndex].Dialogs[dialIndex].RynSentence)
            {
                image.sprite = PortraitRyn;
                //image.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(PortraitRyn.rect.width, PortraitRyn.rect.height);
            }
            else
            {
                image.sprite = CurrentDialSO.MiniaturePerso;
                //image.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(CurrentDialSO.MiniaturePerso.rect.width, CurrentDialSO.MiniaturePerso.rect.height);
            }
        }
        else
        {
            textZone.text = CurrentDialSO.RandomDialog[CurrentRandomIndex].Dialogs[dialIndex].DialLine;

            if (CurrentDialSO.RandomDialog[CurrentRandomIndex].Dialogs[dialIndex].RynSentence) image.sprite = PortraitRyn;
            else image.sprite = CurrentDialSO.MiniaturePerso;

            if(CurrentDialSO.RandomDialog[CurrentRandomIndex].Dialogs[dialIndex].Sound.Length != 0) SoundMana.Play(CurrentDialSO.RandomDialog[CurrentRandomIndex].Dialogs[dialIndex].Sound);
        }

        if (CurrentDialSO.AutoSkip)
        {
            ContinueButton.SetActive(false);
            float timeToWait = 0;

            if (isRandomDialog) timeToWait = CurrentDialSO.RandomDialog[CurrentRandomIndex].Dialogs[dialIndex].DialLine.Length * timeToWaitForeachCharInSentence;
            else timeToWait = CurrentDialSO.Dialog[CurrentDialogIndex].Dialogs[dialIndex].DialLine.Length * timeToWaitForeachCharInSentence;
            Invoke("WriteNextSentence", timeToWait);
        }
        else ContinueButton.SetActive(true);

    }

    public void WriteNextSentence()
    {
        if(!isRandomDialog)
        {
            if(CurrentLineIndex < CurrentDialSO.Dialog[CurrentDialogIndex].Dialogs.Length - 2)
            {
                CurrentLineIndex++;
                Write(CurrentLineIndex);
            }
            else if(CurrentLineIndex == CurrentDialSO.Dialog[CurrentDialogIndex].Dialogs.Length - 2)
            {
                LastSentence();
            }
            else
            {
                if (CurrentDialogIndex < CurrentDialSO.Dialog.Count - 1)
                {
                    StartCoroutine(NextDialog());
                }
                else StartCoroutine(EndConversation());
            }
            
        }
        else
        {
            if (CurrentLineIndex < CurrentDialSO.RandomDialog[CurrentRandomIndex].Dialogs.Length - 2)
            {
                CurrentLineIndex++;
                Write(CurrentLineIndex);
            }
            else if(CurrentLineIndex == CurrentDialSO.RandomDialog[CurrentRandomIndex].Dialogs.Length - 2)
            {
                LastSentence();
            }
            else
            {
                StartCoroutine(EndConversation());
            }
        }
    }

    public IEnumerator NextDialog()
    {
        CurrentLineIndex = 0;
        if (!CurrentDialSO.AutoSkip) ContinueButton.SetActive(false);
        StartCoroutine(dialScript.checkEvents());

        yield return new WaitWhile(() => dialScript.isInEvent);

        CurrentDialogIndex++;
        Write(0);
    }

    private string[] DecomposeSentence(int dialIndex)
    {
        string[] characters = new string[CurrentDialSO.Dialog[CurrentDialogIndex].Dialogs[dialIndex].DialLine.Length];

        for (int i = 0; i < CurrentDialSO.Dialog[CurrentDialogIndex].Dialogs[dialIndex].DialLine.Length; i++)
        {
            characters[i] = CurrentDialSO.Dialog[CurrentDialogIndex].Dialogs[dialIndex].DialLine[i].ToString();
        }
        return characters;
    }

    private void LastSentence()
    {
        CurrentLineIndex++;
        Write(CurrentLineIndex);
        Debug.Log("Dernier dialogue");
    }

    private IEnumerator EndConversation()
    {
        DialogGameObject.SetActive(false);

        StartCoroutine(dialScript.checkEvents(true));

        yield return new WaitWhile(() => dialScript.isInEvent);

        foreach (GameObject item in ElementsToDisable)
        {
            item.SetActive(true);
        }
        CurrentDialSO = null;
        CurrentDialogIndex = 0;
        CurrentLineIndex = 0;
        CurrentRandomIndex = 0;
        isRandomDialog = false;
        camSwitchScript.CancelCamFocus();
        Debug.Log("End Conversation");
    }
}
