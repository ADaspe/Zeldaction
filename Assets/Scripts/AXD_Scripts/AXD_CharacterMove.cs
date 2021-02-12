using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AXD_CharacterMove : MonoBehaviour
{
    public Rigidbody2D rb;
    public PlayerInput inputs;
    private Vector2 rawInputMovement;
    public bool canMove;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (canMove)
        {
            rb.velocity = rawInputMovement;
        }
    }

    public void Move(InputAction.CallbackContext value)
    {
        Vector2 inputMovement = value.ReadValue<Vector2>();
        rawInputMovement = new Vector2(inputMovement.x, inputMovement.y);
    }
}
