using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FLC_OrderLayers : MonoBehaviour
{
    public GameObject mainParent;
    public int playerOrderNbr;
    public int shiftReach;

    private void OnTriggerStay2D(Collider2D collision)
    {
        if(collision.CompareTag("Perspective"))
        {
            //Debug.Log(PositionComparation(collision.gameObject));
            OrderLayers(PositionComparation(collision.gameObject), collision.gameObject);
        }
    }

    bool PositionComparation(GameObject target)
    {
        //Debug.Log("player : " + mainParent.transform.position.y);
        //Debug.Log("target : " + target.transform.position.y);
        if (target.transform.position.y > mainParent.transform.position.y)
        {
            //Debug.Log("Au dessus");
            return true;
        }
        else
        {
            //Debug.Log("En dessous");
            return false;
        }
    }

    void OrderLayers(bool isUp, GameObject target)
    {
        //Stock target children SP
        SpriteRenderer[] sp = target.GetComponentsInChildren<SpriteRenderer>();
        List<SpriteRenderer> renderers = new List<SpriteRenderer>();
        for (int i = 0; i < sp.Length; i++)
        {
            renderers.Add(sp[i]);
        }

        //Compare positions
        for (int i = 0; i < sp.Length; i++)
        {
            sp[i].sortingOrder = OrderCalculation(isUp, sp[i]);
        }
    }

    int OrderCalculation(bool isUpper, SpriteRenderer target) //Vérifie si modification nécessaire et renvoit l'order in layer final
    {
        int result = target.sortingOrder;
        
        if(target.sortingOrder < -shiftReach) //Empêche l'addition anormale des OrderCalculation faisant sortir les sorting order des bornes
        {
            result += shiftReach;
        }
        else if (target.sortingOrder > shiftReach)
        {
            result -= shiftReach;
        }
        else if (isUpper && target.sortingOrder > (-shiftReach + 1) && -(shiftReach - target.sortingOrder) > -shiftReach) //Est au dessus du joueur ET order in layer plus grand que la limite minimum
        {
            result = -(shiftReach - target.sortingOrder);
        }
        else if (!isUpper && target.sortingOrder < shiftReach - 1 && shiftReach + target.sortingOrder < shiftReach) //Est en dessous du joueur ET order in layer plus petit que la limite maximum
        {
            result = shiftReach + target.sortingOrder;
        }
        return result;
    }
}