using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AXD_Mushroom : MonoBehaviour
{
    public ELC_Interact interact;
    public float ExplodingTime;
    public float ProjectionDistance;
    public float ExplodingRadius;
    public float ProjectionTime;
    public bool projected;
    public Rigidbody2D rb;
    //public float timeDecreaseSpeed;
    private void Start()
    {
        interact = GetComponent<ELC_Interact>();
        rb = GetComponent<Rigidbody2D>();
        //timeDecreaseSpeed = ProjectionTime;
    }

    /*private void FixedUpdate()
    {
        if (projected)
        {
            rb.velocity = rb.velocity.normalized * (ProjectionDistance / ProjectionTime);
        }
    }*/
    public void Projection(Vector2 direction)
    {
        Activate();
        projected = true;
        rb.velocity = direction.normalized * (ProjectionDistance / ProjectionTime);
        //StartCoroutine(IncreaseTime());
        Debug.Log("Velocity : " + rb.velocity);
        Invoke("StopChampi", ProjectionTime);
    }

    public void StopChampi()
    {
        rb.velocity = Vector2.zero;
        projected = false;
    }

    public void Activate()
    {
        StartCoroutine(TheFinaleCountDown());
    }

    public void Explode()
    {
        Collider2D[] allObjectsDetected = Physics2D.OverlapCircleAll(transform.position, ExplodingRadius, interact.GameManagerScript.ExplodingMushroomMask);
        foreach (Collider2D item in allObjectsDetected)
        {
            if (item.CompareTag("Torch"))
            {
                // TODO
            }else if(item.CompareTag("Enemy"))
            {
                item.GetComponent<AXD_EnemyHealth>().GetHit(interact.GameManagerScript.CharacterManager.stats.StunTime);
            }else if(item.gameObject.layer == LayerMask.NameToLayer("ThinWall"))
            {
                item.GetComponent<AXD_ThinWall>().CollapseWall();
            }

        }
        Debug.Log("Prout kaboum");
    }

    /*IEnumerator IncreaseTime()
    {
        while (projected)
        {
            yield return new WaitForSeconds(0.01f);
            timeDecreaseSpeed -= Time.deltaTime*ProjectionTime;
        }
        timeDecreaseSpeed = ProjectionTime;
    }*/

    IEnumerator TheFinaleCountDown()
    {
        yield return new WaitForSeconds(ExplodingTime);
        Explode();
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
