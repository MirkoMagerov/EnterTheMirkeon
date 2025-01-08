using OutlineFx;
using UnityEngine;
using UnityEngine.InputSystem;

public enum DoorSide
{
    TopRight,
    TopLeft,
    RightTop,
    RightBottom,
    BottomRight,
    BottomLeft,
    LeftBottom,
    LeftTop
}

public class Door : MonoBehaviour, IInteractuable
{
    [Header("Interaction Settings")]
    public bool interactuable = true;
    public float interactionRange = 2f;
    private bool isPlayerNearby = false;

    [Header("Door settings")]
    [SerializeField] private Sprite[] doorSprites;
    public DoorSide side;
    private bool isOpened = false;

    private Outline outlineFx;
    private SpriteRenderer spriteRenderer;

    private void Start()
    {
        outlineFx = GetComponent<Outline>();
        outlineFx.enabled = false;
    }

    private void OnEnable()
    {
        InputManager.Instance.GetInputActions().Interactuable.Interact.performed += OpenDoor;
    }

    private void OnDisable()
    {
        InputManager.Instance.GetInputActions().Interactuable.Interact.performed -= OpenDoor;
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player") && !isOpened && interactuable)
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

    public void InitializeDoor()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (spriteRenderer == null)
        {
            Debug.LogError($"SpriteRenderer is missing in the Door prefab: {gameObject.name}");
            return;
        }
        spriteRenderer.sprite = GetDoorSprite(side);
    }

    private void OpenDoor(InputAction.CallbackContext context)
    {
        if (isPlayerNearby && !isOpened && interactuable)
        {
            interactuable = false;
            isOpened = true;
            spriteRenderer.sprite = null;
            foreach (var collider in GetComponents<Collider2D>())
            {
                collider.enabled = false;
            }
            SetOutline(false);
        }
    }

    public void Activate()
    {
        interactuable = true;
    }

    public void Deactivate()
    {
        interactuable = false;
        isOpened = false;
        spriteRenderer.sprite = GetDoorSprite(side);
        foreach (var collider in GetComponents<Collider2D>())
        {
            collider.enabled = true;
        }
    }

    public void SetOutline(bool enable)
    {
        outlineFx.enabled = enable;
    }

    Sprite GetDoorSprite(DoorSide side)
    {
        int index = (int)side;
        if (doorSprites == null || index < 0 || index >= doorSprites.Length)
        {
            Debug.LogError($"Invalid doorSprites array or index out of range for side: {side}");
            return null;
        }
        return doorSprites[index];
    }
}
