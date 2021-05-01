using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AXD_Mushroom : MonoBehaviour
{
    public float ExplodingTime;
    public float ProjectionDistance;
    public float ExplodingRadius;
    public float ProjectionTime;
    public bool projected;
    public Rigidbody2D rb;
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void Projection(Vector2 direction)
    {
        projected = true;
        rb.velocity = direction.normalized * (ProjectionDistance / ProjectionTime);
        Debug.Log("Velocity : " + rb.velocity);
        Invoke("StopChampi", ProjectionTime);
    }

    public void StopChampi()
    {
        rb.velocity = Vector2.zero;
        projected = false;
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Prout");
        AXD_CharacterMove tempCharaMove = collision.gameObject.GetComponent<AXD_CharacterMove>();
        if (tempCharaMove.wasDashingWhenColliding)
        {
            Vector2 direction = tempCharaMove.LastDirection;
            Projection(direction);
            tempCharaMove.wasDashingWhenColliding = false; ;
        }
    }

}
