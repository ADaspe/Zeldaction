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

    private void Start()
    {
        RynAnimator = CharaManager.RynGO.GetComponent<Animator>();
        RynMoves = CharaManager.RynMove;
        RynGO = CharaManager.RynGO;
    }

    private void Update()
    {
        RynAnimator.SetFloat("MovesX", RynMoves.LastDirection.x);
        RynAnimator.SetFloat("MovesY", RynMoves.LastDirection.y);
        if (RynMoves.LastDirection.x > 0) RynGO.GetComponent<SpriteRenderer>().flipX = true;
        else RynGO.GetComponent<SpriteRenderer>().flipX = false;
    }

    public void UpdateAnimations(string AnimToPlay)
    {
        if (currentAnimation == AnimToPlay) return;

        RynAnimator.Play(AnimToPlay);

        currentAnimation = AnimToPlay;
    }
}
