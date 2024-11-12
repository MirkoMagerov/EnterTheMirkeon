using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MouseAim : MonoBehaviour
{
    [SerializeField] private GameObject crosshair_1;

    private GameObject playerGameObject;
    private Vector2 mousePos = Vector2.zero;
    private Vector2 playerToMouseDirection = Vector2.zero;

    #region Other Scripts References
    PlayerFlip playerFlip;
    #endregion

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        playerGameObject = GameObject.FindGameObjectWithTag("Player");
        playerFlip = playerGameObject.GetComponent<PlayerFlip>();
    }

    private void Update()
    {
        playerToMouseDirection = GetDirectionToCrosshair(playerGameObject.transform.position);

        FlipPlayerLocalScale();
    }

    private void LateUpdate()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        crosshair_1.transform.position = new Vector2(mousePos.x, mousePos.y);
    }

    private Vector2 GetDirectionToCrosshair(Vector2 position)
    {
        return (mousePos - position).normalized;
    }

    private void FlipPlayerLocalScale()
    {
        Vector3 playerLocalScale = playerGameObject.transform.localScale;

        if (mousePos.x < playerGameObject.transform.position.x && playerLocalScale.x > 0)
        {
            playerFlip.SetPlayerLocalScale(new Vector3(-Mathf.Abs(playerLocalScale.x), playerLocalScale.y, playerLocalScale.z));
        }
        else if (mousePos.x > playerGameObject.transform.position.x && playerLocalScale.x < 0)
        {
            playerFlip.SetPlayerLocalScale(new Vector3(Mathf.Abs(playerLocalScale.x), playerLocalScale.y, playerLocalScale.z));
        }
    }

    private void OnDrawGizmos()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            Vector2 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;

            Gizmos.color = Color.red;
            Gizmos.DrawLine(playerPosition, playerPosition + playerToMouseDirection * 10f);
        }
    }
}
