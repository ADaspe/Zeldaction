using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


public class KF_EnvironmentalDialogue : MonoBehaviour
{
    public Evenements events;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ryn"))
        {
            if (events != null)
            {
                events.evenement.Invoke();
            }
        }
    }

    [System.Serializable]
    public class Evenements
    {
        public UnityEvent evenement;
    }
}
