using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] private float smoothTime = 0.2f;
    [SerializeField] private float mouseInfluence = 0.3f;
    [SerializeField] private float deadZoneRadius = 1.0f;

    private GameObject player;
    private Vector3 shakeOffset = Vector3.zero;
    private Vector3 velocity = Vector3.zero;
    private bool following = false;
    private bool playerAlive = true;

    private void Awake()
    {
        EnableCameraFollowing();
    }

    private void OnEnable()
    {
        player.GetComponent<PlayerLife>().OnPlayerDeath += PlayerDead;
    }

    private void OnDisable()
    {
        player.GetComponent<PlayerLife>().OnPlayerDeath -= PlayerDead;
    }

    private void PlayerDead()
    {
        playerAlive = false;
        player = null;
    }

    public void EnableCameraFollowing()
    {
        player = GameManager.Instance.player;
        Vector3 newPosition = transform.position + player.transform.position;
        newPosition.z = -10;
        transform.position = newPosition;
        following = true;
        playerAlive = true;
    }

    private void LateUpdate()
    {
        if (following && playerAlive)
        {
            Vector3 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePosition.z = player.transform.position.z;

            Vector3 direction = mousePosition - player.transform.position;
            float distance = direction.magnitude;

            Vector3 targetPosition;

            if (distance < deadZoneRadius)
            {
                targetPosition = player.transform.position;
            }
            else
            {
                Vector3 influence = (distance - deadZoneRadius) * mouseInfluence * direction.normalized;
                targetPosition = player.transform.position + influence;
            }

            targetPosition.z = transform.position.z;
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref velocity, smoothTime);
            transform.position = Vector3.SmoothDamp(transform.position, targetPosition + shakeOffset, ref velocity, smoothTime);
        }
    }

    public void ApplyShakeOffset(Vector3 offset)
    {
        shakeOffset = offset;
    }
}
