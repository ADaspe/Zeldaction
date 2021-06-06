using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class FLC_Teleporter : MonoBehaviour
{
    public GameObject destination;
    public AudioManager audioManager;
    public Animator animator;
    public PlayerInput inputsPlayer;
    public GameObject[] thingsToTeleport;
    private bool teleportationEnd;
    [HideInInspector]
    public bool tp;
    [HideInInspector]
    public bool giveBackControl;

    public void Teleport()
    {
        inputsPlayer.enabled = false;
        StartCoroutine(Teleportation());
        animator.SetTrigger("TP");
        giveBackControl = false;
    }

    IEnumerator Teleportation()
    {
        yield return new WaitForSeconds(0.05f);

        if (tp && !teleportationEnd)
        {
            for (int i = 0; i < thingsToTeleport.Length; i++)
            {
                thingsToTeleport[i].transform.SetPositionAndRotation(destination.transform.position, thingsToTeleport[i].transform.rotation);
            }
            teleportationEnd = true;
        }
       
        if(tp && giveBackControl)
        {
            teleportationEnd = false;
            tp = false;
            inputsPlayer.enabled = true;
        }
        else
        {
            Debug.Log("ça passe par là en fait du coup ça complique la solution là");
            StartCoroutine(Teleportation());
        }
    }

    public void SoundDeparture()
    {
        audioManager.Play("TP_Departure");
    }
    public void SoundArrival()
    {
        audioManager.Play("TP_Arrival");
    }

    private void OnDrawGizmos()
    { 
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position, destination.transform.position);
    }
}