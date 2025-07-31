using System.Collections.Generic;
using UnityEngine;

public class RoomManager : MonoBehaviour
{
    public UIStatus StatusDisplay;
    public GameObject Items;

    public static RoomManager Instance;

    [SerializeField] private SpriteRenderer furnitureRenderer;

    [SerializeField] private SpriteRenderer windowRenderer; 

    [SerializeField] private SpriteRenderer wallDecorRenderer;

    [SerializeField] private SpriteRenderer otherStuffRenderer;

    [SerializeField] private SpriteRenderer backgroundRenderer;

    //[SerializeField] private Sprite bedroomSprite;

    [System.Serializable]
    public class FurnitureImageMapping
    {
        public string furnitureId;
        public string type;
        public Sprite sprite;
        public string furnitureName;
        public Sprite image;
    }
    private Dictionary<string, Sprite> furnitureImageDict = new();
    private Dictionary<string, List<FurnitureImageMapping>> furnitureByType = new();

    [SerializeField] private List<FurnitureImageMapping> wallpaperImages;
    [SerializeField] private List<FurnitureImageMapping> chairImages;
    [SerializeField] private List<FurnitureImageMapping> windowImages;
    [SerializeField] private List<FurnitureImageMapping> decorImages;
    [SerializeField] private List<FurnitureImageMapping> plantImages;

    [SerializeField] float yOffset = 0f;
    private bool isEditting = false;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);

        List<FurnitureImageMapping>[] allLists = {
        wallpaperImages, chairImages, windowImages, decorImages, plantImages
    };

        foreach (var list in allLists)
        {
            foreach (var mapping in list)
            {
                string key = GetKey(mapping.furnitureId, mapping.type);

                if (!furnitureImageDict.ContainsKey(key))
                    furnitureImageDict.Add(key, mapping.sprite);

                if (!furnitureByType.ContainsKey(mapping.type))
                    furnitureByType[mapping.type] = new List<FurnitureImageMapping>();

                furnitureByType[mapping.type].Add(mapping);
            }
        }
    }

    private string GetKey(string id, string type)
    {
        return type + "_" + id;
    }
    public void ApplyAllActiveFurniture(string type)
    {
        switch (type)
        {
            case "Chair": ApplyFurniture(Configs._chair, furnitureRenderer); break;
            case "Plants": ApplyFurniture(Configs._plants, otherStuffRenderer); break;
            case "Window": ApplyFurniture(Configs._window, windowRenderer); break;
            case "WallDecoration": ApplyFurniture(Configs._wallDecoration, wallDecorRenderer); break;
            case "Wallpaper": ApplyFurniture(Configs._wallpaper, backgroundRenderer); break;
        }
    }

    public void ApplyAllActiveFurniture()
    {
       ApplyFurniture(Configs._chair, furnitureRenderer);
       ApplyFurniture(Configs._plants, otherStuffRenderer);
       ApplyFurniture(Configs._window, windowRenderer);
       ApplyFurniture(Configs._wallDecoration, wallDecorRenderer); 
       ApplyFurniture(Configs._wallpaper, backgroundRenderer); 
        
    }

    private void ApplyFurniture(string type, SpriteRenderer targetRenderer)
    {
        var item = FurnitureManager.Instance.AllFurniture
            .Find(f => f.furniture_type == type && f.isActive);
        
            if (item != null)
            {
                string key = GetKey(item.furniture_id.ToString(), item.furniture_type);
                if (furnitureImageDict.TryGetValue(key, out Sprite sprite)) targetRenderer.sprite = sprite;
                else Debug.LogWarning($"No sprite found for key: {key}");
            }
    }

    public List<FurnitureImageMapping> GetAllFurnitureImages()
    {
        var all = new List<FurnitureImageMapping>();
        all.AddRange(wallpaperImages);
        all.AddRange(chairImages);
        all.AddRange(windowImages);
        all.AddRange(decorImages);
        all.AddRange(plantImages);
        return all;
    }

}
