using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;

public class MeleeAttack : MonoBehaviour
{
    private bool isMeleeAttacking = false;
    private float attackAnimationDuration;
    private Coroutine meleeCoroutine;
    private WeaponController weaponController;
    private WeaponHolder weaponHolder;
    private Animator animator;

    private void Start()
    {
        weaponController = GetComponentInParent<WeaponController>();
        weaponHolder = GetComponentInParent<WeaponHolder>();
        animator = GetComponentInChildren<Animator>();

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == "AttackSword")
            {
                attackAnimationDuration = clip.length;
                break;
            }
        }
    }

    private void OnEnable()
    {
        var inputActions = InputManager.Instance.GetInputActions();
        inputActions.WeaponSystem.MeleeAttack.performed += HandleMeleeAttack;
    }

    private void OnDisable()
    {
        var inputActions = InputManager.Instance.GetInputActions();
        inputActions.WeaponSystem.MeleeAttack.performed -= HandleMeleeAttack;
    }

    private void HandleMeleeAttack(InputAction.CallbackContext context)
    {
        if (isMeleeAttacking) return;
        meleeCoroutine ??= StartCoroutine(MeleeAttackCoroutine());
    }

    private IEnumerator MeleeAttackCoroutine()
    {
        weaponHolder.SetSword(true);
        PlayerSounds.Instance.PlaySwordAttackSound();
        isMeleeAttacking = true;
        weaponController.StartMeleeAttack();
        animator.SetTrigger("MeleeAttack");

        yield return new WaitForSeconds(attackAnimationDuration);

        isMeleeAttacking = false;
        weaponHolder.SetSword(false);
        weaponController.StopMeleeAttack();

        meleeCoroutine = null;
    }
}
