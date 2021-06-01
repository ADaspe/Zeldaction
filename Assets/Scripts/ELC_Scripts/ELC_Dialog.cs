using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELC_Dialog : MonoBehaviour
{
    public ELC_DialoguesSO Dialogue;
    public ELC_DialogManager DialMana;
    public bool HasAlreadyTalk;

    public void Dialog()
    {
        DialMana.CharacterGO = this.gameObject;
        DialMana.StartNewDialog(Dialogue, HasAlreadyTalk);
        
        HasAlreadyTalk = true;
    }

}
