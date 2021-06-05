using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class AXD_Health : MonoBehaviour
{
    public ELC_CharacterManager CharacterManager;
    public AXD_UIHPDisplay hpDisplay;
    public bool Invincible;
    public float InvincibilityTime;

    [Button]
    public void GetHit()
    {
        if (this.CompareTag("Ryn"))
        {
            if (!Invincible && !CharacterManager.RynAttack.ShieldOn)
            {
                Invincible = true;
                if (CharacterManager.TakeDamage(this.tag))
                {
                    hpDisplay.LoseLife();
                }
                Invoke("Vulnerable", InvincibilityTime);
            }else if (CharacterManager.RynAttack.ShieldOn)
            {
                CharacterManager.gameManager.audioManager.Play("Shield_Impact");
            }
        }else if (this.CompareTag("Spirit"))
        {
            //Appliquer feedback
            CharacterManager.gameManager.audioManager.Play("Spirit_Disappear");
            GetComponent<ELC_SpiritIdle>().Teleport();

        }
    }

    void Vulnerable()
    {
        Invincible = false;
    }
}
