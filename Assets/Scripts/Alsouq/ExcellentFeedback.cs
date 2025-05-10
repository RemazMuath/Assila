using System.Collections;
using UnityEngine;

public class ExcellentFeedback : MonoBehaviour
{
    [Header("Correct Answer")]
    public GameObject excellentImage;

    [Header("Wrong Answer")]
    public GameObject wrongImage;

    [Header("Animation Settings")]
    public float scaleTime = 0.3f;
    public float displayTime = 1.5f;
    public Vector3 targetScale = Vector3.one;

    void Start()
    {
        if (excellentImage != null) excellentImage.SetActive(false);
        if (wrongImage != null) wrongImage.SetActive(false);
    }

    public void ShowExcellent()
    {
        if (excellentImage != null)
            StartCoroutine(ShowAndScale(excellentImage));
    }

    public void ShowWrong()
    {
        if (wrongImage != null)
            StartCoroutine(ShowAndScale(wrongImage));
    }

    private IEnumerator ShowAndScale(GameObject feedbackObject)
    {
        feedbackObject.SetActive(true);
        feedbackObject.transform.localScale = Vector3.zero;

        float elapsed = 0f;
        while (elapsed < scaleTime)
        {
            feedbackObject.transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, elapsed / scaleTime);
            elapsed += Time.deltaTime;
            yield return null;
        }

        feedbackObject.transform.localScale = targetScale;

        yield return new WaitForSeconds(displayTime);

        feedbackObject.SetActive(false);
    }
}
