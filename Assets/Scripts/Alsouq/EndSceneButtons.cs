using UnityEngine;
using UnityEngine.SceneManagement;

public class EndSceneButtons : MonoBehaviour
{
    public void OnTryAgain()
    {
        string lastScene = PlayerPrefs.GetString("LastPlayedLevel", "");
        if (!string.IsNullOrEmpty(lastScene))
        {
            Debug.Log("🔁 Reloading level: " + lastScene);
            SceneManager.LoadScene(lastScene);
        }
        else
        {
            Debug.LogWarning("⚠ No saved level found to retry.");
        }
    }

    public void OnPlayAnotherGame()
    {
        // TODO: Replace with actual scene name for your level selection
        Debug.Log("🎮 Going to level selection...");
        // SceneManager.LoadScene("LevelSelection");
    }
}
