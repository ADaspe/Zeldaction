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
    [HideInInspector]
    public bool isCorrupted;
    [SerializeField]
    private bool ConditionsEnabled;
    private ELC_Interact interactScript;
    public float detectionRadius;
    public AXD_Activable[] objectsToActivate;

    private void Start()
    {
        interactScript = this.GetComponent<ELC_Interact>();
        foreach (AXD_Activable item in objectsToActivate)
        {
            item.ActivationsNeeded.Add(this);
        }
        
    }

    private void Update()
    {
        isCorrupted = interactScript.corrupted;

        if (!isCorrupted) Detection();
    }

    public void ActivateObject()
    {
        if (!isCorrupted && (type == ActivatorType.PRESSUREPLATE || type == ActivatorType.LEVER))
        {
            isActivated = !isActivated;
            foreach (AXD_Activable item in objectsToActivate)
            {
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
            if (type == ActivatorType.PRESSUREPLATE && (col.gameObject.CompareTag("Crate") || col.gameObject.CompareTag("Ryn")))
            {
                if (!isActivated)
                {
                    isActivated = true;
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
                    isActivated = true;
                    StopCoroutine("Countdown");
                    StartCoroutine("Countdown");
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
    }
}
