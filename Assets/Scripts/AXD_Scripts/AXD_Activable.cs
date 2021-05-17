using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AXD_Activable : MonoBehaviour
{
    
    public List<ELC_Activation> ActivationsNeeded;
    public abstract void Activate();
}
