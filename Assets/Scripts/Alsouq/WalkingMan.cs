using UnityEngine;
using UnityEngine.UI;

public class WalkingMan : MonoBehaviour
{
    public float travelTime = 2f; // How many seconds to reach center
    private RectTransform rectTransform;
    private Vector2 startPosition;
    private Vector2 targetPosition = new Vector2(0f, 0f);
    private Vector2 exitPosition = new Vector2(-2500f, 0f); // Exit to left
    private float timer = 0f;
    private bool walkingIn = true;
    private bool waiting = false;
    private bool walkingOut = false;

    public Sprite standingSprite; // Assign standing sprite
    public Sprite walkingSprite;  // 🆕 Assign walking sprite (the original one)
    public GameObject speechBubble; // Speech bubble reference

    private Image manImage; // 🆕 Cache the Image component

    void Start()
    {
        rectTransform = GetComponent<RectTransform>();
        startPosition = rectTransform.anchoredPosition;

        manImage = GetComponent<Image>(); // 🆕 Cache this once

        if (speechBubble != null)
            speechBubble.SetActive(false);
    }

    void Update()
    {
        if (walkingIn)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / travelTime);
            rectTransform.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, t);

            if (t >= 1f)
            {
                walkingIn = false;
                waiting = true;
                timer = 0f;
                if (manImage != null && standingSprite != null)
                    manImage.sprite = standingSprite;

                if (speechBubble != null)
                    speechBubble.SetActive(true);

                GameScene_Hard manager = FindObjectOfType<GameScene_Hard>();
                if (manager != null)
                {
                    manager.StartCustomerRequest();
                }
            }
        }
        else if (waiting)
        {
            timer += Time.deltaTime;
            if (timer >= 2f) // Stay standing for 2 seconds
            {
                waiting = false;
                walkingOut = true;
                timer = 0f;

                if (speechBubble != null)
                    speechBubble.SetActive(false);

                if (manImage != null && walkingSprite != null) // 🆕 Back to walking sprite!
                    manImage.sprite = walkingSprite;
            }
        }
        else if (walkingOut)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / travelTime);
            rectTransform.anchoredPosition = Vector2.Lerp(targetPosition, exitPosition, t);

            if (t >= 1f)
            {
                walkingOut = false;

                GameScene_Hard manager = FindObjectOfType<GameScene_Hard>();
                if (manager != null)
                {
                    manager.SpawnNewCustomer();
                }

                Destroy(gameObject); // Remove old man after exiting
            }
        }
    }
}
