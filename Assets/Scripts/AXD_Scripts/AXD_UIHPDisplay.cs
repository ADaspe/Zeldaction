using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Sirenix.OdinInspector;

public class AXD_UIHPDisplay : MonoBehaviour
{
    public ELC_CharacterManager charaManager;
    public GameObject HPPrefab;
    public List<AXD_HP> allHPs;

    private void Start()
    {
        for (int i = 0; i < charaManager.currentHP; i++)
        {
            allHPs.Add(Instantiate(HPPrefab, this.transform).GetComponent<AXD_HP>());

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

    [Button]
    public void AddLife()
    {
        allHPs.Add(Instantiate(HPPrefab, this.transform).GetComponent<AXD_HP>());
    }

}
