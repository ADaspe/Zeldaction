using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Sirenix.OdinInspector;

public class AXD_Mia : MonoBehaviour
{
    public Rigidbody2D rb;
    public PlayerInput inputs;
    private Vector2 rawInputMovement;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
    public void SpiritRelease()
    {
        //To define
        Debug.Log("Yet to define");
    }

    public void Move(InputAction.CallbackContext value)
    {
        Vector2 inputMovement = value.ReadValue<Vector2>();
        rawInputMovement = new Vector2(inputMovement.x, inputMovement.y);
    }

    public void Attack(InputAction.CallbackContext value)
    {
        if (value.started)
        {
            Debug.Log("Attack");
        }
    }

}
