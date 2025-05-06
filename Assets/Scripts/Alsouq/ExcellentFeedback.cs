using UnityEngine;
using System.Collections;

public class ExcellentFeedback : MonoBehaviour
{
    public GameObject excellentImage;
    public float scaleTime = 0.3f;
    public float displayTime = 1.5f;
    public Vector3 targetScale = Vector3.one;

    void Start()
    {
        excellentImage.SetActive(false);
    }

    public void ShowExcellent()
    {
        StartCoroutine(ShowAndScale());
    }

    IEnumerator ShowAndScale()
    {
        excellentImage.SetActive(true);
        excellentImage.transform.localScale = Vector3.zero;

        float elapsed = 0f;
        while (elapsed < scaleTime)
        {
            excellentImage.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, elapsed / scaleTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        excellentImage.transform.localScale = targetScale;

        yield return new WaitForSeconds(displayTime);

        excellentImage.SetActive(false);
    }
}
