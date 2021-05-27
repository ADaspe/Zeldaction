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
    private BoxCollider2D objectCollider;
    //public LayerMask LayersToDetect;
    public AXD_Activable[] objectsToActivate;
    private Animator animator;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        interactScript = this.GetComponent<ELC_Interact>();
        objectCollider = GetComponent<BoxCollider2D>();
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
                //Debug.Log(gameObject.name + " Activate " + item.gameObject.name);
                item.Activate();
            }
        }
    }

    private void Detection(Collider2D collision, bool isEntering = true)
    {
        //ConditionsEnabled = false;
        if (type == ActivatorType.PRESSUREPLATE && (collision.gameObject.CompareTag("Crate") || collision.gameObject.CompareTag("Ryn")))
        {
            bool itemOnPlate = false;
            Collider2D[] detected = Physics2D.OverlapCircleAll(new Vector2(transform.position.x + objectCollider.offset.x, transform.position.y + objectCollider.offset.y), detectionRadius);
            foreach (Collider2D item in detected)
            {
                if(item.CompareTag("Crate") || item.CompareTag("Ryn"))
                {
                    itemOnPlate = true;
                }
            }
            if (!isActivated)
            {
                isActivated = true;
                AnimatorUpdate();
                foreach (AXD_Activable item in objectsToActivate)
                {
                    item.Activate();
                }
            }
            ConditionsEnabled = (isEntering||itemOnPlate);
            //return;
        }
        else if (type == ActivatorType.TORCH && collision.gameObject.CompareTag("Spirit") && collision.gameObject.GetComponent<AXD_CharacterMove>().isDashing)
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
                ConditionsEnabled = isEntering;
                //return;
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


    private void AnimatorUpdate()
    {
        animator.SetBool("Activated", isActivated);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    { 
        Detection(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        Detection(collision , false);
    }
}
