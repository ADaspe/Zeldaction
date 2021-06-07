using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELC_SpiritIdle : MonoBehaviour
{
    [SerializeField]
    ELC_CharacterManager CharaManager;
    private bool PlayerIsImmobile;
    private float LastPlayerMove;
    public float MaxSpeed;
    public float NearSpeedMultiplicator;
    public float TimeToWaitForIdleState;
    public float DistanceToStayWhenTogether;
    public bool closeToRyn;
    public float minDistToTeleport;
    public bool disabled;
    public bool stuck;
    [SerializeField]
    private Vector2 debug;
    private float RynAngle;
    public Vector2 targetPos;
    
    void FixedUpdate()
    {
        if (!disabled)
        {
            if (CharaManager.RynMove.rawInputMovement.magnitude > 0.01f) LastPlayerMove = Time.deltaTime;

            if (Time.deltaTime - LastPlayerMove > TimeToWaitForIdleState) PlayerIsImmobile = true;
            else PlayerIsImmobile = false;

            if (!PlayerIsImmobile) RynAngle = Mathf.Atan2(CharaManager.RynMove.LastDirection.y, CharaManager.RynMove.LastDirection.x) * Mathf.Rad2Deg;

            targetPos = new Vector2(-(DistanceToStayWhenTogether * Mathf.Cos(RynAngle)) + CharaManager.RynGO.transform.position.x, -(DistanceToStayWhenTogether * Mathf.Sin(RynAngle)) + CharaManager.RynGO.transform.position.y); //Calculer une position en fonction de la longueur qu'on lui donne et d'un angle
            Debug.DrawRay(CharaManager.RynGO.transform.position, new Vector3(targetPos.x - CharaManager.RynGO.transform.position.x, targetPos.y - CharaManager.RynGO.transform.position.y).normalized, Color.red);

            if (CharaManager.Together)
            {
                Vector2 dir = new Vector2(targetPos.x - this.transform.position.x, targetPos.y - this.transform.position.y); //La direction pour rejoindre le point d'idle de l'esprit
                if (Vector2.Distance(new Vector2(targetPos.x, targetPos.y), this.transform.position) < MaxSpeed)
                {
                    closeToRyn = true;
                    CharaManager.spiritMove.isDashing = false;
                    CharaManager.SpiritGO.layer = LayerMask.NameToLayer(CharaManager.SpiritAttack.defaultMask);
                    CharaManager.spiritMove.rb.velocity = dir * NearSpeedMultiplicator; //Si l'esprit commence � �tre proche du joueur on ralentit
                }
                else
                {
                    CharaManager.spiritMove.rb.velocity = dir.normalized * MaxSpeed; //Sinon on le laisse � vitesse constante
                }
            }
        }

        if (!stuck)
        {
            float tempDist = Vector2.Distance(CharaManager.RynGO.transform.position,CharaManager.SpiritGO.transform.position);
            if (tempDist >= minDistToTeleport)
            {
                closeToRyn = false;
            }
        }
    }

    public void Teleport()
    {
        CharaManager.IdenAnimator.SetBool("Ball", true);
        CharaManager.IdenAnimator.SetBool("Idle", true);
        CharaManager.IdenAnimator.SetBool("Dash", false);
        transform.position = targetPos;
        closeToRyn = true;
        CharaManager.spiritMove.isDashing = false;
        CharaManager.SpiritGO.layer = LayerMask.NameToLayer(CharaManager.SpiritAttack.defaultMask);
    }
    public void Teleport(Vector2 targetLocation)
    {
        CharaManager.IdenAnimator.SetBool("Ball", true);
        CharaManager.IdenAnimator.SetBool("Idle", true);
        CharaManager.IdenAnimator.SetBool("Dash", false);
        transform.position = targetLocation;
        closeToRyn = true;
        CharaManager.spiritMove.isDashing = false;
        CharaManager.SpiritGO.layer = LayerMask.NameToLayer(CharaManager.SpiritAttack.defaultMask);
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        if (CharaManager.followingCharacter == CharaManager.RynMove)
        {
            if (collision.gameObject.layer != LayerMask.NameToLayer("ObstacleSpirit") && !collision.gameObject.CompareTag("Ryn") && !closeToRyn && !CharaManager.spiritProjected)
            {
                Teleport(targetPos);
            }else if(CharaManager.spiritProjected == true)
            {
                CharaManager.ResetProjection();
            }
            else if (collision.gameObject.layer == LayerMask.NameToLayer("ObstacleSpirit"))
            {
                stuck = true;
            }
        }
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        if(collision.gameObject.layer == LayerMask.NameToLayer("ObstacleSpirit"))
        {
            stuck = false;
        }
    }

}
