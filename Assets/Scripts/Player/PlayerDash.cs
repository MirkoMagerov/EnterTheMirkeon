using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDash : MonoBehaviour
{
    PlayerMovementInputActions playerMovementInput;

    public delegate void DashStateChanged(bool isDashing);
    public static event DashStateChanged OnDashStateChanged;

    [SerializeField] private float dashSpeed = 30f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float invulnerabilityDuration = 0.2f;
    [SerializeField] private float dashCooldown = .8f;

    private bool isDashing = false;
    private bool isInvulnerable = false;
    private float dashCooldownTimer = 0f;

    private Rigidbody2D rb;
    private Vector2 dashDirection;

    private void Awake()
    {
        playerMovementInput = new PlayerMovementInputActions();
    }

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (isDashing) return;

        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isDashing && dashCooldownTimer <= 0)
        {
            StartDash();
        }
    }

    private void OnEnable()
    {
        playerMovementInput.Enable();
        playerMovementInput.PlayerMovement.Movement.performed += OnMovement;
    }

    private void OnDisable()
    {
        playerMovementInput.Disable();
        playerMovementInput.PlayerMovement.Movement.performed -= OnMovement;
    }

    private void OnMovement(InputAction.CallbackContext context)
    {
        dashDirection = context.ReadValue<Vector2>();
    }

    private void StartDash()
    {
        isInvulnerable = true;
        isDashing = true;
        dashCooldownTimer = dashCooldown;

        OnDashStateChanged?.Invoke(true);

        StartCoroutine(Dash());
    }

    IEnumerator Dash()
    {
        rb.velocity = new Vector2(dashDirection.x * dashSpeed, dashDirection.y * dashSpeed);

        yield return new WaitForSeconds(dashDuration);

        rb.velocity = Vector2.zero;
        isDashing = false;
        isInvulnerable = false;

        OnDashStateChanged?.Invoke(false);
    }
}
