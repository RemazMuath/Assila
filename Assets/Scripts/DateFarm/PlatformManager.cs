﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;
using System.Linq;
using ArabicSupport;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PlatformManager : MonoBehaviour
{
    public GameObject[] platformPrefabs;
    public int poolSize = 100;
    public float spawnY = -2.5f;
    public float minSpacing = 5f;
    public float maxSpacing = 7f;
    public float heightChangeThreshold = 10f;

    [SerializeField] private Transform player;

    private Queue<GameObject> platformPool = new Queue<GameObject>();
    private float nextSpawnX = 0f;
    private float scrollSpeed;
    private bool gameStarted = false;
    private MainGameplayScript game;
    private GameObject startBlockInstance;

    private float lastPlatformHeight;
    private float lastPlatformRightEdge;

    private const float DeactivateThresholdX = -20f;
    private const float SpawnAheadDistance = 30f;

    void Start()
    {
        startBlockInstance = GameObject.Find("StartBlock");

        game = FindAnyObjectByType<MainGameplayScript>();
        scrollSpeed = game.scrollSpeed;

        for (int i = 0; i < poolSize; i++)
        {
            var obj = Instantiate(platformPrefabs[Random.Range(0, platformPrefabs.Length)]);
            obj.SetActive(false);
            obj.tag = "Platform1";
            platformPool.Enqueue(obj);
        }

        Invoke(nameof(StartGame), 4f);
    }

    void StartGame()
    {
        gameStarted = true;

        for (int i = 0; i < poolSize; i++)
            SpawnPlatform();
    }

    private void MoveStartBlock()
    {
        if (startBlockInstance == null) return;

        startBlockInstance.transform.Translate(Vector2.left * scrollSpeed * Time.deltaTime);

        if (startBlockInstance.transform.position.x + GetHalfWidth(startBlockInstance) < DeactivateThresholdX)
        {
            Destroy(startBlockInstance);
            startBlockInstance = null;
        }
    }
    private float GetHalfWidth(GameObject obj)
    {
        var renderer = obj.GetComponent<SpriteRenderer>();
        if (renderer != null)
        {
            return renderer.bounds.size.x / 2f;
        }
        return 0f;
    }

    void Update()
    {
        if (!gameStarted || game == null) return;

        scrollSpeed = game.scrollSpeed;
        MoveStartBlock();

        bool anyPlatformsActive = false;

        foreach (var platform in platformPool)
        {
            if (platform.activeInHierarchy)
            {
                anyPlatformsActive = true;
                platform.transform.Translate(Vector2.left * scrollSpeed * Time.deltaTime);

                if (platform.transform.position.x < DeactivateThresholdX)
                {
                    platform.SetActive(false);
                }
            }
        }
        if (!anyPlatformsActive)
        {
            Debug.LogWarning("No active platforms - forcing spawn");
            SpawnPlatform();
        }

        float spawnBuffer = Mathf.Max(SpawnAheadDistance, Camera.main.orthographicSize * 2f);
        while (nextSpawnX < player.position.x + spawnBuffer)
        {
            SpawnPlatform();
        }
    }

    void SpawnPlatform()
    {
        Debug.Log("Spawning platform...");

        var platform = GetInactivePlatform();
        if (platform == null)
        {
            Debug.LogError("Failed to get platform from pool");
            return;
        }

        platform.SetActive(true);

        float width = platform.GetComponent<SpriteRenderer>().bounds.size.x;
        float gap = Random.Range(minSpacing, maxSpacing);

        // Initialize lastPlatformRightEdge if this is the first platform
        if (lastPlatformRightEdge == 0f && !platformPool.Any(p => p.activeInHierarchy))
        {
            lastPlatformRightEdge = Camera.main.transform.position.x - (width * 2f);
        }

        float spawnPosX = lastPlatformRightEdge + gap + (width / 2f);
        float chosenHeight = ChooseHeight();

        platform.transform.position = new Vector3(spawnPosX, chosenHeight, 0f);

        // Update tracking variables
        lastPlatformRightEdge = spawnPosX + (width / 2f);
        nextSpawnX = lastPlatformRightEdge;
        lastPlatformHeight = chosenHeight;

        Debug.Log($"Spawned platform at X:{spawnPosX}, next spawn at:{nextSpawnX}");
    }

    GameObject GetInactivePlatform()
    {
        foreach (var platform in platformPool)
        {
            if (!platform.activeInHierarchy)
                return platform;
        }

        // If none are available, create a new one dynamically
        var newPlatform = Instantiate(platformPrefabs[UnityEngine.Random.Range(0, platformPrefabs.Length)]);
        newPlatform.SetActive(false);
        newPlatform.tag = "Platform1";
        platformPool.Enqueue(newPlatform); // add to the pool
        Debug.LogWarning("Platform pool expanded dynamically.");
        return newPlatform;
    }

    float ChooseHeight()
    {
        if (Random.value > 0.5f)
            return spawnY;
        else
            return spawnY + 1.5f;
    }
}