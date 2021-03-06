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
    public SpriteRenderer sr;
    public string defaultMask;
    public string projectedMask;
    public GameObject BoomChampi;
    //public float timeDecreaseSpeed;
    private void Start()
    {
        interact = GetComponent<ELC_Interact>();
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
        //timeDecreaseSpeed = ProjectionTime;
    }

    /*private void FixedUpdate()
    {
        if (projected)
        {
            rb.velocity = rb.velocity.normalized * (ProjectionDistance / ProjectionTime);
        }
    }*/
    public void Projection(Vector2 direction, float speed, bool dashUpgrade = false)
    {
        Activate();
        projected = true;
        if (dashUpgrade)
        {
            gameObject.layer = LayerMask.NameToLayer(projectedMask);
        }
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;
        rb.velocity = direction.normalized * speed;
        Invoke("StopChampi", ProjectionTime);
    }

    public void StopChampi()
    {
        rb.velocity = Vector2.zero;
        gameObject.layer = LayerMask.NameToLayer(defaultMask);
        projected = false;
        ResetRigiDoby();
        if (gameObject.layer != LayerMask.NameToLayer(defaultMask))
        {
            gameObject.layer = LayerMask.NameToLayer(defaultMask);
        }
    }

    public void Activate()
    {
        StartCoroutine(TheFinaleCountDown());
    }

    public void Explode()
    {
        BoomChampi.GetComponent<ParticleSystem>().Play();
        this.gameObject.GetComponent<SpriteRenderer>().enabled = false;
        Collider2D[] allObjectsDetected = Physics2D.OverlapCircleAll(transform.position, ExplodingRadius, interact.GameManagerScript.ExplodingMushroomMask);
        if(allObjectsDetected != null)
        {
            foreach (Collider2D item in allObjectsDetected)
            {
                Debug.Log("Tag : " + item.tag);
                if (item.CompareTag("Torch"))
                {
                    ELC_Activation interactible = item.GetComponent<ELC_Activation>();
                    if (interactible.type == ELC_Activation.ActivatorType.TORCH && !interactible.isActivated)
                    {
                        interactible.ActivateObject();
                    }
                }
                else if (item.CompareTag("Enemy"))
                {
                    item.GetComponent<AXD_EnemyHealth>().GetHit(interact.GameManagerScript.CharacterManager.stats.StunTime);
                }
                else if (item.CompareTag("ThinWall"))
                {
                    item.GetComponent<AXD_ThinWall>().CollapseWall();
                }
                else if(item.CompareTag("Boss"))
                {
                    item.GetComponent<ELC_BossHealth>().BossGetHit();
                }
            }
        }
        sr.enabled = false;
        Invoke("DestroyGO",0.5f);
    }

    private void DestroyGO()
    {
        Destroy(this.gameObject);
    }

    IEnumerator TheFinaleCountDown()
    {
        yield return new WaitForSeconds(ExplodingTime);
        Explode();
    }
    public void ResetRigiDoby()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
    }
    public void MakeItMove()
    {
        rb.bodyType = RigidbodyType2D.Dynamic;
        rb.constraints = RigidbodyConstraints2D.None;
        rb.constraints = RigidbodyConstraints2D.FreezeRotation;

    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        
        AXD_CharacterMove tempCharaMove = collision.gameObject.GetComponent<AXD_CharacterMove>();
        if (tempCharaMove != null && tempCharaMove.wasDashingWhenColliding)
        {
            Vector2 direction = tempCharaMove.LastDirection;
            Projection(direction, (tempCharaMove.charaManager.stats.DashDistance/ tempCharaMove.charaManager.stats.DashTime), tempCharaMove.charaManager.dashPlusUpgrade);
            tempCharaMove.wasDashingWhenColliding = false; ;
        }
        else if (collision.gameObject.CompareTag("ThinWall"))
        {
            ResetRigiDoby();            
        }
    }
}
