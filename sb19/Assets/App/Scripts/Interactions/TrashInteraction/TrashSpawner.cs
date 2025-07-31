using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;

public class TrashSpawner : MonoBehaviour
{
    public GameObject messPrefab;
    public int messCount = 5;
    public float spawnAreaWidth = 10f;
    public float spawnAreaHeight = 5f;
    public Transform spawnOrigin;
    public Sprite[] messSprites;

    private List<GameObject> currentTrash = new List<GameObject>();
    //private TimeSpan respawnDelay = TimeSpan.FromHours(4);
    private TimeSpan respawnDelay = TimeSpan.FromMinutes(3);
    private bool cleanedAndSaved = false;
    private bool messWasSpawned = false;
    private void Start()
    {
        StartCoroutine(WaitForNeedsDataAndSpawn());
    }

    private IEnumerator WaitForNeedsDataAndSpawn()
    {
        while (CharacterManager.instance == null ||
               CharacterManager.instance.needsController == null ||
               CharacterManager.instance.needsController.lastTimeRoomCleaned == DateTime.MinValue)
        {
            yield return null;
        }

        TimeSpan timeSinceCleaned = DateTime.Now - CharacterManager.instance.needsController.lastTimeRoomCleaned;

        if (timeSinceCleaned >= respawnDelay)
        {
            SpawnMesses();
            cleanedAndSaved = false;
        }
    }

    private void Update()
    {
        currentTrash.RemoveAll(t => t == null);

        if (messWasSpawned && currentTrash.Count == 0 && !cleanedAndSaved)
        {
            CharacterManager.instance.needsController.lastTimeRoomCleaned = DateTime.Now;

            DatabaseManager.instance.SaveCharacter(
                DatabaseManager.instance.CreateCharacterFromNeeds()
            );

            cleanedAndSaved = true;
        }
    }

    private void SpawnMesses()
    {
        messWasSpawned = true;
        for (int i = 0; i < messCount; i++)
        {
            Vector2 randomPos = new Vector2(
                UnityEngine.Random.Range(spawnOrigin.position.x - spawnAreaWidth / 2f, spawnOrigin.position.x + spawnAreaWidth / 2f),
                UnityEngine.Random.Range(spawnOrigin.position.y - spawnAreaHeight / 2f, spawnOrigin.position.y + spawnAreaHeight / 2f)
            );

            GameObject newTrash = Instantiate(messPrefab, randomPos, Quaternion.identity);
            SpriteRenderer renderer = newTrash.GetComponent<SpriteRenderer>();
            if (renderer != null && messSprites.Length > 0)
            {
                Sprite randomSprite = messSprites[UnityEngine.Random.Range(0, messSprites.Length)];
                renderer.sprite = randomSprite;
            }

            currentTrash.Add(newTrash);
        }
    }
}
