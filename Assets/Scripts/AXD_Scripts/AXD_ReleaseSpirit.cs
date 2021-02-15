using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AXD_ReleaseSpirit : MonoBehaviour
{
    public AXD_CharacterMove miaMoves;
    public ELC_CharacterManager characterManager;

    private void Start()
    {
        miaMoves = GetComponent<AXD_CharacterMove>();
        characterManager = GetComponent<ELC_CharacterManager>();
    }

    public void SpiritAction(InputAction.CallbackContext value)
    {
        if (characterManager.Together)
        {
            ReleaseSpirit();
        }
        else
        {
            CallBackSpirit();
        }
    }

    public void ReleaseSpirit()
    {
        //To define
        Debug.Log("Yet to define");
    }

    public void CallBackSpirit()
    {
        //To define
        Debug.Log("Yet to define");
    }
}
