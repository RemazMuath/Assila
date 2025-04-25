using UnityEngine;

public class CameraShaker : MonoBehaviour
{
    public static CameraShaker Instance;

    public float duration = 0.2f;
    public float magnitude = 0.2f;

    private Vector3 originalPos;

    void Awake()
    {
        Instance = this;
        originalPos = transform.localPosition;
    }

    public void Shake()
    {
        StopAllCoroutines();
        StartCoroutine(ShakeRoutine());
    }

    private System.Collections.IEnumerator ShakeRoutine()
    {
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.localPosition = originalPos + new Vector3(x, y, 0f);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.localPosition = originalPos;
    }
}
