using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELC_Door : AXD_Activable
{
    public bool ActivateOnDisable;
    
    private Collider2D rb;
    private int currentNumberOfActivation;

    private void Start()
    {
        if (gameObject.layer != LayerMask.NameToLayer("ObstacleSpirit")) // sert à ne pas faire buguer sur le pollen qui n'a pas d'animator
        {
            ObjectAnimator = GetComponent<Animator>();
        }
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
                if (!isActivated)
                {
                    isActivated = true;
                    LockTorches();
                    rb.enabled = false;
                    if (ObjectAnimator != null) // Pas de null pointer exception :)
                    {
                        ObjectAnimator.SetBool("Activated", isActivated);
                    }
                }
                return;
            }
            else
            {
                isActivated = false;
                rb.enabled = true;
                if (ObjectAnimator != null) // Pas de null pointer exception :)
                {
                    ObjectAnimator.SetBool("Activated", isActivated);
                }
            }
        }
        
    }

    public override void Activate()
    {
        
        CheckActivations();
    }
}
