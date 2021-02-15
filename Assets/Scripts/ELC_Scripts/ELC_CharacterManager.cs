using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class ELC_CharacterManager : MonoBehaviour
{
    public bool Together;
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
        }else if (spiritMove != null && followingCharacter == spiritMove)
        {
            followingCharacter = miaMove;
            miaMove.currentCharacter = true;
            spiritMove.currentCharacter = false;
            vCam.Follow = miaMove.transform;
        }
    }
}
