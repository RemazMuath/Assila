using UnityEngine;
using UnityEngine.SceneManagement;  // To manage scene loading

public class SceneTransition : MonoBehaviour
{
    public float delayTime = 4f;  // Time delay before loading the next scene

    void Start()
    {
        // Start the scene transition after the delay
        Invoke("LoadNextScene", delayTime);
    }

    // This function will be called after the delay
    void LoadNextScene()
    {
        // Load the next scene (replace "LvlChoice" with the name of your scene)
        SceneManager.LoadScene("LvlChoice");
    }
}
