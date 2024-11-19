using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDash : MonoBehaviour
{
    [SerializeField] private float dashSpeed = 15f;
    [SerializeField] private float dashDuration = 0.2f;
    [SerializeField] private float invulnerabilityDuration = 0.15f;
    [SerializeField] private float dashCooldown = 1f;

    private bool isDashing = false;
    private bool isInvulnerable = false;
    private float dashCooldownTimer = 0f;

    private Rigidbody2D rb;
    private Vector2 dashDirection;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        if (dashCooldownTimer > 0)
        {
            dashCooldownTimer -= Time.deltaTime;
        }

        if (Input.GetKeyDown(KeyCode.Space) && !isDashing && dashCooldownTimer <= 0)
        {
            StartDash();
        }
    }

    private void StartDash()
    {
        isDashing = true;
        dashCooldownTimer = dashCooldown;

        dashDirection = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical")).normalized;

        StartCoroutine(Dash());
    }

    IEnumerator Dash()
    {
        StartInvulnerability();

        float dashTimer = 0f;
        while (dashTimer < dashDuration)
        {
            rb.velocity = dashDirection * dashSpeed;
            dashTimer += Time.deltaTime;
            yield return null;
        }

        rb.velocity = Vector2.zero;

        isDashing = false;
        EndInvulnerability();
    }

    private void StartInvulnerability()
    {
        isInvulnerable = true;

        Invoke(nameof(EndInvulnerability), invulnerabilityDuration);
    }

    private void EndInvulnerability()
    {
        isInvulnerable = false;
    }
}
