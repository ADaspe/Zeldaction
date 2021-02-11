using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class AXD_Character : MonoBehaviour
{
    private int lifePoint;
    private float stunTime;
    private bool isStun;
    private float speed;

    public int LifePoint { get => lifePoint; set => lifePoint = value; }
    public float StunTime { get => stunTime; set => stunTime = value; }
    public bool IsStun { get => isStun; set => isStun = value; }
    public float Speed { get => speed; set => speed = value; }

    public abstract void GetHit();
    public abstract void Move();
    public abstract void Die();
    public abstract void Attack();
}
