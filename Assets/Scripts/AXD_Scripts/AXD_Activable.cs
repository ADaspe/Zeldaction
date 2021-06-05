using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AXD_Activable : MonoBehaviour
{
    
    public List<ELC_Activation> ActivationsNeeded;
    public bool isActivated;
    public Animator ObjectAnimator;
    public abstract void Activate();
    public void LockTorches()
    {
        foreach (ELC_Activation item in ActivationsNeeded)
        {
            if(item.type == ELC_Activation.ActivatorType.TORCH)
            {
                item.isActivated = true;
                item.definitivelyActivated = true;
                item.StopCoroutine("Countdown");
            }
        }
    }
    
}
