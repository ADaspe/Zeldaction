using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AXD_ThinWall : MonoBehaviour
{
    public void CollapseWall()
    {
        Debug.Log("Wall destruction");
        //TODO : animations management
        Destroy(this.gameObject);
    }
}
