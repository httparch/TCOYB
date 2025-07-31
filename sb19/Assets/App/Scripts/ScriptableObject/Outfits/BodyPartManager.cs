using System.Collections.Generic;
using UnityEngine;

public class BodyPartManager : MonoBehaviour
{
    public static BodyPartManager Instance;

    [SerializeField] private List<BodyPart_SO> masterBodyPartList;

    private List<BodyPart> _allBodyParts = new();
    public List<BodyPart> AllBodyParts => _allBodyParts;

    private Dictionary<BodyPartType, int> activeBodyParts = new();

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        LoadOrInitializeParts();
        Debug.Log("Loaded bodyparts: " + _allBodyParts.Count);
        foreach (var part in _allBodyParts)
        {
            Debug.Log($"{part.partType}: type = {part.skin_id}");
        }
    }

    public void LoadOrInitializeParts()
    {
        if (DatabaseManager.instance.database.SaveExists(Configs._BodyPartFilename))
            _allBodyParts = DatabaseManager.instance.database.LoadData<List<BodyPart>>(Configs._BodyPartFilename);
        else
        {
            _allBodyParts = new List<BodyPart>();
            foreach (var so in masterBodyPartList)
            {
                _allBodyParts.Add(BodyPart.FromSO(so));
            }
        }

        foreach (var p in _allBodyParts)
        {
            Debug.Log($"[BodyPartManager] Loaded: {p.partType}, ID: {p.skin_id}, Owned: {p.isOwned}");
        }


        activeBodyParts.Clear();
        foreach (var part in _allBodyParts)
        {
            if (part.isActive)
                activeBodyParts[part.partType] = part.skin_id;
        }
    }

    public void SetActivePart(BodyPartType partType, int id)
    {
        foreach (var part in _allBodyParts)
        {
            if (part.partType == partType)
            {
                part.isActive = (part.skin_id == id); // Only this ID stays true
            }
        }


        activeBodyParts[partType] = id;
        DatabaseManager.instance.database.SaveData(_allBodyParts, Configs._BodyPartFilename);
    }

    public BodyPart FindPartById(int id, BodyPartType type)
    {
        return _allBodyParts.Find(p => p.skin_id == id && p.partType == type);
    }

    public BodyPart GetActivePart(BodyPartType type)
    {
        if (activeBodyParts.TryGetValue(type, out int id))
            return FindPartById(id, type);
        return null;
    }

    private void OnApplicationQuit()
    {
        DatabaseManager.instance.database.SaveData(_allBodyParts, Configs._BodyPartFilename);
    }
}
