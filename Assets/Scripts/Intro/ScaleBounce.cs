using UnityEngine;

public class ScaleBounce : MonoBehaviour
{
    public float scaleAmount = 1.1f;
    public float speed = 2f;

    private Vector3 originalScale;

    void Start()
    {
        originalScale = transform.localScale;
    }

    void Update()
    {
        float scale = Mathf.Lerp(1f, scaleAmount, (Mathf.Sin(Time.time * speed) + 1f) / 2f);
        transform.localScale = originalScale * scale;
    }
}
