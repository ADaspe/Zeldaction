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
    public ELC_Interact grabbedObject;
    public bool wasDashingWhenColliding;
    private Vector2 tempDirMultiplier;

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
                if (grabbedObject != null)
                {
                    
                    tempDirMultiplier.x = tempDirMultiplier.y = 1;
                    if ((grabbedObject.rightLock && rawInputMovement.x > 0) || rb.velocity.x == 0)
                    {
                        tempDirMultiplier.x = 0;
                    }else if(grabbedObject.rightLock && rawInputMovement.x < 0)
                    {
                        grabbedObject.rightLock = false;
                    }
                    if (grabbedObject.leftLock && rawInputMovement.x < 0 || rb.velocity.x == 0)
                    {
                        tempDirMultiplier.x = 0;
                    }else if (grabbedObject.leftLock && rawInputMovement.x > 0)
                    {
                        grabbedObject.leftLock = false;
                    }

                    if (grabbedObject.upLock && rawInputMovement.y > 0 || rb.velocity.y == 0)
                    {
                        tempDirMultiplier.y = 0;
                    }
                    else if (grabbedObject.upLock && rawInputMovement.y < 0)
                    {
                        grabbedObject.upLock = false;
                    }
                    if (grabbedObject.downLock && rawInputMovement.y < 0 || rb.velocity.y == 0)
                    {
                        tempDirMultiplier.y = 0;
                    }
                    else if (grabbedObject.downLock && rawInputMovement.y > 0)
                    {
                        grabbedObject.downLock = false;
                    }
                    
                    grabbedObject.rbInteractObject.velocity = rb.velocity * tempDirMultiplier;
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
            charaManager.SpiritGO.GetComponent<ELC_Attack>().StopDashCoroutine();
        }
        else if(CompareTag("Spirit") && collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if((currentCharacter && isDashing) || (!currentCharacter && charaManager.RynGO.GetComponent<ELC_Attack>().spiritAttack))
            {
                Debug.Log("Ceci est un ennemi");
                collision.gameObject.GetComponent<AXD_EnemyHealth>().GetHit(charaManager.stats.StunTime);
            }
            
        }else if (CompareTag("Ryn") && currentCharacter && grabbedObject != null)
        {
            Debug.Log("Kwa ?");
            Vector2 averageContactPoint = Vector2.zero;
            ContactPoint2D[] allContactPoints = new ContactPoint2D[2];
            collision.collider.GetContacts(allContactPoints);
            foreach (ContactPoint2D contactPoint in allContactPoints)
            {
                averageContactPoint += contactPoint.point;
            }
            averageContactPoint /= allContactPoints.Length;
            if (averageContactPoint.x - charaManager.RynGO.transform.position.x > 0) // Si la caisse est à droite de Ryn
            {
                grabbedObject.rightLock = true;
            }
            if (averageContactPoint.x - charaManager.RynGO.transform.position.x <= 0) // Si la caisse est à gauche de Ryn
            {
                grabbedObject.leftLock = true;
            }
            if (averageContactPoint.y - charaManager.RynGO.transform.position.y > 0) // Si la caisse est au dessus de Ryn
            {
                grabbedObject.upLock = true;
            }
            if (averageContactPoint.y - charaManager.RynGO.transform.position.x <= 0) // Si la caisse est en dessous de Ryn
            {
                grabbedObject.downLock = true;
            }

        }

    }
}
