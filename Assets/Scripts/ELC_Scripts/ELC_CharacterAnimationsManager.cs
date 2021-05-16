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

    private void Start()
    {
        RynAnimator = CharaManager.RynGO.GetComponent<Animator>();
        RynMoves = CharaManager.RynMove;
        RynGO = CharaManager.RynGO;
    }

    private void Update()
    {
        UpdateTurns();
        ManageAnimations();
    }

    private void ManageAnimations()
    {
        together = CharaManager.Together;
        if(isAttacking)
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

    public void UpdateAnimations(string AnimToPlay)
    {
        if (currentAnimation == AnimToPlay) return;

        RynAnimator.Play(AnimToPlay);

        currentAnimation = AnimToPlay;
    }
}
