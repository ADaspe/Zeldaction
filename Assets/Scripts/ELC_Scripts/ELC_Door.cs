using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.InputSystem;

public class ELC_Door : AXD_Activable
{
    public bool ActivateOnDisable;
    public bool jingleOnFirstOpen;
    public bool IsOpenAtStart;
    public bool Pollen;
    public GameObject PollenParticles;

    public AudioManager audioManager;
    private Collider2D rb;
    private int currentNumberOfActivation;
    private bool openedOnce = false;

    [Header("Sounds Names")]
    public string openSound;
    public string closeSound;

    [Header("Cam Cinematic Parameters")]
    public bool openingCinematic;
    public float cinematicDuration = 2f;
    public ELC_CharacterManager charaMana;
    public CinemachineVirtualCamera vCam;
    public PlayerInput playerInput;
    private ELC_SwitchCamFocus switchCam;

    private void Start()
    {
        if (gameObject.layer != LayerMask.NameToLayer("ObstacleSpirit")) // sert à ne pas faire buguer sur le pollen qui n'a pas d'animator
        {
            ObjectAnimator = GetComponent<Animator>();
        }
        rb = this.GetComponent<Collider2D>();
        isActivated = IsOpenAtStart;
        rb.enabled = !IsOpenAtStart;
        
        if(openingCinematic)
        {
            switchCam = gameObject.AddComponent<ELC_SwitchCamFocus>();
            switchCam.CharaMana = charaMana;
            switchCam.vCam = vCam;
            switchCam.Inputs = playerInput;
        }
    }

    public void CheckActivations()
    {
        if (ActivationsNeeded.Count != 0)
        {
            currentNumberOfActivation = 0;

            foreach (ELC_Activation active in ActivationsNeeded)
            {
                if ((!ActivateOnDisable && active.isActivated) || (ActivateOnDisable && !active.isActivated)) currentNumberOfActivation++;
            }
            if (currentNumberOfActivation == ActivationsNeeded.Count)
            {
                if (!isActivated)
                {
                    isActivated = true;
                    PollenParticles.SetActive(false);
                    LockTorches();
                    rb.enabled = false;
                    if(!openedOnce)
                    {
                        openedOnce = true;
                        if(openingCinematic)
                        {
                            switchCam.SwitchCamFocus(this.transform, false);
                            StartCoroutine(CamSwapCanceler());
                        }
                        if (jingleOnFirstOpen)
                        {
                            audioManager.Play("Door_Open");
                        }
                    }                    
                    if (ObjectAnimator != null) // Pas de null pointer exception :)
                    {
                        ObjectAnimator.SetBool("Activated", isActivated);
                        if (openSound != "") audioManager.Play(openSound);
                    }
                }
                return;
            }
            else
            {
                isActivated = false;
                PollenParticles.SetActive(true);
                rb.enabled = true;
                if (ObjectAnimator != null) // Pas de null pointer exception :)
                {
                    ObjectAnimator.SetBool("Activated", isActivated);
                    if(closeSound != "") audioManager.Play(closeSound);
                }
            }
        }
        
    }

    public override void Activate()
    {        
        CheckActivations();
    }

    private IEnumerator CamSwapCanceler()
    {
        yield return new WaitForSeconds(cinematicDuration);
        switchCam.CancelCamFocus();
    }
}
