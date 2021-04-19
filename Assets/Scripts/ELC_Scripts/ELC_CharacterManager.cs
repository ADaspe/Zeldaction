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
    public ELC_CharacterAnimationsManager AnimationManager;
    public ELC_Interact ToPurify;
    //Variables locales
    private ELC_Attack RynAttack;
    private ELC_Attack SpiritAttack;
    public string PlayerIdle;
    public string PlayerWalk;
    public string PlayerShield;
    public string PlayerAttackTogether;
    public string PlayerHit;
    public bool xLocked;
    public bool yLocked;


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
        //SpiritGO.GetComponent<Collider2D>().enabled = false;
        SpiritGO.GetComponent<ELC_SpiritIdle>().enabled = true;
    }

    public void DetachSpirit()
    {
        Together = false;
        spiritMove.rb.velocity = Vector2.zero; //En attendant d'avoir la projection de l'esprit, on bloque son déplacement quand on le détache
        SpiritGO.GetComponent<Collider2D>().enabled = true;
        SpiritGO.GetComponent<ELC_SpiritIdle>().enabled = false;
    }

    public void Move(InputAction.CallbackContext value)
    {
        if (followingCharacter.canMove)
        {
            Vector2 inputMovement = value.ReadValue<Vector2>() * followingCharacter.currentSpeed;
            if (xLocked)
            {
                followingCharacter.rawInputMovement = new Vector2(0, inputMovement.y).normalized;
            }
            else if (yLocked)
            {
                followingCharacter.rawInputMovement = new Vector2(inputMovement.x, 0).normalized;
            }
            else
            {
                followingCharacter.rawInputMovement = new Vector2(inputMovement.x, inputMovement.y).normalized;
            }
            

        }
        if (value.canceled)
        {
            followingCharacter.rawInputMovement = Vector2.zero;
        }
    }

    public void Combat(InputAction.CallbackContext value)
    {
        
        if (value.started)
        {
            //Debug.Log("Combat");
            if (followingCharacter == RynMove)
            {
                if (Together) RynAttack.AttackTogether();
                else RynAttack.RynShield();
            }
            else SpiritAttack.SpiritDashAttack();
        }
    }

    public void Action(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            Debug.Log("Action");
            if (DetectedInteraction != null && followingCharacter == RynMove)
            {
                if (DetectedInteraction.PlayerCanInteract && !DetectedInteraction.isMobile)
                {
                    DetectedInteraction.Interact.Invoke();
                }else if(DetectedInteraction.PlayerCanInteract && DetectedInteraction.isMobile && !DetectedInteraction.isGrabbed)
                {
                    DetectedInteraction.isGrabbed = true;
                    RynMove.isRynGrabbing = true;
                    Vector2 vectorDiff = new Vector2(RynMove.transform.position.x - DetectedInteraction.transform.position.x, RynMove.transform.position.y-DetectedInteraction.transform.position.y);


                    if (Mathf.Abs(vectorDiff.x) >=0 && Mathf.Abs(vectorDiff.y) <= Mathf.Abs(vectorDiff.x)) // Si on est à droite de la caisse
                    {
                        //Animation à gauche
                        yLocked = true;
                    }else if (Mathf.Abs(vectorDiff.x) < 0 && Mathf.Abs(vectorDiff.y) <= Mathf.Abs(vectorDiff.x)) // Si on est à gauche de la caisse
                    {
                        //Animation à droite
                        yLocked = true;
                    }
                    else if (Mathf.Abs(vectorDiff.y) >= 0 && Mathf.Abs(vectorDiff.x) <= Mathf.Abs(vectorDiff.y)) // Si on est au dessus de la caisse
                    {
                        //Animation en bas
                        xLocked = true;
                    }else if (Mathf.Abs(vectorDiff.y) < 0 && Mathf.Abs(vectorDiff.x) <= Mathf.Abs(vectorDiff.y)) // Si on est au dessous de la caisse
                    {
                        //Animation en haut
                        xLocked = true;
                    }
                    else
                    {
                        Debug.Log("Kestuveu frr?");
                    }

                    RynMove.currentSpeed = stats.SpeedGrabbing;
                    RynMove.grabbebObject = DetectedInteraction;
                }

            }
            else if (ToPurify != null && followingCharacter == spiritMove)
            {
                ToPurify.Purify();
            }
        }
        if (value.canceled)
        {
            Debug.Log("Action stop");
            if (DetectedInteraction != null && DetectedInteraction.isGrabbed)
            {
                DetectedInteraction.isGrabbed = false;
                RynMove.isRynGrabbing = false;
                xLocked = yLocked = false;
                RynMove.currentSpeed = stats.RynSpeed;
                RynMove.grabbebObject.rbInteractObject.velocity = Vector2.zero;
                RynMove.grabbebObject = null;
            }
        }
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
