using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private float moveSpeed;
    private bool isKnockedBack = false;
    private Vector2 knockbackForce;
    private Animator anim;
    private Vector2 movementInput;
    private Rigidbody2D rb;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>();
    }

    private void Update()
    {
        HandleAnimations();
    }

    private void FixedUpdate()
    {
        if (PlayerState.Instance.IsDashing) return;

        Vector2 moveVelocity = movementInput.normalized * moveSpeed;

        // Reducir gradualmente la fuerza de knockback
        if (isKnockedBack)
        {
            knockbackForce = Vector2.Lerp(knockbackForce, Vector2.zero, Time.fixedDeltaTime * 2f);
            if (knockbackForce.magnitude < 0.1f)
            {
                isKnockedBack = false;
                knockbackForce = Vector2.zero;
            }
        }

        // Combinar movimiento con knockback
        rb.velocity = moveVelocity + knockbackForce;
    }

    private void OnEnable()
    {
        InputManager.Instance.GetInputActions().Movement.Direction.performed += OnMovement;
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
        isKnockedBack = true;
    }

    public IEnumerator DisableMovementCoroutine(float seconds)
    {
        float prevSpeed = moveSpeed;
        moveSpeed = 0;
        yield return new WaitForSeconds(seconds);
        moveSpeed = prevSpeed;
    }
}
