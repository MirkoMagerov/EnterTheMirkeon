using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseAim : MonoBehaviour
{
    [SerializeField] private GameObject crosshair_1;
    private Vector2 mousePos = Vector2.zero;
    private GameObject player;
    private Vector2 playerToMouseDirection = Vector2.zero;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Confined;
        player = GameObject.FindGameObjectWithTag("Player");
        
    }

    private void Update()
    {
        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        crosshair_1.transform.position = new Vector2(mousePos.x, mousePos.y);

        playerToMouseDirection = GetDirectionToCrosshair(player.transform.position);
    }

    private Vector2 GetDirectionToCrosshair(Vector2 position)
    {
        return (mousePos - position).normalized;
    }

    private void OnDrawGizmos()
    {
        if (GameObject.FindGameObjectWithTag("Player") != null)
        {
            Vector2 playerPosition = GameObject.FindGameObjectWithTag("Player").transform.position;

            // Dibujar una línea en la dirección del jugador hacia el mouse
            Gizmos.color = Color.red;
            Gizmos.DrawLine(playerPosition, playerPosition + playerToMouseDirection * 5f); // Ajusta el factor para cambiar la longitud de la línea
        }
    }
}
