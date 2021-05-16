using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ELC_Interact : MonoBehaviour
{
    public List<Transform> PlayerDetectors = new List<Transform>();
    public ELC_GameManager GameManagerScript;
    public bool corrupted;
    public bool PlayerCanInteract;
    public UnityEvent Interact;
    public bool isMobile;
    public bool isGrabbed;
    public bool isTotem;
    public bool isBrambles;
    public Rigidbody2D rbInteractObject;

    private int touchedSideIndex;
    private bool playerIsTouchingSide;

    private void Start()
    {
        touchedSideIndex = -1;
        rbInteractObject = GetComponent<Rigidbody2D>();
    }
    void Update()
    {
        if (!corrupted)
        {
            for (int i = 0; i < PlayerDetectors.Count; i++)
            {
                Vector3 dir = PlayerDetectors[i].position - this.transform.position;
                float raycastLength = Vector2.Distance(this.transform.position, PlayerDetectors[i].position);

                RaycastHit2D playerHit = Physics2D.Raycast(this.transform.position, dir, raycastLength, GameManagerScript.PlayerMask);
                Debug.DrawRay(this.transform.position, dir.normalized * raycastLength, Color.red);

                if (playerHit.collider != null)
                {
                    touchedSideIndex = i;
                    playerIsTouchingSide = true;
                    PlayerIsOnSide(playerHit.transform.gameObject, raycastLength);
                }
                else if (i == touchedSideIndex) //Si ça touche pas et que le dernier coté à être touché est celui-là, alors ça veut dire que le joueur ne touche aucun coté
                {
                    PlayerCanInteract = false;
                }
            }
        }
        else
        {
            PlayerCanInteract = false;
        }
    }

    void PlayerIsOnSide(GameObject player, float rayLength)
    {
        AXD_CharacterMove playerMovesScript = player.GetComponent<AXD_CharacterMove>();
        RaycastHit2D PlayerFaceDetection = Physics2D.Raycast(player.transform.position, playerMovesScript.LastDirection.normalized, rayLength, LayerMask.GetMask(LayerMask.LayerToName(gameObject.layer)));

        Debug.DrawRay(player.transform.position, playerMovesScript.LastDirection.normalized * rayLength, Color.green);

        if (PlayerFaceDetection.collider != null)
        {
            PlayerCanInteract = true;
            GameManagerScript.CharacterManager.DetectedInteraction = this;
        }
        else PlayerCanInteract = false;
    }

    public void Purify()
    {
        if (corrupted)
        {
            if (isTotem) this.GetComponent<ELC_Totem>().Purify();
            if (isBrambles) this.GetComponent<AXD_Brambles>().Purify();
            corrupted = !corrupted;
        }
    }
}
