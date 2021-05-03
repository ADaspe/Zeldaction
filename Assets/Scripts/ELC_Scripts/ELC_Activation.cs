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
    public bool isCorrupted;
    private ELC_Interact interactScript;
    public float detectionRadius;
    public AXD_Activable objectToActivate;

    private void Start()
    {
        interactScript = this.GetComponent<ELC_Interact>();
        objectToActivate.ActivationsNeeded.Add(this);
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
            objectToActivate.Activate();
        }
    }

    private void Detection()
    {
        Collider2D[] detected = Physics2D.OverlapCircleAll(this.transform.position, detectionRadius);

        foreach (Collider2D col in detected)
        {
            if (type == ActivatorType.PRESSUREPLATE && col.gameObject.CompareTag("Crate") || col.gameObject.CompareTag("Ryn"))
            {
                isActivated = true;
                objectToActivate.Activate();
                return;
            }
            else if(type == ActivatorType.TORCH && col.gameObject.CompareTag("Spirit") && col.gameObject.GetComponent<AXD_CharacterMove>().isDashing)
            {
                isActivated = true;
                StopCoroutine("Countdown");
                StartCoroutine("Countdown");
                objectToActivate.Activate();
                return;
            }
            else isActivated = false;
        }
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(TorchDuration);
        isActivated = false;
    }
}
