using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELC_Interact : MonoBehaviour
{
    [SerializeField]
    private Transform PlayerDetectorLength;
    [SerializeField]
    private ELC_GameManager GameManagerScript;
    public bool PlayerCanInteract;

    
    void Update()
    {
        Vector3 dir = PlayerDetectorLength.position - this.transform.position;
        float raycastLength = Vector2.Distance(this.transform.position, PlayerDetectorLength.position);

        RaycastHit2D playerHit = Physics2D.Raycast(this.transform.position, dir, raycastLength, GameManagerScript.PlayerMask);
        Debug.DrawRay(this.transform.position, dir.normalized * raycastLength, Color.red);

        if (playerHit.collider != null)
        {
            AXD_CharacterMove playerMovesScript = playerHit.collider.gameObject.GetComponent<AXD_CharacterMove>();
            RaycastHit2D PlayerFaceDetection = Physics2D.Raycast(playerHit.transform.position, playerMovesScript.LastDirection.normalized, raycastLength, LayerMask.GetMask(this.gameObject.layer.ToString()));
            
            Debug.DrawRay(playerHit.transform.position, playerMovesScript.LastDirection.normalized * raycastLength, Color.green);

            if(PlayerFaceDetection.collider != null)
            {
                PlayerCanInteract = true;
            }
            else PlayerCanInteract = false;
        }
        else PlayerCanInteract = false;
    }

    public void Use()
    {
        Debug.Log("Interaction avec l'objet " + this.name);
    }
}
