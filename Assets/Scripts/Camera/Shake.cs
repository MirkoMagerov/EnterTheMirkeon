using System.Collections;
using UnityEngine;

public class Shake : MonoBehaviour
{
    public static Shake Instance;

    public float duration = 0.2f;
    public AnimationCurve curve;
    private CameraController cameraController;

    private void Awake()
    {
        Instance = this;
        cameraController = GetComponent<CameraController>();
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
            cameraController.ApplyShakeOffset(new Vector3(shakeOffset.x, shakeOffset.y, 0));

            yield return null;
        }

        cameraController.ApplyShakeOffset(Vector3.zero);
    }
}
