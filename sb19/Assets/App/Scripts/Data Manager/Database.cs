
using System.IO;
using UnityEngine;

public class Database
{
    private string path = Application.persistentDataPath + "/Saves/";

    public void SaveData<T>(string saveName, T saveData)
    {
        if (!Directory.Exists(path)) Directory.CreateDirectory(path);

        string jsonToSave = JsonUtility.ToJson(saveData);
        File.WriteAllText(path + saveName + ".json", jsonToSave);
    }

    public T LoadData<T>(string saveName)
    { 
        string fullPath = path + saveName + ".json";

        if (File.Exists(fullPath))
        {
            string loadedJson = File.ReadAllText(fullPath);
            return JsonUtility.FromJson<T>(loadedJson);
        }
        else return default;
    }

}
