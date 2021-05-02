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

    public void ActivateObject()
    {
        isActivated = !isActivated;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(type == ActivatorType.PRESSUREPLATE)
        {
            if (collision.gameObject.CompareTag("Crate") || collision.gameObject.CompareTag("Ryn"))
            {
                isActivated = true;
            }
            else isActivated = false;
        }
        else if(type == ActivatorType.TORCH)
        {
            if(collision.gameObject.CompareTag("Spirit"))
            {
                if (collision.gameObject.GetComponent<AXD_CharacterMove>().isDashing)
                {
                    isActivated = true;
                    StopCoroutine("Countdown");
                    StartCoroutine("Countdown");
                }
            }
        }
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(TorchDuration);
        isActivated = false;
    }
}
