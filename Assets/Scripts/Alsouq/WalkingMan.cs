using UnityEngine;
using UnityEngine.UI;

public class WalkingMan : MonoBehaviour
{
    public float bobAmplitude = 20f; // How high he bounces up/down
    public float bobFrequency = 2f;  // How fast he bounces

    public float travelTime = 2f; // How many seconds to reach center
    public float spawnYPosition = -150f; // control spawn Y position

    private RectTransform rectTransform;
    private Vector2 startPosition;
    private Vector2 targetPosition;
    private Vector2 exitPosition;

    private float timer = 0f;
    private bool walkingIn = true;
    private bool walkingOut = false;

    public Sprite standingSprite;    // Assign standing sprite
    public Sprite walkingSprite;     // Assign walking sprite
    public GameObject speechBubble;  // SpeechBubble
    public GameObject askedNameObject; // Asked name object

    private Image manImage;
    private Image askedNameImage;

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();

        startPosition = new Vector2(2500f, spawnYPosition);
        targetPosition = new Vector2(0f, spawnYPosition);
        exitPosition = new Vector2(-2500f, spawnYPosition);

        rectTransform.anchoredPosition = startPosition;

        manImage = GetComponent<Image>();

        if (speechBubble != null)
            speechBubble.SetActive(false);

        if (askedNameObject != null)
        {
            askedNameImage = askedNameObject.GetComponent<Image>();
            askedNameObject.SetActive(false);
        }
    }

    void Update()
    {
        if (walkingIn)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / travelTime);

            Vector2 walkPos = Vector2.Lerp(startPosition, targetPosition, t);
            walkPos.y += Mathf.Sin(Time.time * bobFrequency) * bobAmplitude; // Add bouncing
            rectTransform.anchoredPosition = walkPos;

            if (t >= 1f)
            {
                walkingIn = false;
                timer = 0f;

                if (manImage != null && standingSprite != null)
                    manImage.sprite = standingSprite;

                if (speechBubble != null)
                    speechBubble.SetActive(true);

                if (askedNameObject != null)
                    askedNameObject.SetActive(true);

                GameScene_Hard manager = FindObjectOfType<GameScene_Hard>();
                if (manager != null && askedNameImage != null)
                {
                    askedNameImage.sprite = manager.GetAskedNameSprite();
                    askedNameImage.preserveAspect = true;
                    manager.ConfirmAskedName();
                }
            }
        }
        else if (walkingOut)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / travelTime);

            Vector2 walkOutPos = Vector2.Lerp(targetPosition, exitPosition, t);
            walkOutPos.y += Mathf.Sin(Time.time * bobFrequency) * bobAmplitude; // Add bouncing out
            rectTransform.anchoredPosition = walkOutPos;

            if (t >= 1f)
            {
                walkingOut = false;

                GameScene_Hard manager = FindObjectOfType<GameScene_Hard>();
                if (manager != null)
                    manager.SpawnNewCustomer();

                Destroy(gameObject);
            }
        }
    }

    public void StartWalkingOut()
    {
        if (!walkingOut)
        {
            walkingOut = true;
            timer = 0f;

            if (speechBubble != null)
                speechBubble.SetActive(false);

            if (askedNameObject != null)
                askedNameObject.SetActive(false);

            if (manImage != null && walkingSprite != null)
                manImage.sprite = walkingSprite;
        }
    }
}
