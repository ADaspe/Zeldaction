using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AXD_MushroomSpawner : AXD_Activable
{
    public ELC_GameManager gm;
    public AXD_Mushroom mushroomPrefab;
    public AXD_Mushroom ActiveMushroom;
    public float MushroomNextrSpawn;
    public float secondsToAddToExplodingTimeForSpawning;
    private int currentNumberOfActivation;

    public override void Activate()
    {
        if (ActivationsNeeded.Count != 0)
        {
            currentNumberOfActivation = 0;

            foreach (ELC_Activation active in ActivationsNeeded)
            {
                if (active.isActivated) currentNumberOfActivation++;
            }
            if (currentNumberOfActivation == ActivationsNeeded.Count)
            {
                if (!isActivated)
                {
                    isActivated = true;
                    LockTorches();
                    if (ObjectAnimator != null) // Pas de null pointer exception :)
                    {
                        ObjectAnimator.SetBool("Activated", isActivated);
                    }
                }
                return;
            }
            else
            {
                isActivated = false;
                if (ObjectAnimator != null) // Pas de null pointer exception :)
                {
                    ObjectAnimator.SetBool("Activated", isActivated);
                }
            }
        }
    }

    private void FixedUpdate()
    {
        if (isActivated)
        {
            if(ActiveMushroom == null && MushroomNextrSpawn < Time.time)
            {
                MushroomNextrSpawn = mushroomPrefab.ExplodingTime + secondsToAddToExplodingTimeForSpawning;
                ActiveMushroom = Instantiate(mushroomPrefab, transform.position, Quaternion.identity);
                ActiveMushroom.GetComponent<ELC_Interact>().GameManagerScript = gm;
            }
        }
    }

}
