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
    private bool dragSoundEnabled;
    Animator animsIden;
    SpriteRenderer spriteRend;
    private bool SFXIdenEnabled;
    public ParticleSystem IdenTrailPS;
    public bool DashDontMove;

    private void Start()
    {
        spriteRend = this.GetComponent<SpriteRenderer>();
        if(this.gameObject.CompareTag("Spirit")) animsIden = this.GetComponent<Animator>();
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

            if (rawInputMovement.magnitude >= 0.05f) rb.velocity = rawInputMovement * currentSpeed;
            else rb.velocity = Vector2.zero;

            if (rawInputMovement.magnitude >= 0.05f)
            {
                LastDirection = rawInputMovement.normalized; // Sauvegarder la derni�re direction dans laquelle le joueur est tourn�;
                
            }
            if(grabbedObject == null || isRynGrabbing)
            {
                charaManager.gameManager.audioManager.Stop("Box_Drag");
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
                    if(tempDirMultiplier != Vector2.zero && !grabbedObject.gameObject.CompareTag("Mushroom"))
                    {
                        if (!dragSoundEnabled)
                        {
                            dragSoundEnabled = true;
                            charaManager.gameManager.audioManager.Play("Box_Drag");
                        }
                    }
                    else
                    {
                        if (dragSoundEnabled && !grabbedObject.gameObject.CompareTag("Mushroom"))
                        {
                            dragSoundEnabled = false;
                            charaManager.gameManager.audioManager.Stop("Box_Drag");
                        }
                    }
                    grabbedObject.rbInteractObject.velocity = rb.velocity * tempDirMultiplier;
                }
                else
                {
                    isRynGrabbing = false;
                    charaManager.xLocked = false;
                    charaManager.yLocked = false;
                    currentSpeed = charaManager.stats.RynSpeed;
                    dragSoundEnabled = false;

                }
            }
            if (this.gameObject.CompareTag("Ryn"))
            {
                RynAnimatorUpdate();
            }
            if(this.gameObject.CompareTag("Spirit"))
            {
                IdenAnimationsUpdates();
            }
        }else if (!canMove)
        {
            rb.velocity = Vector2.zero;
            if(this.gameObject.CompareTag("Ryn")) RynAnimatorUpdate();
        }

        if(this.gameObject.CompareTag("Spirit"))
        {
            if (charaManager.Together) IdenTrailPS.Stop();
            else IdenTrailPS.Play();

            if (isDashing)
            {
                RaycastHit2D hit = Physics2D.Raycast(this.transform.position, LastDirection, 0.4f, LayerMask.GetMask("Obstacle", "ObstacleSpirit"));
                if (hit.collider != null)
                {
                    Debug.Log("Boum");
                    DashDontMove = true;
                    charaManager.spiritMove.rb.velocity = Vector2.zero;
                }
                animsIden.SetBool("Dash", true);
            }
            else
            {
                animsIden.SetBool("Dash", false);
            }

            if (charaManager.Together)
            {
                animsIden.SetBool("Ball", true);
            }
            else animsIden.SetBool("Ball", false);
        }
    }

    private void IdenAnimationsUpdates()
    {
        animsIden.SetFloat("MovesX", LastDirection.x);
        animsIden.SetFloat("MovesY", LastDirection.y);
        if (LastDirection.x > 0)
        {
            spriteRend.flipX = true;
        }
        else spriteRend.flipX = false;
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

    public void PlayStepFX()
    {
        charaManager.gameManager.audioManager.Play("Ryn_Move");
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(CompareTag("Spirit") && currentCharacter && isDashing && (collision.gameObject.layer == LayerMask.NameToLayer("Obstacle") || collision.gameObject.layer == LayerMask.NameToLayer("ObstacleSpirit")))
        {
            charaManager.SpiritGO.GetComponent<ELC_Attack>().StopDashCoroutine();
        }
        else if(CompareTag("Spirit") && collision.gameObject.layer == LayerMask.NameToLayer("Enemy"))
        {
            if((currentCharacter && isDashing) || (!currentCharacter && charaManager.RynGO.GetComponent<ELC_Attack>().spiritAttack))
            {
                if (collision.gameObject.GetComponent<AXD_EnemyHealth>().GetHit(charaManager.stats.StunTime))
                {
                    charaManager.gameManager.audioManager.Play("Spirit_Paralyze");
                }
            }
            
        }else if (CompareTag("Ryn") && currentCharacter && grabbedObject != null)
        {
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
