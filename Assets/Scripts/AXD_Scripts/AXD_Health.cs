using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AXD_Health : MonoBehaviour
{
    public ELC_CharacterManager CharacterManager;
    public AXD_UIHPDisplay hpDisplay;
    public bool Invincible;
    public float InvincibilityTime;
    public void GetHit()
    {
        if (!Invincible)
        {
            Invincible = true;
            hpDisplay.LoseLife();
            CharacterManager.TakeDamage(this.tag);
            Invoke("Vulnerable", InvincibilityTime);
        }
    }

    void Vulnerable()
    {
        Invincible = false;
    }
}
