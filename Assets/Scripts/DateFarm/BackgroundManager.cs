using UnityEngine;
using System.Collections.Generic;

public class BackgroundManager : MonoBehaviour
{
    public GameObject backgroundPrefab;
    public int tileCount = 5; // 👈 Enough to loop!
    public float scrollSpeed = 2f;

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

        bgWidth = backgroundPrefab.GetComponent<SpriteRenderer>().bounds.size.x;

        for (int i = 0; i < tileCount; i++)
        {
            Vector3 spawnPos = new Vector3(i * bgWidth, 0, 0);
            GameObject tile = Instantiate(backgroundPrefab, spawnPos, Quaternion.identity);
            tile.transform.SetParent(transform);
            tiles.Add(tile);
        }
    }

    void Update()
    {
        if (!gameManager.gameStarted) return;

        foreach (GameObject tile in tiles)
        {
            tile.transform.Translate(Vector2.left * scrollSpeed * Time.deltaTime);
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
}
