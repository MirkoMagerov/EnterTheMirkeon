using System.Collections;
using UnityEngine;

public class Shake : MonoBehaviour
{
    public static Shake Instance;

    public float duration = 0.2f;
    public AnimationCurve curve;

    private void Awake()
    {
        Instance = this;
    }

    public void TriggerShake()
    {
        StartCoroutine(Shaking());
    }

    private IEnumerator Shaking()
    {
        Vector3 startPosition = transform.position;
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float strength = curve.Evaluate(elapsedTime / duration);

            Vector2 shakeOffset = Random.insideUnitCircle * strength;
            //transform.position = new Vector3(startPosition.x + shakeOffset.x, startPosition.y + shakeOffset.y, startPosition.z);
            CameraController.Instance.ApplyShakeOffset(new Vector3(shakeOffset.x, shakeOffset.y, 0));

            yield return null;
        }

        //transform.position = startPosition;
        CameraController.Instance.ApplyShakeOffset(Vector3.zero);
    }
}
