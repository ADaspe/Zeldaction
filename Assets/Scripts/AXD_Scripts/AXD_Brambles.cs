using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AXD_Brambles : MonoBehaviour
{
    public Animator anims;
    public void Purify()
    {
        anims.SetBool("Dissolve", true);
        Invoke("Destroy", 1f);
    }

    private void Destroy()
    {
        Destroy(this.gameObject);
    }
}
