using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELC_CharacterAnimationsManager : MonoBehaviour
{
    public ELC_CharacterManager CharaManager;
    private Animator RynAnimator;
    private string currentAnimation;
    private AXD_CharacterMove RynMoves;
    private GameObject RynGO;
    public bool isMoving;
    public bool isAttacking;
    public bool shielding;
    private bool together;
    public bool isPushingObjects;

    private void Start()
    {
        RynAnimator = CharaManager.RynGO.GetComponent<Animator>();
        RynMoves = CharaManager.RynMove;
        RynGO = CharaManager.RynGO;
    }

    private void Update()
    {
        if(!isPushingObjects) UpdateTurns();
        ManageAnimations();
    }

    private void ManageAnimations()
    {
        isPushingObjects = RynMoves.isRynGrabbing;
        together = CharaManager.Together;
        if (CharaManager.currentHP <= 0)
        {
            
            StartCoroutine(RynDeathAnim());
            return;
        }
        if (isAttacking)
        {
            if (!together)
            {
                StartCoroutine("ShieldAnim");
            }
            else
            {
                StartCoroutine("SpiritReleaseAnim");
            }
            return;
        }

        if(isPushingObjects)
        {
            UpdateAnimations(CharaManager.PlayerPushObject);
            return;
        }


        if(isMoving)
        {
            UpdateAnimations(CharaManager.PlayerWalk);
            return;
        }
        else
        {
            UpdateAnimations(CharaManager.PlayerIdle);
        }
        
    }

    private void UpdateTurns()
    {
        RynAnimator.SetFloat("MovesX", RynMoves.LastDirection.x);
        RynAnimator.SetFloat("MovesY", RynMoves.LastDirection.y);
        if (RynMoves.LastDirection.x > 0) RynGO.GetComponent<SpriteRenderer>().flipX = true;
        else RynGO.GetComponent<SpriteRenderer>().flipX = false;
    }

    IEnumerator ShieldAnim()
    {
        UpdateAnimations(CharaManager.PlayerShield);
        yield return new WaitForSeconds(CharaManager.stats.ShieldDuration);
        isAttacking = false;
    }

    IEnumerator SpiritReleaseAnim()
    {
        UpdateAnimations(CharaManager.PlayerDetachSpirit);
        yield return new WaitForSeconds(CharaManager.SpiritReleaseDuration);
        isAttacking = false;
    }

    IEnumerator RynDeathAnim()
    {
        Debug.Log("Lol t mor");
        UpdateAnimations(CharaManager.PlayerDeath);
        yield return new WaitForSeconds(0.5f);
    }

    private void  PushObjects()
    {
        UpdateAnimations(CharaManager.PlayerPushObject);

    }

    public void UpdateAnimations(string AnimToPlay)
    {
        if (currentAnimation == AnimToPlay) return;

        RynAnimator.Play(AnimToPlay);

        currentAnimation = AnimToPlay;
    }
}
