using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AXD_Health : MonoBehaviour
{
    public ELC_CharacterManager CharacterManager;
    public AXD_UIHPDisplay hpDisplay;
    public void GetHit()
    {
        hpDisplay.LoseLife();
        CharacterManager.TakeDamage(this.tag);
    }
}
