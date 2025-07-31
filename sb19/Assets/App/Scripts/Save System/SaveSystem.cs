using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Unity.VisualScripting;
using UnityEngine;

public class SaveSystem
{
    //private const string FileType = ".json";
    private string SavePath => Application.persistentDataPath + "/Saves/";
    private string BackUpSavePath => Application.persistentDataPath + "/BackUps/";

    private int SaveCount;
    public void SaveData<T> (T data, string filename)
    {
        Directory.CreateDirectory(SavePath);
        Directory.CreateDirectory(BackUpSavePath);
        Debug.Log("Save path: " + SavePath);
        Debug.Log("Backup save path: " + BackUpSavePath);
        Debug.Log("persistentDataPath: " + Application.persistentDataPath);
        Save(SavePath);
        SaveCount++;
        if(SaveCount % 5 == 0) Save(BackUpSavePath);

        void Save(string path)
        {
            try
            {
                using (StreamWriter writer = new StreamWriter(path + filename + Configs.FileType))
                {
                    BinaryFormatter formatter = new BinaryFormatter();
                    MemoryStream memoryStream = new MemoryStream();
                    formatter.Serialize(memoryStream, data);
                    string dataToSave = Convert.ToBase64String(memoryStream.ToArray());
                    writer.WriteLine(dataToSave);
                }
            }
            catch (Exception e)
            {
                Debug.LogError("Save failed: " + e.Message);
            }
        }
    }

    public T LoadData<T>(string filename)
    {
        Directory.CreateDirectory(SavePath);
        Directory.CreateDirectory(BackUpSavePath);
        Debug.Log("Load path: " + SavePath);
        Debug.Log("Load save path: " + BackUpSavePath);

        bool backUpNeeded = false;
        T dataToReturn;

        Load(SavePath);
        if (backUpNeeded) Load(BackUpSavePath);

        return dataToReturn;

        void Load(string path)
        {
            using(StreamReader reader = new StreamReader(path + filename + Configs.FileType))
            {
                BinaryFormatter formatter = new BinaryFormatter();
                string dataToLoad = reader.ReadToEnd();
                MemoryStream memoryStream = new MemoryStream(Convert.FromBase64String(dataToLoad));
                try
                {
                    dataToReturn = (T)formatter.Deserialize(memoryStream);
                }
                catch 
                {
                    backUpNeeded = true;
                    dataToReturn = default;
                }
            }
            
        }
    }

    public bool SaveExists(string filename) => File.Exists(BackUpSavePath + filename + Configs.FileType) || File.Exists(SavePath + filename + Configs.FileType);
}
