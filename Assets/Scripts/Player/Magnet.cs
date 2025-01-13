using UnityEngine;

public class Magnet : MonoBehaviour
{
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Coin coin))
        {
            coin.SetTarget(transform.parent.position);
        }
    }
}
