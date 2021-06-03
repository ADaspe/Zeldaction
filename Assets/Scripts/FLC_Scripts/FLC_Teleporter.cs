using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FLC_Teleporter : MonoBehaviour
{
    public GameObject destination;
    public AudioManager audioManager;
    public Animator animator;
    private bool teleportationEnd;
    public bool tp;
    public bool giveBackControl;

    public void Teleport(GameObject[] thingsToTeleport)
    {
        //Retirer le contrôle au joueur
        StartCoroutine(Teleportation(thingsToTeleport));        
        animator.SetTrigger("TP");
        giveBackControl = false;
    }

    IEnumerator Teleportation(GameObject[] thingsToTeleport)
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
       
        if(!tp || !giveBackControl)
        {
            StartCoroutine(Teleportation(thingsToTeleport));
        }
        else
        {
            teleportationEnd = false;
            tp = false;
            //Rendre le contrôle au joueur
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