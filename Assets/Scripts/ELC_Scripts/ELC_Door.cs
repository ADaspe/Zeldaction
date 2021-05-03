using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELC_Door : MonoBehaviour
{
    public bool open;
    [HideInInspector]
    public List<ELC_Activation> ActivationsNeeded;
    private Collider2D rb;
    private int currentNumberOfActivation;

    private void Start()
    {
        rb = this.GetComponent<Collider2D>();
    }

    public void CheckActivations()
    {
        currentNumberOfActivation = 0;

        foreach (ELC_Activation active in ActivationsNeeded)
        {
            if(active.isActivated) currentNumberOfActivation++;
        }

        if (currentNumberOfActivation == ActivationsNeeded.Count)
        {
            open = true;
            rb.enabled = false;
            return;
        }
        else
        {
            open = false;
            rb.enabled = true;
        }
    }
}
