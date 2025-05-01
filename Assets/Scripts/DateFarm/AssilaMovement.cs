using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;


public class AssilaMovement : MonoBehaviour
{
    [Header("Jump Physics")]
    public float baseJumpForce = 23f;
    public float maxJumpForce = 30f;
    public int maxJumps = 2;

    [Header("Platform Sync")]
    public float minJumpDistance = 3f;
    public float maxJumpDistance = 7f;

    private Rigidbody2D rb;
    private int jumpCount;
    private float lastPlatformX;

    void Start() => rb = GetComponent<Rigidbody2D>();

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) || (Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began))
        {
            TryJump();
        }

        if (transform.position.y < -10f) GameOver();
    }

    private void TryJump()
    {
        if (jumpCount >= maxJumps) return;

        rb.velocity = new Vector2(rb.velocity.x, 0f);
        rb.AddForce(Vector2.up * baseJumpForce, ForceMode2D.Impulse);
        jumpCount++;
    }


    private float GetNextPlatformDistance()
    {
        // Raycast to find next platform
        RaycastHit2D hit = Physics2D.Raycast(
            transform.position + Vector3.right * 2f,
            Vector2.right,
            maxJumpDistance * 1.5f,
            LayerMask.GetMask("Platform")
        );

        return hit.collider != null ? hit.distance : minJumpDistance;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        rb.velocity = new Vector2(rb.velocity.x, 0f);

        if (!col.gameObject.CompareTag("Platform1")) return;

        ContactPoint2D contact = col.contacts[0];
        if (contact.normal.y > 0.5f)
        {
            jumpCount = 0;
            lastPlatformX = col.transform.position.x;
        }
        else GameOver();
    }
    public void GameOver()
    {
        var timer = FindObjectOfType<GameTimerDF>();
        if (timer != null)
        {
            timer.StopTimer();
            float finalTime = timer.GetCurrentTime();

            // Get current difficulty first
            string difficulty = PlayerPrefs.GetString("Difficulty", "Easy");

            // Save times separately for each difficulty
            PlayerPrefs.SetFloat("LastTime_" + difficulty, finalTime);

            float bestTime = PlayerPrefs.GetFloat("BestTime_" + difficulty, 0f);
            if (finalTime > bestTime)
            {
                PlayerPrefs.SetFloat("BestTime_" + difficulty, finalTime);
                PlayerPrefs.Save();
                StartCoroutine(LoadSceneDelayed("WinSceneDF"));
                return;
            }

            // Save even if it's not a new best
            PlayerPrefs.Save();
        }

        StartCoroutine(LoadSceneDelayed("FailSceneDF"));
    }


    private IEnumerator LoadSceneDelayed(string sceneName)
    {
        yield return new WaitForSecondsRealtime(0.2f); // Give Unity time to finish saving
        Debug.Log("Loading scene: " + sceneName);
        SceneManager.LoadScene(sceneName);
    }

}

public static class ExtensionMethods
    {
        public static float Remap(this float value, float from1, float to1, float from2, float to2)
        {
            return (value - from1) / (to1 - from1) * (to2 - from2) + from2;
        }
    }
