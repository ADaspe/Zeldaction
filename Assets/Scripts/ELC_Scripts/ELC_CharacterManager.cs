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
    public int maxHP;
    public ELC_CharacterAnimationsManager AnimationManager;
    public ELC_Interact ToPurify;
    //Variables locales
    private ELC_Attack RynAttack;
    private ELC_Attack SpiritAttack;
    [Header("Animations")]
    public string PlayerIdle;
    public string PlayerWalk;
    public string PlayerShield;
    public string PlayerAttackTogether;
    public string PlayerHit;
    public string PlayerDetachSpirit;
    public float SpiritReleaseDuration;
    public float nextDash;
    public bool spiritProjected;


    public bool xLocked;
    public bool yLocked;

    [Header ("Upgrades")]
    public bool dashPlusUpgrade;
    

    private void Awake()
    {
        vCam.Follow = RynMove.transform;
        followingCharacter = RynMove;
        RynMove.currentCharacter = true;
        RynAttack = RynGO.GetComponent<ELC_Attack>();
        SpiritAttack = SpiritGO.GetComponent<ELC_Attack>();
        currentHP = maxHP = stats.initialHP;
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
        }
    }
    public void ChangeCamRyn(InputAction.CallbackContext value)
    {
        if (value.started && !Together)
        {
            ChangeCamFocusRyn();
        }
    }
    public void ChangeCamIden(InputAction.CallbackContext value)
    {
        if (value.started && !Together)
        {
            ChangeCamFocusSpirit();
        }
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

    public void Action(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            
            if (DetectedInteraction != null && followingCharacter == RynMove)
            {
                if (DetectedInteraction.PlayerCanInteract && !DetectedInteraction.isMobile)
                {
                    DetectedInteraction.Interact.Invoke();
                }
                else if(DetectedInteraction.PlayerCanInteract && DetectedInteraction.isMobile && !DetectedInteraction.isGrabbed)
                {
                    DetectedInteraction.Interact.Invoke();
                    DetectedInteraction.isGrabbed = true;
                    RynMove.isRynGrabbing = true;
                    Vector2 vectorDiff = new Vector2(RynMove.transform.position.x - DetectedInteraction.transform.position.x, RynMove.transform.position.y-DetectedInteraction.transform.position.y);


                    if (Mathf.Abs(vectorDiff.x) >=0 && Mathf.Abs(vectorDiff.y) <= Mathf.Abs(vectorDiff.x)) // Si on est � droite de la caisse
                    {
                        //Animation � gauche
                        yLocked = true;
                    }else if (Mathf.Abs(vectorDiff.x) < 0 && Mathf.Abs(vectorDiff.y) <= Mathf.Abs(vectorDiff.x)) // Si on est � gauche de la caisse
                    {
                        //Animation � droite
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
        if (value.started)
        {
            if (Together)
            {
                DetachSpirit();
            }
            else
            {
                RegroupTogether();
                ChangeCamFocusRyn();
            }
        }
    }

    public void Pause(InputAction.CallbackContext value)
    {
        //To define
        Debug.Log("Pause yet to define");
    }

    public void IdenAttack(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            if (Together)
            {
                RynAttack.AttackTogether();
            }
            else if(Time.time >= nextDash)
            {
                SpiritAttack.SpiritDashAttack();
            }
        }
    }
    
    public void RynShield(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            RynAttack.RynShield();
        }
    }

    public void ChangeCamFocusRyn()
    {
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
        //Disabling Ryn
        RynMove.currentCharacter = false;
        RynMove.rb.velocity = Vector2.zero;
        AnimationManager.isMoving = false;
        RynMove.rawInputMovement = Vector2.zero;

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
        ResetProjection();
    }

    public void DetachSpirit()
    {
        Together = false;
        GoToRyn();
        SpiritGO.GetComponent<Collider2D>().enabled = true;
        ELC_SpiritIdle tmpIdle = SpiritGO.GetComponent<ELC_SpiritIdle>();
        tmpIdle.closeToRyn = false;
        tmpIdle.enabled = false;
    }

    public void GoToRyn()
    {
        RynMove.canMove = false;
        spiritProjected = true;
        Vector2 tempDirection = new Vector2(RynGO.transform.position.x - SpiritGO.transform.position.x, RynGO.transform.position.y - SpiritGO.transform.position.y);
        spiritMove.rb.velocity = tempDirection * spiritMove.currentSpeed;
        Invoke("ProjectSpirit", ((Mathf.Sqrt(Mathf.Pow(tempDirection.x, 2) + (Mathf.Pow(tempDirection.y, 2))) / spiritMove.currentSpeed)));
    }

    public void ProjectSpirit()
    {
        vCam.Follow = SpiritGO.transform;
        spiritMove.rb.velocity = RynMove.LastDirection.normalized * (stats.IdenProjectionDistance / stats.IdenProjectionTime);
        Invoke("SlowDownSpirit", stats.IdenProjectionTime);

    }

    public void SlowDownSpirit()
    {
        StartCoroutine(ProjectionSlowdown());
    }
    public void ResetProjection()
    {
        CancelInvoke("ProjectSpirit");
        StopCoroutine(ProjectionSlowdown());
        RynMove.canMove = true;
        spiritProjected = false;
        spiritMove.currentSpeed = stats.SpiritSpeed;

    }

    public void TakeDamage(string tag)
    {
        if (tag == "Ryn")
        {
            if (!RynAttack.ShieldOn)
            {
                currentHP--;
            }
        }
        else if (tag == "Spirit")
        {
            currentHP--;
        }
        else
        {
            Debug.Log("Tag introuvable");
        }
    }


    public IEnumerator ProjectionSlowdown()
    {
        
        float tmpTimeToReach = Time.time + stats.IdenSlowDownTime;
        while(Time.time <= tmpTimeToReach)
        {
            spiritMove.currentSpeed -= spiritMove.currentSpeed/6 ;
            spiritMove.rb.velocity = spiritMove.rb.velocity.normalized * spiritMove.currentSpeed;
            yield return new WaitForSeconds(stats.IdenSlowDownTime/20);
        }

        spiritMove.rb.velocity = Vector2.zero;
        ChangeCamFocusSpirit();
        ResetProjection();
    }
}
