using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ELC_Activation : MonoBehaviour
{
    public enum ActivatorType {TORCH, LEVER, PRESSUREPLATE};
    public ActivatorType type;
    public bool isActivated;
    public float TorchDuration;
    [SerializeField]
    private bool ConditionsEnabled;
    private ELC_Interact interactScript;
    public float detectionRadius;
    public LayerMask LayersToDetect;
    public AXD_Activable[] objectsToActivate;
    private Animator animator;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        interactScript = this.GetComponent<ELC_Interact>();
        foreach (AXD_Activable item in objectsToActivate)
        {        
            item.ActivationsNeeded.Add(this);
        }
    }
    public void ActivateObject()
    {
        if (!interactScript.corrupted && (type == ActivatorType.PRESSUREPLATE || type == ActivatorType.LEVER))
        {
            isActivated = !isActivated;
            AnimatorUpdate();
            foreach (AXD_Activable item in objectsToActivate)
            {
                Debug.Log(gameObject.name + " Activate " + item.gameObject.name);
                item.Activate();
            }
        }
    }

    private void Detection()
    {
        Collider2D[] detected = Physics2D.OverlapCircleAll(this.transform.position, detectionRadius);
        
        ConditionsEnabled = false;

        foreach (Collider2D col in detected)
        {
            Debug.Log(this.gameObject.name +" a détecté : " + col.name);
            if (type == ActivatorType.PRESSUREPLATE && (col.gameObject.CompareTag("Crate") || col.gameObject.CompareTag("Ryn")))
            {
                if (!isActivated)
                {
                    isActivated = true;
                    AnimatorUpdate();
                    foreach (AXD_Activable item in objectsToActivate)
                    {
                        item.Activate();
                    }
                }
                ConditionsEnabled = true;
                //return;
            }
            else if (type == ActivatorType.TORCH && col.gameObject.CompareTag("Spirit") && col.gameObject.GetComponent<AXD_CharacterMove>().isDashing)
            {
                if (!isActivated)
                {
                    StopCoroutine("Countdown");
                    StartCoroutine("Countdown");
                    isActivated = true;
                    AnimatorUpdate();
                    foreach (AXD_Activable item in objectsToActivate)
                    {
                        item.Activate();
                    }
                    ConditionsEnabled = true;
                    //return;
                }
            }
        }

        if (ConditionsEnabled == false && isActivated)
        {
            isActivated = false;
            AnimatorUpdate();
            foreach (AXD_Activable item in objectsToActivate)
            {
                item.Activate();
            }
        }
        
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(TorchDuration);
        isActivated = false;
        AnimatorUpdate();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log(gameObject.name + " entre en collision avec "+collision.gameObject.name);
        Detection();
    }

    private void AnimatorUpdate()
    {
        animator.SetBool("Activated", isActivated);
    }
}
