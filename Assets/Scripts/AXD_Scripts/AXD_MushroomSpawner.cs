using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AXD_MushroomSpawner : MonoBehaviour
{
    public bool activated;
    public AXD_Mushroom mushroomPrefab;
    public AXD_Mushroom ActiveMushroom;
    public float MushroomNextrSpawn;
    public float secondsToAddToExplodingTimeForSpawning;

    private void FixedUpdate()
    {
        if (activated)
        {
            if(ActiveMushroom == null && MushroomNextrSpawn >= Time.time)
            {
                MushroomNextrSpawn = mushroomPrefab.ExplodingTime + secondsToAddToExplodingTimeForSpawning;
                ActiveMushroom = Instantiate(mushroomPrefab, transform);
            }
        }
    }
}
