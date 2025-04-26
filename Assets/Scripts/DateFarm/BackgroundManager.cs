using UnityEngine;
using System.Collections.Generic;

public class BackgroundManager : MonoBehaviour
{
    public GameObject backgroundPrefab;
    public int tileCount = 5;

    private List<GameObject> tiles = new List<GameObject>();
    private float bgWidth;
    private MainGameplayScript gameManager;

    void Start()
    {
        gameManager = FindObjectOfType<MainGameplayScript>();

        if (!backgroundPrefab || !gameManager)
        {
            Debug.LogError("Missing backgroundPrefab or gameManager!");
            return;
        }

        // 🔧 Create a temp background just to calculate correct size
        GameObject tempBG = Instantiate(backgroundPrefab);
        AutoFitToCamera(tempBG); // 🧠 Scale the temp to match screen size
        bgWidth = tempBG.GetComponent<SpriteRenderer>().bounds.size.x;
        Destroy(tempBG);

        // 🧱 Spawn actual background tiles
        for (int i = 0; i < tileCount; i++)
        {
            Vector3 spawnPos = new Vector3(i * bgWidth, 0, 0);
            GameObject tile = Instantiate(backgroundPrefab, spawnPos, Quaternion.identity);
            AutoFitToCamera(tile); // Ensure each one scales to screen
            tile.transform.SetParent(transform);
            tiles.Add(tile);
        }
    }

    void Update()
    {
        if (!gameManager.gameStarted) return;

        foreach (GameObject tile in tiles)
        {
            tile.transform.Translate(Vector2.left * gameManager.scrollSpeed * Time.deltaTime);
        }

        GameObject first = tiles[0];
        if (first.transform.position.x <= -bgWidth)
        {
            GameObject last = tiles[tiles.Count - 1];
            first.transform.position = new Vector3(last.transform.position.x + bgWidth, 0, 0);
            tiles.RemoveAt(0);
            tiles.Add(first);
        }
    }

    // 🧠 Fit any background sprite to camera view horizontally
    void AutoFitToCamera(GameObject tile)
    {
        SpriteRenderer sr = tile.GetComponent<SpriteRenderer>();
        if (sr == null) return;

        float camHeight = Camera.main.orthographicSize * 2f;
        float camWidth = camHeight * Camera.main.aspect;

        Vector2 spriteSize = sr.sprite.bounds.size;
        tile.transform.localScale = new Vector3(
            camWidth / spriteSize.x,
            camHeight / spriteSize.y,
            1f
        );
    }
}
