using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;

    private Vector2 knockbackForce;
    private Animator anim;
    private Vector2 movementInput;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();

        InputManager.Instance.GetInputActions().Movement.Direction.performed += OnMovement;
    }

    private void Update()
    {
        HandleAnimations();
    }

    private void FixedUpdate()
    {
        if (PlayerState.Instance.IsDashing) return;

        Vector2 moveVelocity = movementInput.normalized * moveSpeed;

        rb.velocity = moveVelocity + knockbackForce;

        knockbackForce = Vector2.Lerp(knockbackForce, Vector2.zero, Time.fixedDeltaTime * 5f);
    }

    private void OnDisable()
    {
        InputManager.Instance.GetInputActions().Movement.Direction.performed -= OnMovement;
    }

    private void OnMovement(InputAction.CallbackContext context)
    {
        movementInput = context.ReadValue<Vector2>();
    }

    private void HandleAnimations()
    {
        if (rb.velocity != Vector2.zero)
        {
            anim.SetBool("Running", true);
        }
        else
        {
            anim.SetBool("Running", false);
        }
    }

    public void ApplyKnockback(Vector2 force)
    {
        knockbackForce = force;
    }
}
