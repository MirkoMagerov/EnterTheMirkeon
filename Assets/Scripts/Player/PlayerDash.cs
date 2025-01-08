using System;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerDash : MonoBehaviour
{
    public static event Action<bool> OnDashStateChanged;

    [SerializeField] private float dashSpeed = 30f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float dashCooldown = 0.8f;

    private Rigidbody2D rb;
    private Vector2 dashDirection;
    private float dashCooldownTimer = 0f;
    private int originalLayer;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        InputManager.Instance.GetInputActions().Movement.Direction.performed += OnMovement;
        InputManager.Instance.GetInputActions().Movement.Dash.performed += OnDash;
        originalLayer = gameObject.layer;
    }

    void Update()
    {
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }
    }

    private void OnDisable()
    {
        InputManager.Instance.GetInputActions().Movement.Direction.performed -= OnMovement;
        InputManager.Instance.GetInputActions().Movement.Dash.performed -= OnDash;
    }

    private void OnMovement(InputAction.CallbackContext context)
    {
        dashDirection = context.ReadValue<Vector2>();
    }

    private void OnDash(InputAction.CallbackContext context)
    {
        if (dashCooldownTimer <= 0 && !PlayerState.Instance.IsDashing)
        {
            StartDash();
        }
    }

    private void StartDash()
    {
        PlayerState.Instance.StartInvulnerability();
        PlayerState.Instance.StartDash();
        dashCooldownTimer = dashCooldown;

        gameObject.layer = LayerMask.NameToLayer("Invulnerable");
        OnDashStateChanged?.Invoke(true);

        StartCoroutine(Dash());
    }

    IEnumerator Dash()
    {
        rb.velocity = new Vector2(dashDirection.x * dashSpeed, dashDirection.y * dashSpeed);

        yield return new WaitForSeconds(dashDuration);

        rb.velocity = Vector2.zero;
        PlayerState.Instance.StopDash();
        PlayerState.Instance.StopInvulnerability();
        gameObject.layer = originalLayer;

        OnDashStateChanged?.Invoke(false);
    }
}
