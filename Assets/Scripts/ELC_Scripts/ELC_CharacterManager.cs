using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class ELC_CharacterManager : MonoBehaviour
{
    public bool Together;
    public GameObject RynGO;
    public GameObject SpiritGO;
    public CinemachineVirtualCamera vCam;
    public AXD_CharacterMove followingCharacter;
    public AXD_CharacterMove RynMove;
    public AXD_CharacterMove spiritMove;
    public ELC_Interact DetectedInteraction;
    public AXD_CharacterVariablesSO stats;
    public int currentHP;
    //Variables locales
    private ELC_Attack RynAttack;
    private ELC_Attack SpiritAttack;

    private void Start()
    {
        vCam.Follow = RynMove.transform;
        followingCharacter = RynMove;
        RynMove.currentCharacter = true;
        RynAttack = RynGO.GetComponent<ELC_Attack>();
        SpiritAttack = SpiritGO.GetComponent<ELC_Attack>();
    }
    public void ChangeCamFocus(InputAction.CallbackContext value)
    {
        if (value.started && !Together)
        {
            if (RynMove != null && followingCharacter == RynMove)
            {
                ChangeCamFocusSpirit();

            }
            else if (spiritMove != null && followingCharacter == spiritMove)
            {
                ChangeCamFocusRyn();
            }
            else
            {
                Debug.Log("Nope");
            }
        }
    }

    public void ChangeCamFocusRyn()
    {
        Debug.Log("CamSwapRyn");
        //Disabling Spirit
        spiritMove.currentCharacter = false;
        spiritMove.rb.velocity = Vector2.zero;

        //Enabling Ryn
        vCam.Follow = RynMove.transform;
        followingCharacter = RynMove;
        RynMove.currentCharacter = true;
    }

    public void ChangeCamFocusSpirit()
    {
        Debug.Log("CamSwapSpirit");
        //Disabling Ryn
        RynMove.currentCharacter = false;
        RynMove.rb.velocity = Vector2.zero;

        //Enabling Spirit
        followingCharacter = spiritMove;
        spiritMove.currentCharacter = true;
        vCam.Follow = spiritMove.transform;
    }

    public void RegroupTogether()
    {
        Together = true;
        followingCharacter = RynMove;
        RynMove.currentCharacter = true;
        vCam.Follow = RynMove.transform;
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
        Debug.Log("Combat");
        if (followingCharacter == RynMove)
        {
            if (Together) RynAttack.AttackTogether();
            else RynAttack.RynShield();
        }
        else SpiritAttack.SpiritDashAttack();
    }

    public void Action(InputAction.CallbackContext value)
    {
        Debug.Log("Action");
        if (DetectedInteraction != null) if(DetectedInteraction.PlayerCanInteract) DetectedInteraction.Interact.Invoke();
    }

    public void Spirit(InputAction.CallbackContext value)
    {
        Debug.Log("Spirit");
        if (Together) { 
            DetachSpirit();
        }
        else {
            RegroupTogether();
            ChangeCamFocusRyn();
        }
    }

    public void TakeDamage(string tag)
    {
        if(tag == "Ryn")
        {
            if (!RynAttack.ShieldOn)
            {
                currentHP--;
            }
        }else if (tag == "Spirit")
        {
            currentHP--;
        }
        else
        {
            Debug.Log("Tag introuvable");
        }
    }

    public void Pause(InputAction.CallbackContext value)
    {
        //To define
        Debug.Log("Pause yet to define");
    }
}
