using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELC_Door : AXD_Activable
{
    public bool open;
    public bool ActivateOnDisable;
    
    private Collider2D rb;
    private int currentNumberOfActivation;

    private void Start()
    {
        rb = this.GetComponent<Collider2D>();
        CheckActivations();
    }

    public void CheckActivations()
    {
        if (ActivationsNeeded.Count != 0)
        {
            currentNumberOfActivation = 0;

            foreach (ELC_Activation active in ActivationsNeeded)
            {
                if ((!ActivateOnDisable && active.isActivated) || (ActivateOnDisable && !active.isActivated)) currentNumberOfActivation++;
            }
            if (currentNumberOfActivation == ActivationsNeeded.Count)
            {
                if (!open)
                {
                    open = true;
                    rb.enabled = false;
                }
                return;
            }
            else
            {
                open = false;
                rb.enabled = true;
            }
        }
    }

    public override void Activate()
    {
        
        CheckActivations();
    }
}
