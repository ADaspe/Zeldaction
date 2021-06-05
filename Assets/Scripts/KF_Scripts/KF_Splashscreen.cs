using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public class KF_Splashscreen : MonoBehaviour
{
    public Evenements events;

    // Update is called once per frame
    void Update()
    {
        if (Input.anyKey)
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
