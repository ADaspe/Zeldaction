using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ELC_Activation : MonoBehaviour
{
    public enum ActivatorType {TORCH, LEVER, PRESSUREPLATE};
    public ActivatorType type;
    public ParticleSystem ActivationParticles;
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
    private AudioManager audioManager;
    public bool definitivelyActivated;
    [HideInInspector]
    public bool ticTacTorch;
    private static bool ticTacEnabled;
    private bool LastActivationState;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        interactScript = this.GetComponent<ELC_Interact>();
        audioManager = interactScript.GameManagerScript.GetComponentInChildren<AudioManager>();
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
            CheckSounds();
            foreach (AXD_Activable item in objectsToActivate)
            {
                //Debug.Log(gameObject.name + " Activate " + item.gameObject.name);
                item.Activate();
            }
        }
        else if(!interactScript.corrupted && type == ActivatorType.TORCH)
        {
            StopCoroutine("Countdown");
            StartCoroutine("Countdown");
            isActivated = true;
            AnimatorUpdate();
            ActivationParticles.Play();
            if (!ticTacEnabled)
            {
                audioManager.Play("TicTac");
            }
            foreach (AXD_Activable item in objectsToActivate)
            {
                item.Activate();
            }
            CheckSounds();
            ConditionsEnabled = true;
            return;
        }
    }

    private void FixedUpdate()
    {
        if(type == ActivatorType.PRESSUREPLATE)
        {
            bool itemOnPlate = false;
            Collider2D[] detected = Physics2D.OverlapCircleAll(new Vector2(transform.position.x + objectCollider.offset.x, transform.position.y + objectCollider.offset.y), detectionRadius);
            foreach (Collider2D item in detected)
            {
                if ((item.CompareTag("Crate") || item.CompareTag("Ryn")))
                {
                    itemOnPlate = true;
                }
            }
            
            isActivated = itemOnPlate;
            
            if (itemOnPlate != LastActivationState)
            {
                LastActivationState = itemOnPlate;
                CheckSounds();
                AnimatorUpdate();
                foreach (AXD_Activable item in objectsToActivate)
                {
                    item.Activate();
                }
            }
        }
    }

    private void Detection(Collider2D collision, bool isEntering = true)
    {
        //ConditionsEnabled = false;
        //if (type == ActivatorType.PRESSUREPLATE && (collision.gameObject.CompareTag("Crate") || collision.gameObject.CompareTag("Ryn")))
        //{
        //    bool itemOnPlate = false;
        //    Collider2D[] detected = Physics2D.OverlapCircleAll(new Vector2(transform.position.x + objectCollider.offset.x, transform.position.y + objectCollider.offset.y), detectionRadius);
        //    foreach (Collider2D item in detected)
        //    {
        //        if ((item.CompareTag("Crate") || item.CompareTag("Ryn")))
        //        {
        //            itemOnPlate = true;
        //        }
        //    }
        //    if (!isActivated && itemOnPlate)
        //    {
        //        isActivated = true;
        //        AnimatorUpdate();
        //        foreach (AXD_Activable item in objectsToActivate)
        //        {
        //            item.Activate();
        //        }
        //        CheckSounds();
        //    }

        //    if(!itemOnPlate)
        //    {
        //        isActivated = false;
        //        AnimatorUpdate();
        //        foreach (AXD_Activable item in objectsToActivate)
        //        {
        //            item.Activate();
        //        }
        //        CheckSounds();
        //    }

        //    ConditionsEnabled = (isEntering || itemOnPlate);
        //    //return;
        //}


        if (type == ActivatorType.TORCH && collision.gameObject.CompareTag("Spirit") && collision.gameObject.GetComponent<AXD_CharacterMove>().isDashing)
        {
            StopCoroutine("Countdown");
            StartCoroutine("Countdown");
            isActivated = true;
            AnimatorUpdate();
            ActivationParticles.Play();
            if (!ticTacEnabled)
            {
                audioManager.Play("TicTac");
            }
            foreach (AXD_Activable item in objectsToActivate)
            {
                item.Activate();
            }
            CheckSounds();
            ConditionsEnabled = true;
            return;
        }
        else if (type == ActivatorType.TORCH)
        {
            ConditionsEnabled = false;
            return;
        }
        

        if (type != ActivatorType.LEVER && type != ActivatorType.PRESSUREPLATE && ConditionsEnabled == false && isActivated)
        {
            isActivated = false;
            AnimatorUpdate();
            foreach (AXD_Activable item in objectsToActivate)
            {
                item.Activate();
            }
            CheckSounds();
        }

    }

    public void StopTicTac()
    {
        ticTacEnabled = false;
        audioManager.Stop("TicTac");
    }

    IEnumerator Countdown()
    {
        yield return new WaitForSeconds(TorchDuration);
        isActivated = false;
        StopTicTac();
        ActivationParticles.Stop();
        AnimatorUpdate();
    }


    private void AnimatorUpdate()
    {
        animator.SetBool("Activated", isActivated);
    }

    private void CheckSounds()
    {
        switch (type)
        {
            case ActivatorType.LEVER:
                audioManager.Play("Lever");
                return;
            case ActivatorType.TORCH:
                if (isActivated) audioManager.Play("Torch_On");
                Debug.Log("Torche Son");
                return;
            case ActivatorType.PRESSUREPLATE:
                if (isActivated) audioManager.Play("PressurePlate_On");
                else audioManager.Play("PressurePlate_Off");
                return;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    { 
        if(!definitivelyActivated) Detection(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if(!definitivelyActivated) Detection(collision , false);
    }
}
