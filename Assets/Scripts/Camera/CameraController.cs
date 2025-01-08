using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance;

    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] private float mouseInfluence = 0.3f;
    [SerializeField] private float deadZoneRadius = 1.0f;

    private Transform player;
    private Vector3 shakeOffset = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    private bool following = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void LateUpdate()
    {
        if (following)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = player.position.z;

            Vector3 direction = mousePosition - player.position;
            float distance = direction.magnitude;

            Vector3 targetPosition;

            if (distance < deadZoneRadius)
            {
                targetPosition = player.position;
            }
            else
            {
                Vector3 influence = (distance - deadZoneRadius) * mouseInfluence * direction.normalized;
                targetPosition = player.position + influence;
            }

            targetPosition.z = transform.position.z;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition + shakeOffset, ref velocity, smoothTime);
        }
    }

    public void EnableCameraFollow(Transform player)
    {
        transform.position = player.position;
        this.player = player;
        following = true;
    }

    public void ApplyShakeOffset(Vector3 offset)
    {
        shakeOffset = offset;
    }
}
