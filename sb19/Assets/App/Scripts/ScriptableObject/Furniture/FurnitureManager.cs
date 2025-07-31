using System.Collections.Generic;
using UnityEditor.Experimental;
using UnityEngine;

public class FurnitureManager : MonoBehaviour
{
    
    public static FurnitureManager Instance;

    [SerializeField] private List<Furniture_SO> masterFurnitureList;
    //private const string _FurnitureFilename = "Furnitures";

    private List<Furniture> _allFurniture = new();
    public List<Furniture> AllFurniture => _allFurniture;

    private Dictionary<string, int> activeFurnitureDict = new();
    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        LoadOrInitializeFurnitures();
        Debug.Log("Loaded artifacts: " + _allFurniture.Count);
        foreach (var furniture in _allFurniture)
        {
            Debug.Log($"{furniture.furniture_id}: type = {furniture.furniture_type}");
        }
    }

    public void LoadOrInitializeFurnitures()
    {
        if (DatabaseManager.instance.database.SaveExists(Configs._FurnitureFilename))
            _allFurniture = DatabaseManager.instance.database.LoadData<List<Furniture>>(Configs._FurnitureFilename);
        else
        {
            _allFurniture = new List<Furniture>();
            foreach (var so in masterFurnitureList)
            {
                _allFurniture.Add(Furniture.FROMSO(so));
            }
        }

        activeFurnitureDict.Clear();
        foreach (var furniture in _allFurniture)
        {
            if (furniture.isActive)
                activeFurnitureDict[furniture.furniture_type] = furniture.furniture_id;
        }
    }
    public void SetActiveFurniture(string type, int id)
    {
        if (activeFurnitureDict.TryGetValue(type, out var currentActiveId) && currentActiveId == id)
        {
            Debug.LogWarning($"[FurnitureManager] {type} ID {id} is already active. Skipping.");
            return;
        }

        foreach (var item in _allFurniture)
        {
            if (item.furniture_type == type)
                item.isActive = (item.furniture_id == id);
        }
        activeFurnitureDict[type] = id;
        RoomManager.Instance.ApplyAllActiveFurniture(type);
        DatabaseManager.instance.database.SaveData(AllFurniture, Configs._FurnitureFilename);
    }
    public Furniture FindFurnitureById(int id, string type)
    {
        return AllFurniture.Find(f => f.furniture_id == id && f.furniture_type == type);
    }
    public bool IsFurnitureActive(Furniture furniture)
    {
        return activeFurnitureDict.TryGetValue(furniture.furniture_type, out var activeId)
            && activeId == furniture.furniture_id;
    }
    private void OnApplicationQuit()
    {
        DatabaseManager.instance.database.SaveData(_allFurniture, Configs._FurnitureFilename);
    }

}
