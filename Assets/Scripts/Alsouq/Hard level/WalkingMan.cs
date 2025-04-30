using UnityEngine;
using UnityEngine.UI;

public class WalkingMan : MonoBehaviour
{
    public float bobAmplitude = 20f;
    public float bobFrequency = 2f;
    public float travelTime = 2f;
    public float spawnYPosition = -150f;

    private RectTransform rectTransform;
    private Vector2 startPosition;
    private Vector2 targetPosition;
    private Vector2 exitPosition;

    private float timer = 0f;
    private bool walkingIn = true;
    private bool walkingOut = false;

    public Sprite standingSprite;
    public Sprite walkingSprite;
    public GameObject speechBubble;
    public GameObject askedNameObject;

    private Image manImage;
    private Image askedNameImage;

    private enum LevelType { Easy, Medium, Hard }
    private LevelType currentLevel;

    private MonoBehaviour manager;

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

        // Detect level
        if (FindObjectOfType<GameScene_Easy>() != null)
        {
            manager = FindObjectOfType<GameScene_Easy>();
            currentLevel = LevelType.Easy;
        }
        else if (FindObjectOfType<GameScene_Medium>() != null)
        {
            manager = FindObjectOfType<GameScene_Medium>();
            currentLevel = LevelType.Medium;
        }
        else if (FindObjectOfType<GameScene_Hard>() != null)
        {
            manager = FindObjectOfType<GameScene_Hard>();
            currentLevel = LevelType.Hard;
        }
    }

    void Update()
    {
        if (walkingIn)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / travelTime);
            Vector2 walkPos = Vector2.Lerp(startPosition, targetPosition, t);
            walkPos.y += Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
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

                SetAskedNameImage();
            }
        }
        else if (walkingOut)
        {
            timer += Time.deltaTime;
            float t = Mathf.Clamp01(timer / travelTime);
            Vector2 walkOutPos = Vector2.Lerp(targetPosition, exitPosition, t);
            walkOutPos.y += Mathf.Sin(Time.time * bobFrequency) * bobAmplitude;
            rectTransform.anchoredPosition = walkOutPos;

            if (t >= 1f)
            {
                walkingOut = false;

                if (!IsGameOver()) // ✅ Prevent spawning after end
                    SpawnNewCustomer();

                Destroy(gameObject);
            }
        }
    }

    private void SetAskedNameImage()
    {
        if (manager == null || askedNameImage == null) return;

        if (currentLevel == LevelType.Easy)
        {
            var easy = manager as GameScene_Easy;
            askedNameImage.sprite = easy.GetAskedNameSprite();
            easy.ConfirmAskedName();
        }
        else if (currentLevel == LevelType.Medium)
        {
            var medium = manager as GameScene_Medium;
            askedNameImage.sprite = medium.GetAskedNameSprite();
            medium.ConfirmAskedName();
        }
        else if (currentLevel == LevelType.Hard)
        {
            var hard = manager as GameScene_Hard;
            askedNameImage.sprite = hard.GetAskedNameSprite();
            hard.ConfirmAskedName();
        }

        askedNameImage.preserveAspect = true;
    }

    private void SpawnNewCustomer()
    {
        if (manager == null) return;

        if (currentLevel == LevelType.Easy)
            (manager as GameScene_Easy).SpawnNewCustomer();
        else if (currentLevel == LevelType.Medium)
            (manager as GameScene_Medium).SpawnNewCustomer();
        else if (currentLevel == LevelType.Hard)
            (manager as GameScene_Hard).SpawnNewCustomer();
    }

    private bool IsGameOver() // ✅ Add this check for smoother scene switching
    {
        if (manager == null) return true;

        if (currentLevel == LevelType.Easy)
            return (manager as GameScene_Easy).IsGameOver();
        else if (currentLevel == LevelType.Medium)
            return (manager as GameScene_Medium).IsGameOver();
        else if (currentLevel == LevelType.Hard)
            return (manager as GameScene_Hard).IsGameOver();

        return true;
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
