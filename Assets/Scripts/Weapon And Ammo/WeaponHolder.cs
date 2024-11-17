using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    [SerializeField] private SpriteRenderer playerRenderer;
    [SerializeField] private SpriteRenderer weaponRenderer;

    private Weapon currentWeapon;
    private Vector2 mousePos = Vector2.zero;

    private void Start()
    {
        weaponRenderer.sortingLayerName = playerRenderer.sortingLayerName;
        weaponRenderer.sortingOrder = playerRenderer.sortingOrder;
        UpdateWeaponSprite();
    }

    void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        Vector2 direction = (mousePos - (Vector2)transform.position).normalized;
        transform.right = direction;

        Vector2 localScale = transform.localScale;
        if (direction.x < 0)
        {
            transform.localScale = new(localScale.x, -1);
        }
        else if (direction.x > 0)
        {
            transform.localScale = new(localScale.x, 1);
        }

        if (transform.eulerAngles.z > 15 && transform.eulerAngles.z < 165)
        {
            weaponRenderer.sortingOrder = playerRenderer.sortingOrder - 1;
        }
        else if (transform.eulerAngles.z > 180)
        {
            weaponRenderer.sortingOrder = playerRenderer.sortingOrder + 1;
        }
    }

    public void EquipWeapon(Weapon newWeapon)
    {
        currentWeapon = newWeapon;
        UpdateWeaponSprite();
    }

    private void UpdateWeaponSprite()
    {
        if (currentWeapon != null && currentWeapon.weaponSprite != null)
        {
            weaponRenderer.sprite = currentWeapon.weaponSprite;
        }
    }
}
