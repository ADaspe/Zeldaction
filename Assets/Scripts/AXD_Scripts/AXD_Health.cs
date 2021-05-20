using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AXD_Health : MonoBehaviour
{
    public ELC_CharacterManager CharacterManager;
    public void GetHit()
    {
        CharacterManager.TakeDamage(this.tag);
        Debug.Log("oh le hit");
    }
}
