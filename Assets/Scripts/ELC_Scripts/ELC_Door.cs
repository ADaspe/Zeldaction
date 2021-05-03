using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELC_Door : AXD_Activable
{
    public bool open;
    [HideInInspector]
    
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

    public override void Activate()
    {
        CheckActivations();
    }
}
