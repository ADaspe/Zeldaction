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
    public float currentSpeed;
    public bool currentCharacter;
    public bool camSwapOn;
    public bool isDashing;
    public bool isRynGrabbing;
    public ELC_Interact grabbebObject;
    public bool wasDashingWhenColliding;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        camSwapOn = false;
        AnimManager = charaManager.AnimationManager;
        if(this.tag == "Ryn")
        {
            currentSpeed = charaManager.stats.RynSpeed;
        }
        else if (this.tag == "Spirit")
        {
            currentSpeed = charaManager.stats.SpiritSpeed;
        }
    }

    private void Update()
    {
        if (canMove && currentCharacter && !isDashing)
        {
            rb.velocity = rawInputMovement*currentSpeed;
            if (rawInputMovement.magnitude >= 0.005f)
            {
                LastDirection = rawInputMovement.normalized; // Sauvegarder la derni�re direction dans laquelle le joueur est tourn�;
                
            }
            if (isRynGrabbing)
            {
                if (grabbebObject != null)
                {
                    grabbebObject.rbInteractObject.velocity = rawInputMovement * charaManager.stats.SpeedGrabbing;
                }
                else
                {
                    isRynGrabbing = false;
                    charaManager.xLocked = false;
                    charaManager.yLocked = false;
                    currentSpeed = charaManager.stats.RynSpeed;
                }
            }
            if (charaManager.followingCharacter == charaManager.RynMove)
            {
                RynAnimatorUpdate();
            }
        }else if (!canMove)
        {
            rb.velocity = Vector2.zero;
            RynAnimatorUpdate();
        }
    }

    private void RynAnimatorUpdate()
    {
        if(rb.velocity.magnitude >= 0.005f)
        {
            AnimManager.isMoving = true;
        }
        else
        {
            AnimManager.isMoving = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(CompareTag("Spirit") && currentCharacter && isDashing && (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle") || collision.gameObject.layer == LayerMask.NameToLayer("ObstacleSpirit")))
        {
            Debug.Log("Ceci est un mur");
            StopCoroutine(charaManager.SpiritGO.GetComponent<ELC_Attack>().DashCoroutine());
            currentSpeed = charaManager.stats.SpiritSpeed;
            isDashing = false;
        }
        else if(CompareTag("Spirit") && currentCharacter && isDashing && collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            Debug.Log("Ceci est un ennemi");
            collision.gameObject.GetComponent<AXD_EnemyHealth>().GetHit(charaManager.stats.StunTime);
        }
    }
}
