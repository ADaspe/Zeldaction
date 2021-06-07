using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class KF_EndBoss : MonoBehaviour
{
    public ELC_BossManager bM;
    public Evenements events;
    private int count;

    private void FixedUpdate()
    {
        if ((bM.isStunned == true) && (count == 0))
        {
            if (events != null)
            {
                events.evenement.Invoke();
                count++;
            }
        }
    }


    [System.Serializable]
    public class Evenements
    {
        public UnityEvent evenement;
    }

}
