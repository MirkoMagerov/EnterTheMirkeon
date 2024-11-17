using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponVisual : MonoBehaviour
{
    public WeaponController weaponController;
    private SpriteRenderer spriteRenderer;

    void Start()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (weaponController != null && weaponController.currentWeapon != null)
        {
            UpdateWeaponSprite(weaponController.currentWeapon.weaponSprite);
        }
    }

    void Update()
    {
        if (weaponController.currentWeapon != null && spriteRenderer.sprite != weaponController.currentWeapon.weaponSprite)
        {
            UpdateWeaponSprite(weaponController.currentWeapon.weaponSprite);
        }
    }

    public void UpdateWeaponSprite(Sprite newSprite)
    {
        spriteRenderer.sprite = newSprite;
    }
}
