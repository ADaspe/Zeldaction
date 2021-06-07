using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;
using UnityEngine.Events;
using Sirenix.OdinInspector;

public class ELC_CharacterManager : MonoBehaviour
{
    [Header("References")]
    public GameObject RynGO;
    public GameObject SpiritGO;
    public CinemachineVirtualCamera vCam;
    public AXD_CharacterMove followingCharacter;
    public AXD_CharacterMove RynMove;
    public AXD_CharacterMove spiritMove;
    public AXD_CharacterVariablesSO stats;
    private ELC_SpiritIdle spiritIdle;
    public ELC_GameManager gameManager;
    public ELC_CharacterAnimationsManager AnimationManager;
    public AXD_CheckPoint LastCheckPointBeforeBoss;
    [HideInInspector]
    public ELC_Interact ToPurify;
    [HideInInspector]
    public ELC_Attack RynAttack;
    [HideInInspector]
    public ELC_Attack SpiritAttack;
    [Header("Movements")]
    public bool Together;
    public bool xLocked;
    public bool yLocked;
    [Header("Animations")]
    public Animator RynAnimator;
    public Animator IdenAnimator;
    public string PlayerIdle;
    public string PlayerWalk;
    public string PlayerShield;
    public string PlayerAttackTogether;
    public string PlayerHit;
    public string PlayerDetachSpirit;
    public string PlayerDeath;
    public string PlayerPushObject;
    [Header("Menu")]
    public GameObject PauseMenu;
    public UnityEvent enableMenu;
    public UnityEvent disableMenu;
    public bool toggleMenu;


    [Header("Health")]

    [ReadOnly]
    public bool invincibilityCheat;
    public int currentHP;
    [HideInInspector]
    public AXD_Health health;
    public int maxHP;
    public bool isDead;
    [Header("Upgrades")]
    [ReadOnly]
    public bool returnUpgrade;
    [ReadOnly]
    public bool dashPlusUpgrade;
    [ReadOnly]
    public bool purificationUpgrade;

    [HideInInspector]
    public float SpiritReleaseDuration;
    [HideInInspector]
    public float nextDash;
    //[HideInInspector]
    public bool spiritProjected;
    [HideInInspector]
    public AXD_CheckPoint lastCheckPoint;
    [HideInInspector]
    private Collider2D[] allDetected;
    
    public ELC_Interact DetectedInteraction;
    private bool ticTacEnabled;
    private float timeToTeleportTooFar;
    private float timeToRynScared;
    [SerializeField]
    private bool toggleCheats;

    private void Awake()
    {
        vCam.Follow = RynMove.transform;
        followingCharacter = RynMove;
        RynMove.currentCharacter = true;
        RynAttack = RynGO.GetComponent<ELC_Attack>();
        SpiritAttack = SpiritGO.GetComponent<ELC_Attack>();
        spiritIdle = SpiritGO.GetComponent<ELC_SpiritIdle>();
        RynAnimator = RynGO.GetComponent<Animator>();
        IdenAnimator = SpiritGO.GetComponent<Animator>();
        health = RynGO.GetComponent<AXD_Health>();
        currentHP = maxHP = stats.initialHP;
        ticTacEnabled = false;
        timeToTeleportTooFar = 0;
        timeToRynScared = 0;
        invincibilityCheat = false;
    }

    private void FixedUpdate()
    {
        if(Vector2.Distance(RynGO.transform.position, SpiritGO.transform.position) > stats.SeparationDistance)
        {
            if(!ticTacEnabled)
            {
                ticTacEnabled = true;
                gameManager.audioManager.Play("TicTac");
                
            }
            if (timeToRynScared-stats.SFXRynScaredFrequency > 0)
            {
                gameManager.audioManager.Play("Ryn_Scared" + Random.Range(1, 9));
                timeToRynScared = 0;
            }
            else
            {
                timeToRynScared += Time.deltaTime;
            }
            
            if(timeToTeleportTooFar >= stats.SeparationTime)
            {
                spiritIdle.Teleport();
                RegroupTogether();
                timeToTeleportTooFar = 0;
            }
            else
            {
                timeToTeleportTooFar += Time.deltaTime;
            }
        }
        else
        {
            if (ticTacEnabled)
            {
                ticTacEnabled = false;

                gameManager.audioManager.Stop("TicTac");
            }
           
            
        }
    }
    public void ChangeCamFocus(InputAction.CallbackContext value)
    {
        if (value.started && !Together && !toggleMenu)
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
        if (value.started && !Together && !toggleMenu)
        {
            ChangeCamFocusRyn();
        }
    }
    public void ChangeCamIden(InputAction.CallbackContext value)
    {
        if (value.started && !Together && !toggleMenu)
        {
            ChangeCamFocusSpirit();
        }
    }

    public void Move(InputAction.CallbackContext value)
    {
        if (followingCharacter.canMove && !toggleMenu)
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
        if (value.canceled && !toggleMenu)
        {
            followingCharacter.rawInputMovement = Vector2.zero;
        }
    }

    public void Action(InputAction.CallbackContext value)
    {
        if (value.started && !toggleMenu)
        {

            if (DetectedInteraction != null && followingCharacter == RynMove)
            {
                if (DetectedInteraction.PlayerCanInteract && !DetectedInteraction.isMobile)
                {
                    DetectedInteraction.Interact.Invoke();
                }
                else if (DetectedInteraction.PlayerCanInteract && DetectedInteraction.isMobile && !DetectedInteraction.isGrabbed)
                {
                    DetectedInteraction.Interact.Invoke();
                    DetectedInteraction.isGrabbed = true;
                    RynMove.isRynGrabbing = true;
                    Vector2 vectorDiff = new Vector2(RynMove.transform.position.x - DetectedInteraction.transform.position.x, RynMove.transform.position.y - DetectedInteraction.transform.position.y);
                    if (Mathf.Abs(vectorDiff.x) >= 0 && Mathf.Abs(vectorDiff.y) <= Mathf.Abs(vectorDiff.x)) // Si on est � droite de la caisse
                    {
                        //Animation � gauche
                        yLocked = true;
                    } else if (Mathf.Abs(vectorDiff.x) < 0 && Mathf.Abs(vectorDiff.y) <= Mathf.Abs(vectorDiff.x)) // Si on est � gauche de la caisse
                    {
                        //Animation � droite
                        yLocked = true;
                    }
                    else if (Mathf.Abs(vectorDiff.y) >= 0 && Mathf.Abs(vectorDiff.x) <= Mathf.Abs(vectorDiff.y)) // Si on est au dessus de la caisse
                    {
                        //Animation en bas
                        xLocked = true;
                    } else if (Mathf.Abs(vectorDiff.y) < 0 && Mathf.Abs(vectorDiff.x) <= Mathf.Abs(vectorDiff.y)) // Si on est au dessous de la caisse
                    {
                        //Animation en haut
                        xLocked = true;
                    }

                    RynMove.currentSpeed = stats.SpeedGrabbing;
                    RynMove.grabbedObject = DetectedInteraction;
                }

            }
            else if (ToPurify != null && followingCharacter == spiritMove && purificationUpgrade)
            {
                gameManager.audioManager.Play("Spirit_Purification");
                ToPurify.Purify();
            }
            if (followingCharacter == RynMove && DetectedInteraction == null)
            {
                int tempEnemyNumber = 0;
                allDetected = Physics2D.OverlapCircleAll(RynGO.transform.position, stats.pacificationRadius, LayerMask.GetMask("Enemy","Boss"));
                foreach (Collider2D item in allDetected)
                {
                    if (item.CompareTag("Enemy") && tempEnemyNumber<stats.maxEnemyPacification)
                    {
                        tempEnemyNumber++;
                        item.gameObject.GetComponent<AXD_EnemyHealth>().Pacificate();
                    }
                    else if(item.CompareTag("Boss") && tempEnemyNumber < stats.maxEnemyPacification)
                    {
                        gameManager.audioManager.Play("Pacification");
                        tempEnemyNumber++;
                        item.gameObject.GetComponent<ELC_BossHealth>().Pacificate();
                    }else if (item.CompareTag("Totem"))
                    {
                        item.gameObject.GetComponent<ELC_Totem>().Pacificate();
                        
                    }
                }
            }
        }
        if (value.canceled && !toggleMenu)
        {
            Debug.Log("Allo ?");
            Debug.Log("Detected interraction : " + DetectedInteraction);
            Debug.Log("Is grabbed : " + DetectedInteraction.isGrabbed);
            if (DetectedInteraction != null && DetectedInteraction.isGrabbed)
            {
                Debug.Log("Oui ?");
                DetectedInteraction.isGrabbed = false;
                RynMove.isRynGrabbing = false;
                xLocked = yLocked = false;
                RynMove.currentSpeed = stats.RynSpeed;
                RynMove.grabbedObject.rbInteractObject.velocity = Vector2.zero;
                RynMove.grabbedObject.ResetLocks();
                RynMove.grabbedObject = null;
            }
        }
    }

    public void Spirit(InputAction.CallbackContext value)
    {
        if (value.started && !toggleMenu)
        {
            if (Together)
            {
                DetachSpirit();
            }
            else
            {
                if (returnUpgrade)
                {
                    RegroupTogether();
                    ChangeCamFocusRyn();
                }
                
            }
        }
    }

    public void Pause(InputAction.CallbackContext value)
    {
        if (value.started) { 
            if (!toggleMenu)
            {
            
                EnableMenu();
            }
            else
            {
                DisableMenu();
            }
        }
    }

    public void IdenAttack(InputAction.CallbackContext value)
    {
        if (value.started && !toggleMenu)
        {
            if (Together)
            {
                RynAttack.AttackTogether();
            }
            else if(Time.time >= nextDash && followingCharacter == spiritMove)
            {
                SpiritAttack.SpiritDashAttack();
            }else if (Time.time < nextDash && followingCharacter == spiritMove)
            {
                gameManager.audioManager.Play("Capacity_Cooldown");
            }
        }
    }
    
    public void RynShield(InputAction.CallbackContext value)
    {
        if (value.started && !toggleMenu && !Together)
        {
            RynAttack.RynShield();
        }
    }

    public void TPCheat(InputAction.CallbackContext value)
    {
        if (value.started && toggleCheats)
        {
            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
            mousePos.z = 0;
            RynGO.transform.position = mousePos;
        }
    }

    public void InvincibilityCheat(InputAction.CallbackContext value)
    {
        if (value.started && toggleCheats)
        {
            invincibilityCheat = !invincibilityCheat;
        }
    }

    public void ReturnUpgradeCheat(InputAction.CallbackContext value)
    {
        if (value.started && toggleCheats)
        {
            UpgradeReturn();
        }
    }
    public void PurificationUpgradeCheat(InputAction.CallbackContext value)
    {
        if (value.started &&toggleCheats)
        {
            UpgradePurification();
        }
    }
    public void DashPlusUpgradeCheat(InputAction.CallbackContext value)
    {
        if (value.started && toggleCheats)
        {
            UpgradeDash();
        }
    }

    public void TPBossCheat(InputAction.CallbackContext value)
    {
        if (value.started && toggleCheats)
        {
            RynGO.transform.position = LastCheckPointBeforeBoss.GetSpawnPosition();
            spiritIdle.Teleport(spiritIdle.targetPos);
        }
    }

    public void ToggleCheats()
    {
        toggleCheats = !toggleCheats;
    }

    public void EnableMenu() 
    {
        toggleMenu = true;
        enableMenu.Invoke();
    }

    public void DisableMenu()
    {
        toggleMenu = false;
        disableMenu.Invoke();

    }
    public void ChangeCamFocusRyn()
    {
        gameManager.audioManager.Play("CamSwap");
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
        gameManager.audioManager.Play("CamSwap");
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
        //Debug.Log("Regroup");
        gameManager.audioManager.Stop("Spirit_Move");
        gameManager.audioManager.Play("Spirit_Return");
        Together = true;
        followingCharacter = RynMove;
        RynMove.currentCharacter = true;
        vCam.Follow = RynMove.transform;
        //SpiritGO.GetComponent<Collider2D>().enabled = false;
        SpiritGO.GetComponent<ELC_SpiritIdle>().disabled = false;
        ResetProjection();
    }

    public void DetachSpirit()
    {

        if (!spiritIdle.stuck)
        {
            gameManager.audioManager.Play("Spirit_Move");
            Together = false;
            RynMove.currentCharacter = false;
            GoToRyn();
            SpiritGO.GetComponent<Collider2D>().enabled = true;
            ELC_SpiritIdle tmpIdle = SpiritGO.GetComponent<ELC_SpiritIdle>();
            tmpIdle.disabled = true;
        }
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
        IdenAnimator.SetBool("Ball", false);
        IdenAnimator.SetBool("Dash", true);
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
        IdenAnimator.SetBool("Idle", true);
        IdenAnimator.SetBool("Ball", true);
        RynMove.canMove = true;
        spiritProjected = false;
        spiritMove.currentSpeed = stats.SpiritSpeed;

    }

    public bool TakeDamage(string tag)
    {
        if (tag == "Ryn")
        {
            if (!RynAttack.ShieldOn)
            {
                currentHP--;
                if (currentHP <= 0)
                {
                    Die();
                }
                return true;
            }
        }
        else if (tag == "Spirit")
        {
            //currentHP--;
            spiritIdle.Teleport(spiritIdle.targetPos);
            return false;
        }
        return false;
        
    }

    public void Die()
    {
        Debug.Log("Die");
        isDead = true;
        RynMove.canMove = false;
        Invoke("TeleportToLastCheckPoint", 1f);
    }

    public void TeleportToLastCheckPoint()
    {
        RynGO.transform.position = lastCheckPoint.GetSpawnPosition();
        RynMove.canMove = true;
        spiritIdle.Teleport();
        health.FullHeal();

    }

    [Button]
    public void UpgradeDash()
    {

        dashPlusUpgrade = true;
        gameManager.audioManager.Play("Jingle_Spirit");
    }
    [Button]
    public void UpgradePurification()
    {

        purificationUpgrade = true;
        gameManager.audioManager.Play("Jingle_Spirit");
    }
    [Button]
    public void UpgradeReturn()
    {

        returnUpgrade = true;
        gameManager.audioManager.Play("Jingle_Spirit");
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere(RynGO.transform.position, stats.pacificationRadius);
    }
}
