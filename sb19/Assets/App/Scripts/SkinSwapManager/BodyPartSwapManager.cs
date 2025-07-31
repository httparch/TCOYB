using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using static UIOutfitManager;

[System.Serializable]
public class BodyPartImageMapping
{
    public int skinId;
    public BodyPartType partType;
    public Sprite sprite;
    public string partName; // optional for UI label
    public string outfitFor;
}
    public enum BodyPartType
{
    Head,
    Torso,
    LeftArm,
    RightArm,
    LeftFoot,
    RightFoot
}
public class BodyPartSwapManager : MonoBehaviour
{
    public static BodyPartSwapManager Instance;

    [Header("Assign All Sprites")]
    [SerializeField] private List<BodyPartImageMapping> headSprites;
    [SerializeField] private List<BodyPartImageMapping> torsoSprites;
    [SerializeField] private List<BodyPartImageMapping> leftArmSprites;
    [SerializeField] private List<BodyPartImageMapping> rightArmSprites;
    [SerializeField] private List<BodyPartImageMapping> leftFootSprites;
    [SerializeField] private List<BodyPartImageMapping> rightFootSprites;

    private Dictionary<string, Sprite> bodyPartSpriteDict = new();
    private Dictionary<BodyPartType, List<BodyPartImageMapping>> bodyPartLibrary = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        BuildSpriteLookup();
    }

    private void BuildSpriteLookup()
    {
        // Combine all lists for iteration
        List<BodyPartImageMapping>[] allLists = {
            headSprites, torsoSprites, leftArmSprites,
            rightArmSprites, leftFootSprites, rightFootSprites
        };

        foreach (var list in allLists)
        {
            foreach (var part in list)
            {
                string key = GetKey(part.skinId, part.partType);

                if (!bodyPartSpriteDict.ContainsKey(key))
                    bodyPartSpriteDict.Add(key, part.sprite);

                if (!bodyPartLibrary.ContainsKey(part.partType))
                    bodyPartLibrary[part.partType] = new List<BodyPartImageMapping>();

                bodyPartLibrary[part.partType].Add(part);
            }
        }

        // 🔽 Sort each body part type list by skinId (optional but useful)
        foreach (var type in bodyPartLibrary.Keys.ToList())
        {
            bodyPartLibrary[type] = bodyPartLibrary[type]
                .OrderBy(part => part.skinId)
                .ToList();
        }

        foreach (var kvp in bodyPartLibrary)
        {
            Debug.Log($"Loaded {kvp.Key} with {kvp.Value.Count} sprites.");
        }
    }

    private string GetKey(int id, BodyPartType type) => $"{type}_{id}";

    public Sprite GetSprite(BodyPartType type, int id)
    {
        string key = GetKey(id, type);
        bodyPartSpriteDict.TryGetValue(key, out var sprite);
        return sprite;
    }
    
    public List<BodyPartImageMapping> GetAllPartsOfType(BodyPartType type)
    {
        if (bodyPartLibrary.TryGetValue(type, out var list))
            return list;
        return new List<BodyPartImageMapping>();
    }

    public List<BodyPartImageMapping> GetAllPartsSorted()
    {
        // Combine all types into one sorted list by partType then skinId
        var all = new List<BodyPartImageMapping>();
        foreach (var kvp in bodyPartLibrary)
        {
            all.AddRange(kvp.Value);
        }
        return all
            .OrderBy(part => part.partType)
            .ThenBy(part => part.skinId)
            .ToList();
    }
}
