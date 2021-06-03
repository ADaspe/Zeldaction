using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ELC_Dialog : MonoBehaviour
{
    public ELC_DialoguesSO Dialogue;
    public ELC_DialogManager DialMana;
    public bool HasAlreadyTalk;
    public Evenements[] events;
    public bool isInEvent;

    public void Dialog()
    {
        DialMana.dialScript = this;
        DialMana.CharacterGO = this.gameObject;
        DialMana.StartNewDialog(Dialogue, HasAlreadyTalk);
        HasAlreadyTalk = true;
    }

    public IEnumerator checkEvents(bool EndConversation = false)
    {
        isInEvent = true;
        Evenements Event = null;

        foreach (var item in events)
        {
            if(DialMana.CurrentDialogIndex == item.TriggerAtDialIndex)
            {
                Debug.Log("oui laaaaa");
                Event = item;
                continue;
            }
        }

        if(Event != null)
        {
            if(Event.evenement != null) Event.evenement.Invoke();
            if (Event.SwitchCamera)
            {
                DialMana.camSwitchScript.SwitchCamFocus(Event.ObjectToSwitchOn);
            }
            yield return new WaitForSeconds(Event.EventDuration);
            if (Event.SwitchCamera)
            {
                DialMana.camSwitchScript.CancelCamFocus();
                if (!EndConversation) DialMana.camSwitchScript.SwitchCamFocus(this.transform);
            }
        }

        isInEvent = false;


    }

    [System.Serializable]
    public class Evenements
    {
        public int TriggerAtDialIndex;
        public UnityEvent evenement;
        public bool SwitchCamera;
        public Transform ObjectToSwitchOn;
        public float EventDuration;
    }
}
