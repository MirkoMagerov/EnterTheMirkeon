using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    PlayerMovementInputActions playerMovementInput;

    [SerializeField] private float moveSpeed;

    private Vector2 movementInput;
    private Rigidbody2D rb;
    private bool isDashing;

    private void Awake()
    {
        playerMovementInput = new PlayerMovementInputActions();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        if (isDashing) return;

        rb.velocity = movementInput.normalized * moveSpeed;
    }

    private void OnEnable()
    {
        playerMovementInput.Enable();
        playerMovementInput.PlayerMovement.Movement.performed += OnMovement;
        PlayerDash.OnDashStateChanged += HandleDashStateChanged;
    }

    private void OnDisable()
    {
        playerMovementInput.Disable();
        playerMovementInput.PlayerMovement.Movement.performed -= OnMovement;
    }

    private void OnMovement(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    private void HandleDashStateChanged(bool dashing)
    {
        isDashing = dashing;
    }
}
