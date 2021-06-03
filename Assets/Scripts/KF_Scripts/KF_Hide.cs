using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KF_Hide : MonoBehaviour
{
    public List<GameObject> objectsToHide;
    public bool setactive;
    public bool hide;
    public Animator animator;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ryn"))
        {
            if (setactive)
            {
                if (hide == true)
                {
                    foreach (GameObject go in objectsToHide)
                    {
                        go.SetActive(false);
                    }
                }
                else
                {
                    foreach (GameObject go in objectsToHide)
                    {
                        go.SetActive(true);
                    }
                }
            }
            else //there is an animator!
            {
                if (hide == true)
                {
                    foreach (GameObject go in objectsToHide)
                    {
                        animator = go.GetComponent<Animator>();
                        animator.SetTrigger("FadeOut");
                    }
                }
                else
                {
                    foreach (GameObject go in objectsToHide)
                    {
                        animator = go.GetComponent<Animator>();
                        animator.SetTrigger("FadeIn");
                    }
                }
            }

           
        }
        
        
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Ryn"))
        {
            if (setactive)
            {
                if (hide == true)
                {
                    foreach (GameObject go in objectsToHide)
                    {
                        go.SetActive(true);
                    }
                }
                else
                {
                    foreach (GameObject go in objectsToHide)
                    {
                        go.SetActive(false);
                    }
                }
            }
            else //there is an animator!
            {
                if (hide == true)
                {
                    foreach (GameObject go in objectsToHide)
                    {
                        animator = go.GetComponent<Animator>();
                        animator.SetTrigger("FadeIn");
                    }
                }
                else
                {
                    foreach (GameObject go in objectsToHide)
                    {
                        animator = go.GetComponent<Animator>();
                        animator.SetTrigger("FadeOut");
                    }
                }
            }


        }
            
    }
}
