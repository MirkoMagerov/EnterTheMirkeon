using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponHolder : MonoBehaviour
{
    [SerializeField] private SpriteRenderer playerRenderer;

    private Vector2 mousePos = Vector2.zero;
    private SpriteRenderer weaponRenderer;

    #region Other Scripts References
    private MouseAim mouseAim;
    #endregion

    private void Start()
    {
        mouseAim = GetComponent<MouseAim>();
        weaponRenderer = GetComponentInChildren<SpriteRenderer>();
        weaponRenderer.sortingLayerName = playerRenderer.sortingLayerName;
    }

    void Update()
    {
        mousePos = mouseAim.GetMousePos();

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
}
