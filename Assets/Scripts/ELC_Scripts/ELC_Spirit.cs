using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ELC_Spirit : MonoBehaviour
{
    [SerializeField]
    private GameObject MiaGO;
    private AXD_Mia MiaScript;
    [SerializeField]
    private float angleDeg;
    void Update()
    {
        TurnAroundPlayer();
    }

    private void TurnAroundPlayer()
    {
        Vector3 zAxis = new Vector3(0, 0, 1);
        transform.RotateAround(MiaGO.transform.position, zAxis, 3f); //Pour faire tourner autour du perso

        if (MiaGO.transform.position.x < this.transform.position.x) this.transform.Translate(new Vector3(0, -1.5f, 0) * Time.deltaTime); //Si l'esprit est en dessous à droite ou au dessus à gauche on éloigne l'esprit
        else this.transform.Translate(new Vector3(0, 1.5f, 0) * Time.deltaTime);
        if (MiaGO.transform.position.y < this.transform.position.y) this.transform.Translate(new Vector3(-1.5f, 0, 0) * Time.deltaTime);
        else this.transform.Translate(new Vector3(1.5f, 0, 0) * Time.deltaTime);
        Debug.DrawRay(this.transform.position, Vector3.down * 0.1f, Color.red, Mathf.Infinity);


        //Vector3 ToPlayer = MiaGO.transform.position - this.transform.position;
        //float angleDeg = Mathf.Atan2(ToPlayer.y, ToPlayer.x) * Mathf.Rad2Deg;

    }
}
