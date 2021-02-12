using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class ELC_CharacterManager : MonoBehaviour
{
    public bool Together;
    public CinemachineVirtualCamera vCam;
    public AXD_CharacterMove followingCharacter;
    public AXD_CharacterMove mia;
    public AXD_CharacterMove spirit;

    public void ChangeCamFocus()
    {
        if(followingCharacter == mia)
        {
            followingCharacter = spirit;
            vCam.Follow = spirit.transform;
        }else if (followingCharacter == spirit)
        {
            followingCharacter = mia;
            vCam.Follow = mia.transform;
        }
    }


}
