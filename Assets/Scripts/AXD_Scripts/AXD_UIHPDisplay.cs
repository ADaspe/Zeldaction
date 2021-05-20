using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AXD_UIHPDisplay : MonoBehaviour
{
    public ELC_CharacterManager charaManager;
    public AXD_HP HPPrefab;
    public List<AXD_HP> allHPs;

    private void Start()
    {
        for (int i = 0; i < charaManager.currentHP; i++)
        {
            allHPs.Add(Instantiate(HPPrefab));
        }
    }
    public void LoseLife()
    {

    }

    public void HealLife()
    {

    }

    public void HealFullLife()
    {
        
    }

    public void AddLife()
    {

    }

}
