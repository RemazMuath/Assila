using UnityEngine;
using UnityEngine.SceneManagement;

public class DifficultySelectorDF : MonoBehaviour
{
    public void SetDifficulty(string difficulty)
    {
        PlayerPrefs.SetString("Difficulty", difficulty);
        Debug.Log("Difficulty set to: " + difficulty);

        SceneManager.LoadScene("MainFarm"); // Replace with your actual gameplay scene name
    }
}
