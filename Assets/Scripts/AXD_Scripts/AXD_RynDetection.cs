using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AXD_RynDetection : MonoBehaviour
{
    [HideInInspector]
    public ELC_SpiritIdle spiritIdle;

    private void Start()
    {
        spiritIdle = GetComponentInParent<ELC_SpiritIdle>();
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        spiritIdle.closeToRyn = true;
        if (collision.CompareTag("Ryn") )
        {
            ELC_CharacterManager tempManager = this.GetComponentInParent<AXD_CharacterMove>().charaManager;
            if(tempManager.followingCharacter == tempManager.RynMove && tempManager.RynMove.canMove && !tempManager.Together)
            {
                //Vérifier qu'il y a pas d'obstacle entre les deux personnages
                /*Debug.Log("Layer : "+ LayerMask.LayerToName(Physics2D.Raycast(spiritIdle.transform.position, collision.transform.position, 100f, LayerMask.GetMask("PlayerRyn","Obstacle", "ObstacleSpirit", "ThinWall")).collider.gameObject.layer));
                Debug.DrawRay(spiritIdle.transform.position, collision.transform.position*100f,Color.cyan);
                if (Physics2D.Raycast(transform.position, collision.transform.position, 100f, LayerMask.GetMask("Obstacle", "ObstacleSpirit", "PlayerRyn", "ThinWall")).collider.gameObject.layer == LayerMask.NameToLayer("PlayerRyn"))
                {*/
                    tempManager.RegroupTogether();
                //}
                
            }
        }
    }


}
