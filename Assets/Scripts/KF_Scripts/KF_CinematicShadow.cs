using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KF_CinematicShadow : MonoBehaviour
{
    public GameObject shadow;

    public void RotateShadow0()
    {
        shadow.transform.SetPositionAndRotation(shadow.transform.position, new Quaternion(0, 0, 0, 0));
    }

    public void RotateShadow90()
    {
        shadow.transform.SetPositionAndRotation(shadow.transform.position, new Quaternion(0, 0, 90, 0));
    }
}
