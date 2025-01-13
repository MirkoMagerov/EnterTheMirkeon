using UnityEngine;

public class MouseAim : MonoBehaviour
{
    [SerializeField] private GameObject crosshair_1;

    private GameObject playerGameObject;
    private Vector2 mousePos = Vector2.zero;

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
        FlipPlayerLocalScale();
    }

    private void LateUpdate()
    {
        if (GameManager.Instance.gamePaused) return;

        mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        crosshair_1.transform.position = new Vector2(mousePos.x, mousePos.y);
    }

    private void FlipPlayerLocalScale()
    {
        Vector3 playerSpriteLocalScale = playerFlip.GetPlayerSpriteScale();

        if (mousePos.x < playerGameObject.transform.position.x && playerSpriteLocalScale.x > 0)
        {
            playerFlip.SetPlayerSpriteLocalScale(new Vector3(-Mathf.Abs(playerSpriteLocalScale.x), playerSpriteLocalScale.y, playerSpriteLocalScale.z));
        }
        else if (mousePos.x > playerGameObject.transform.position.x && playerSpriteLocalScale.x < 0)
        {
            playerFlip.SetPlayerSpriteLocalScale(new Vector3(Mathf.Abs(playerSpriteLocalScale.x), playerSpriteLocalScale.y, playerSpriteLocalScale.z));
        }
    }
}
