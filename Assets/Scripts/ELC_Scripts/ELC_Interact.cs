using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ELC_Interact : MonoBehaviour
{
    public GameObject PurifyParticles;
    public GameObject CorruptedBrambles;
    [HideInInspector]
    public bool leftLock, rightLock, upLock, downLock;
    public List<Transform> PlayerDetectors = new List<Transform>();
    public ELC_GameManager GameManagerScript;
    public bool corrupted;
    public bool PlayerCanInteract;
    public UnityEvent Interact;
    [HideInInspector]
    public bool isMobile;
    [HideInInspector]
    public bool isGrabbed;
    [HideInInspector]
    public bool isTotem;
    [HideInInspector]
    public bool isBrambles;
    public Rigidbody2D rbInteractObject;
    
    private int touchedSideIndex;
    private GameObject InstantiatedBrambles;
    //private bool playerIsTouchingSide;

    private void Start()
    {
        touchedSideIndex = -1;
        rbInteractObject = GetComponent<Rigidbody2D>();
        if (corrupted && CorruptedBrambles != null)
        {
            InstantiatedBrambles = Instantiate(CorruptedBrambles, this.transform);
            
        }
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
                    //playerIsTouchingSide = true;
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
            if (isBrambles) this.GetComponent<AXD_Brambles>().Purify();
            Instantiate(PurifyParticles, this.transform).GetComponent<ParticleSystem>().Play();
            Destroy(InstantiatedBrambles);
            corrupted = !corrupted;
        }
    }

    public void ResetLocks()
    {
        rightLock = leftLock = upLock = downLock = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        if((collision.gameObject.layer == LayerMask.NameToLayer("PlayerRyn") || collision.gameObject.layer == LayerMask.NameToLayer("Obstacle")) && isGrabbed )
        {
            Vector2 averageContactPoint = Vector2.zero;
            ContactPoint2D[] allContactPoints = new ContactPoint2D[2];
            collision.collider.GetContacts(allContactPoints);
            foreach (ContactPoint2D contactPoint in allContactPoints)
            {
                averageContactPoint += contactPoint.point;
            }
            averageContactPoint /= allContactPoints.Length;
            if (averageContactPoint.x - GameManagerScript.CharacterManager.RynGO.transform.position.x > 0) // Si la caisse est à droite de Ryn
            {
                leftLock = true;
            }
            if(averageContactPoint.x - GameManagerScript.CharacterManager.RynGO.transform.position.x <= 0) // Si la caisse est à gauche de Ryn
            {
                rightLock = true;
            }
            if(averageContactPoint.y - GameManagerScript.CharacterManager.RynGO.transform.position.y > 0) // Si la caisse est au dessus de Ryn
            {
                downLock = true;
            }
            if(averageContactPoint.y - GameManagerScript.CharacterManager.RynGO.transform.position.x <=0) // Si la caisse est en dessous de Ryn
            {
                upLock = true;
            }
        }
    }
}
