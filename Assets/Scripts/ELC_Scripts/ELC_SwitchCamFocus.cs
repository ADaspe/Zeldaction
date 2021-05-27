using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class ELC_SwitchCamFocus : MonoBehaviour
{
    public ELC_CharacterManager CharaMana;
    public CinemachineVirtualCamera vCam;
    public PlayerInput Inputs;

    public void SwitchCamFocus(Transform focus, bool PlayerCanMove = true)
    {
        vCam.Follow = focus;
        if (!PlayerCanMove) Inputs.enabled = false;
    }

    public void CancelCamFocus()
    {
        Inputs.enabled = true;
        if(CharaMana.followingCharacter == CharaMana.RynMove)
        {
            vCam.Follow = CharaMana.RynGO.transform;
        }
        else if(CharaMana.followingCharacter == CharaMana.spiritMove)
        {
            vCam.Follow = CharaMana.SpiritGO.transform;
        }
        
    }
}
