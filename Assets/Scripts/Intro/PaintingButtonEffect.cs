using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class PaintingButtonEffect : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Vector3 hoverScale = new Vector3(1.2f, 1.2f, 1.2f);
    private Vector3 originalScale;
    public string sceneToLoad = "difficultyselectionalsouq"; // ← Change to your scene name

    void Start()
    {
        originalScale = transform.localScale;
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        transform.localScale = hoverScale;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        transform.localScale = originalScale;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        SceneManager.LoadScene(sceneToLoad);
    }
}
