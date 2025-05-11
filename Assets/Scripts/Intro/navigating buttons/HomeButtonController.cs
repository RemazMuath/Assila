using UnityEngine;
using UnityEngine.SceneManagement;

public class HomeButtonController : MonoBehaviour
{
    [Header("UI Elements")]
    public GameObject confirmationPanel;

    private bool isPaused = false;

    // Call this when the home button is clicked
    public void OnHomeButtonPressed()
    {
        Time.timeScale = 0f; // Pause the game
        confirmationPanel.SetActive(true);
        isPaused = true;
    }

    // Call this when the "Yes" button is clicked
    public void OnConfirmHome()
    {
        Time.timeScale = 1f; // Resume time before switching scenes
        SceneManager.LoadScene("Tammar"); // Replace with your actual scene name
    }

    // Call this when the "No" button is clicked
    public void OnCancelHome()
    {
        confirmationPanel.SetActive(false);
        Time.timeScale = 1f; // Resume the game
        isPaused = false;
    }
}
