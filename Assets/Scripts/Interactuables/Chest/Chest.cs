using OutlineFx;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Chest : MonoBehaviour, IInteractuable
{
    [Header("Chest Settings")]
    public List<Weapon> weaponsList;

    [Header("Interaction Settings")]
    public float interactionRange = 2f;
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
            int randomIndex = Random.Range(0, weaponsList.Count);
            WeaponInventory weaponInv = GameObject.FindGameObjectWithTag("Player").GetComponentInChildren<WeaponInventory>();
            {
                weaponInv.PickUpWeapon(weaponsList[randomIndex]);
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
