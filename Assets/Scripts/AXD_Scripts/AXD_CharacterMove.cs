using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AXD_CharacterMove : MonoBehaviour
{
    public Rigidbody2D rb;
    public PlayerInput inputs;
    public ELC_CharacterManager charaManager;
    public Vector2 rawInputMovement;
    public bool canMove;
    public float speed;
    public bool currentCharacter;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        if (canMove && currentCharacter)
        {
            rb.velocity = rawInputMovement;
        }
    }

    public void Move(InputAction.CallbackContext value)
    {
        Vector2 inputMovement = value.ReadValue<Vector2>()*speed;
        rawInputMovement = new Vector2(inputMovement.x, inputMovement.y);
    }
}
