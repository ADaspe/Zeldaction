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
    [SerializeField]
    private Vector2 debug;
    private float MiaAngle;
    private Vector2 targetPos;
    

    
    void Update()
    {
        if (CharaManager.RynMove.rawInputMovement.magnitude > 0.01f) LastPlayerMove = Time.deltaTime;

        if (Time.deltaTime - LastPlayerMove > TimeToWaitForIdleState) PlayerIsImmobile = true;
        else PlayerIsImmobile = false;
        
        if(!PlayerIsImmobile) MiaAngle = Mathf.Atan2(CharaManager.RynMove.LastDirection.y, CharaManager.RynMove.LastDirection.x) * Mathf.Rad2Deg;

        targetPos = new Vector2( -(DistanceToStayWhenTogether * Mathf.Cos(MiaAngle)) + CharaManager.RynGO.transform.position.x, -(DistanceToStayWhenTogether * Mathf.Sin(MiaAngle)) + CharaManager.RynGO.transform.position.y); //Calculer une position en fonction de la longueur qu'on lui donne et d'un angle
        Debug.DrawRay(CharaManager.RynGO.transform.position, new Vector3(targetPos.x - CharaManager.RynGO.transform.position.x, targetPos.y - CharaManager.RynGO.transform.position.y ).normalized, Color.red);

        if (CharaManager.Together)
        {
            Vector2 dir = new Vector2(targetPos.x - this.transform.position.x, targetPos.y - this.transform.position.y); //La direction pour rejoindre le point d'idle de l'esprit
            if (Vector2.Distance(new Vector2(targetPos.x, targetPos.y), this.transform.position) < MaxSpeed)
            {
                CharaManager.spiritMove.rb.velocity = dir * NearSpeedMultiplicator; //Si l'esprit commence à être proche du joueur on ralentit
            }
            else
            {
                CharaManager.spiritMove.rb.velocity = dir.normalized * MaxSpeed; //Sinon on le laisse à vitesse constante
            }
        }
    }
}
