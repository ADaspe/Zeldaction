using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class ELC_CharacterManager : MonoBehaviour
{
    public bool Together;
    public GameObject MiaGO;
    public GameObject SpiritGO;
    public CinemachineVirtualCamera vCam;
    public AXD_CharacterMove followingCharacter;
    public AXD_CharacterMove miaMove;
    public AXD_CharacterMove spiritMove;

    private void Start()
    {
        vCam.Follow = miaMove.transform;
        followingCharacter = miaMove;
        miaMove.currentCharacter = true;
    }
    public void ChangeCamFocus(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            Debug.Log("CamSwap");
            if (miaMove != null && followingCharacter == miaMove)
            {
                Debug.Log("CamSwapSpirit");
                //Disabling Mia
                miaMove.currentCharacter = false;
                miaMove.rb.velocity = Vector2.zero;

                //Enabling Spirit
                followingCharacter = spiritMove;
                spiritMove.currentCharacter = true;
                vCam.Follow = spiritMove.transform;

            }
            else if (spiritMove != null && followingCharacter == spiritMove)
            {
                Debug.Log("CamSwapMia");
                //Disabling Spirit
                spiritMove.currentCharacter = false;
                spiritMove.rb.velocity = Vector2.zero;

                //Enabling Mia
                vCam.Follow = miaMove.transform;
                followingCharacter = miaMove;
                miaMove.currentCharacter = true;

            }
            else
            {
                Debug.Log("Nope");
            }
        }
    }

    public void RegroupOrDetach()
    {
        if (!Together) DetachSpirit();
        else RegroupTogether();
        
    }

    public void RegroupTogether()
    {
        Together = true;
        followingCharacter = miaMove;
        miaMove.currentCharacter = true;
        vCam.Follow = miaMove.transform;
        SpiritGO.GetComponent<Collider2D>().enabled = false;
        SpiritGO.GetComponent<ELC_SpiritIdle>().enabled = true;
    }

    public void DetachSpirit()
    {
        Together = false;
        SpiritGO.GetComponent<Collider2D>().enabled = true;
        SpiritGO.GetComponent<ELC_SpiritIdle>().enabled = false;
    }

    public void Move(InputAction.CallbackContext value)
    {
        Vector2 inputMovement = value.ReadValue<Vector2>() * followingCharacter.speed;
        followingCharacter.rawInputMovement = new Vector2(inputMovement.x, inputMovement.y);
    }

    public void Combat(InputAction.CallbackContext value)
    {
        //To define
        Debug.Log("Combat yet to define");
    }

    public void Action(InputAction.CallbackContext value)
    {
        //To define
        Debug.Log("Action yet to define");
    }

    public void Spirit(InputAction.CallbackContext value)
    {
        //To define
        Debug.Log("Spirit yet to define");
    }

    public void Pause(InputAction.CallbackContext value)
    {
        //To define
        Debug.Log("Pause yet to define");
    }
}
