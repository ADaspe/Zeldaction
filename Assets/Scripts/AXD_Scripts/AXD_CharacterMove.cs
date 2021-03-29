using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AXD_CharacterMove : MonoBehaviour
{
    public Rigidbody2D rb;
    public ELC_CharacterManager charaManager;
    private ELC_CharacterAnimationsManager AnimManager;
    public Vector2 rawInputMovement;
    public Vector2 LastDirection;
    public bool canMove;
    public float speed;
    public bool currentCharacter;
    public bool camSwapOn;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        camSwapOn = false;
        AnimManager = charaManager.AnimationManager;
    }

    private void Update()
    {
        if (canMove && currentCharacter)
        {
            rb.velocity = rawInputMovement;
            if (rawInputMovement.magnitude >= 0.005f)
            {
                LastDirection = rawInputMovement.normalized; // Sauvegarder la derni�re direction dans laquelle le joueur est tourn�;
                AnimManager.UpdateAnimations(charaManager.PlayerWalk);
            }
            else
            {
                AnimManager.UpdateAnimations(charaManager.PlayerIdle);
            }

        }
    }    
}
