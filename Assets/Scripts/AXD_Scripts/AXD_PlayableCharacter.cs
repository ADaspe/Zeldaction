using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AXD_PlayableCharacter : AXD_Character
{
    private bool together;

    public bool Together { get => together; set => together = value; }

    public void SpiritReturn() {
        //To define
        Debug.Log("Yet to define");
    }

    public void SwapCam()
    {
        //To define
        Debug.Log("Yet to define");
    }

    public void Interact()
    {
        //To define
        Debug.Log("Yet to define");
    }

    public override void GetHit()
    {
        throw new System.NotImplementedException();
    }

    public override void Move()
    {
        throw new System.NotImplementedException();
    }

    public override void Die()
    {
        throw new System.NotImplementedException();
    }

    public override void Attack()
    {
        throw new System.NotImplementedException();
    }
}
