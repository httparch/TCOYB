using System;
using UnityEngine;

[System.Serializable]
public class SpawnableConfig
{
    public string name;
    public GameObject prefab;
    public Sprite[] possibleSprites;
    public int spawnCount = 5;
    public TimeSpan respawnDelay = TimeSpan.FromHours(4);
}