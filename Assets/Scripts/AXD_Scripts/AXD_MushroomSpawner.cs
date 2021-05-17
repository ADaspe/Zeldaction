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

    public override void Activate()
    {
        LockTorches();
        isActivated = true;
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
