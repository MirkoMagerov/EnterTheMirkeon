using OutlineFx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Chest : MonoBehaviour, IInteractuable
{
    [Header("Loot Settings")]
    public List<Weapon> weaponsList;

    [Header("Interaction Settings")]
    private bool isPlayerNearby = false;
    private bool isOpened = false;

    private Animator anim;
    private Outline outlineFx;

    void Start()
    {
        outlineFx = GetComponentInChildren<Outline>();
        outlineFx.enabled = false;
        anim = GetComponentInChildren<Animator>();
        anim.enabled = false;
    }

    private void OnEnable()
    {
        InputManager.Instance.GetInputActions().Interactuable.Interact.performed += OpenChest;
    }

    private void OnDisable()
    {
        InputManager.Instance.GetInputActions().Interactuable.Interact.performed -= OpenChest;
    }

    private void OpenChest(InputAction.CallbackContext context)
    {
        if (isPlayerNearby && !isOpened)
        {
            isOpened = true;
            StartCoroutine(OpenChestCoroutine());
        }
    }

    private IEnumerator OpenChestCoroutine()
    {
        SetOutline(false);
        anim.enabled = true;
        yield return new WaitForSeconds(anim.GetCurrentAnimatorClipInfo(0).Length);

        if (weaponsList.Count > 0)
        {
            WeaponInventory weaponInv = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<WeaponInventory>();

            Weapon selectedWeapon;
            int attempts = 0;

            do
            {
                int randomIndex = Random.Range(0, weaponsList.Count);
                selectedWeapon = weaponsList[randomIndex];
                attempts++;

                if (attempts >= weaponsList.Count)
                {
                    selectedWeapon = null;
                    break;
                }
            }
            while (weaponInv.HasWeapon(selectedWeapon));

            if (selectedWeapon != null)
            {
                weaponInv.PickUpWeapon(selectedWeapon);
            }
        }

        yield return null;
    }

    public void SetOutline(bool enable)
    {
        outlineFx.enabled = enable;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isOpened)
        {
            isPlayerNearby = true;
            SetOutline(true);
        }
    }

    public void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isOpened)
        {
            isPlayerNearby = false;
            SetOutline(false);
        }
    }
}
