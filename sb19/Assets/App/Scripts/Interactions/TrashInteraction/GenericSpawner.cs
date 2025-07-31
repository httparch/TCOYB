using System.Collections.Generic;
using System;
using UnityEngine;
using System.Collections;

public class GenericSpawner : MonoBehaviour
{
    public SpawnableConfig config;
    public Transform spawnOrigin;
    public float spawnAreaWidth = 10f;
    public float spawnAreaHeight = 5f;

    private List<GameObject> spawnedEntities = new List<GameObject>();
    private bool wasSpawned = false;
    private bool saved = false;
    private void Start()
    {
          StartCoroutine(SpawnRoutine());
    }

    private IEnumerator SpawnRoutine()
    {
        yield return new WaitUntil(() =>
            CharacterManager.instance != null &&
            CharacterManager.instance.needsController != null
        );

        yield return new WaitUntil(() => UITree.HasPickedCharacter);

        DateTime lastSpawnedTime = CharacterManager.instance.needsController.lastTimeRoomCleaned;
        TimeSpan timeSinceLast = DateTime.Now - lastSpawnedTime;

        if (timeSinceLast >= config.respawnDelay)
        {
            Spawn();
            saved = false;
        }
        else
        {
            Debug.Log($"[{config.name.ToUpper()}] Not enough time passed. {timeSinceLast.TotalMinutes:F1}m < {config.respawnDelay.TotalMinutes}m");
        }
    }

    private void Update()
    {
        spawnedEntities.RemoveAll(e => e == null);

        if (wasSpawned && spawnedEntities.Count == 0 && !saved)
        {
            CharacterManager.instance.needsController.lastTimeRoomCleaned = DateTime.Now;
            DatabaseManager.instance.SaveCharacter(
                DatabaseManager.instance.CreateCharacterFromNeeds()
            );

            saved = true;
            Debug.Log($"[{config.name.ToUpper()}] All cleared and saved.");
        }
    }

    private void Spawn()
    {
        wasSpawned = true;

        for (int i = 0; i < config.spawnCount; i++)
        {
            Vector2 pos = new Vector2(
                UnityEngine.Random.Range(spawnOrigin.position.x - spawnAreaWidth / 2f, spawnOrigin.position.x + spawnAreaWidth / 2f),
                UnityEngine.Random.Range(spawnOrigin.position.y - spawnAreaHeight / 2f, spawnOrigin.position.y + spawnAreaHeight / 2f)
            );

            GameObject obj = Instantiate(config.prefab, pos, Quaternion.identity);

            // Assign random sprite if applicable
            SpriteRenderer sr = obj.GetComponent<SpriteRenderer>();
            if (sr != null && config.possibleSprites.Length > 0)
            {
                sr.sprite = config.possibleSprites[UnityEngine.Random.Range(0, config.possibleSprites.Length)];
            }

            spawnedEntities.Add(obj);
        }

        Debug.Log($"[{config.name.ToUpper()}] Spawned {config.spawnCount} objects.");
    }
}
