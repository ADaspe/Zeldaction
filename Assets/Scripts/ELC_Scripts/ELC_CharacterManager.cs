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
    public PlayerInput inputs;
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

        if(miaMove != null && followingCharacter == miaMove)
        {
            followingCharacter = spiritMove;
            miaMove.currentCharacter = false;
            spiritMove.currentCharacter = true;
            vCam.Follow = spiritMove.transform;
            miaMove.rb.velocity = Vector2.zero;
        }else if (spiritMove != null && followingCharacter == spiritMove)
        {
            followingCharacter = miaMove;
            miaMove.currentCharacter = true;
            spiritMove.currentCharacter = false;
            spiritMove.rb.velocity = Vector2.zero;
            vCam.Follow = miaMove.transform;
        }
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
}
