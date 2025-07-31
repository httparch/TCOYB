using System;
using UnityEngine;

public class DatabaseManager : MonoBehaviour
{

    //tomcomment
    private string currentBuddyName; 
    public static DatabaseManager instance;
    public SaveSystem database;
    public NeedsController needsController;

    private bool hasSavedThisCycle = false;
    private bool firstTime = false;
    private void Awake()
    {
        database = new SaveSystem();

        if (instance == null) instance = this;
        else Destroy(gameObject);
      
    }

    private void Update()
    {
        if (string.IsNullOrEmpty(currentBuddyName)) return;
       
            if (MoodManager.instance.gameHourTimer < 0 && !hasSavedThisCycle && UIManager.Instance.TreeDisplay.IsMainActive())
            {
                SaveCharacter(CreateCharacterFromNeeds());
                SetDataToNeedController();
                hasSavedThisCycle = true;
            }
            else if (MoodManager.instance.gameHourTimer >= 0) hasSavedThisCycle = false;

            if (UIManager.Instance.TreeDisplay.IsMainActive() && !firstTime)
            {
                Debug.Log("IM AT FALSE MAIN");
                SaveCharacter(CreateCharacterFromNeeds());
                SetDataToNeedController();
                firstTime = true;
            }
    }
    public void CheckExistingFileName()
    {
        if (string.IsNullOrEmpty(currentBuddyName)) return;

        string fileName = Configs._CharacterFileName + currentBuddyName.ToLower();
        if (database.SaveExists(fileName)) SetDataToNeedController();
        else return;
    }

    public void SetDataToNeedController()
    {
        
        //sets loaded data to needs controller
                Character character = LoadCharacter();
        character.name = currentBuddyName.ToUpper();
           if (character != null)

           {
               needsController.Initialize(
                   character.name,
                   character.food,
                   character.happiness,
                   character.energy,
                   Configs.foodTickRate,
                   Configs.happinessTickRate,
                   Configs.energyTickRate,
                   character.exp,
                   character.gold,
                   character.diamond,
                   character.level,
                   character.isSleeping,
                   DateTime.Parse(character.lastTimeFed),
                   DateTime.Parse(character.lastTimeHappy),
                   DateTime.Parse(character.lastTimeGainedEnergy),
                   DateTime.Parse(character.lastPerfectMoodRewardDate),
                   DateTime.Parse(character.lastTimeRoomCleaned)
               );

               if (character.isSleeping)
                {
                    UIManager.Instance.CharacterStatusDisplay.sleepingState();
                }
               CharacterManager.instance.needsController.UpdateAllStatusUI();
               CharacterManager.instance.evolutionController.SetCharacterSprites(character.name);
               Debug.LogWarning("[DBMANAGER TIME]" + character.lastTimeRoomCleaned);
           }
           else
           {
               Debug.LogWarning("[CHARACTER] is null");
           }    
    }
    public string GetCurrentBuddyName()
    {
        return currentBuddyName;
    }
    public void SetCurrentBuddyName(string buddyName)
    {
        if (string.IsNullOrEmpty(buddyName)) return;
        currentBuddyName = buddyName.ToLower();
        CheckExistingFileName();
    }

    public void SaveCharacter(Character character)
    {
        if (string.IsNullOrEmpty(currentBuddyName)) return;
        if (character == null) return;

        try
        {
           string fileName = Configs._CharacterFileName + currentBuddyName.ToLower(); // NEW
            database.SaveData(character, fileName);
        }
        catch (Exception e)
        {
            Debug.LogError("Save failed: " + e.Message);
        }
    }

    public Character LoadCharacter()
    {
        if (string.IsNullOrEmpty(currentBuddyName)) return null;
        Debug.LogWarning("[LOAD CHARACTER]" +currentBuddyName);
        string fileName = Configs._CharacterFileName + currentBuddyName.ToLower(); // NEW
        return database.LoadData<Character>(fileName);
    }

    public Character CreateCharacterFromNeeds()
    {
            if (needsController == null) return null;

            Character character = new Character(); // use default constructor
            character.Initialize(
                needsController.name,
                needsController.lastTimeFed,
                needsController.lastTimeHappy,
                needsController.lastTimeGainedEnergy,
                needsController.lastPerfectMoodRewardDate,
                needsController.lastTimeRoomCleaned,
                needsController.food,
                needsController.happiness,
                needsController.energy,
                needsController.exp,
                needsController.gold,
                needsController.diamond,
                needsController.level,
                needsController.isSleeping
            );
            return character;

    }
    public bool DoesCharacterFileExist(string fileName)
    {
        if (string.IsNullOrEmpty(fileName)) return false;

        return database.SaveExists(fileName);
    }

    public void ForceReinitialize()
    {
        SetCurrentBuddyName(currentBuddyName); // reuse logic

    }
    private void OnApplicationQuit()
    {
        SaveCharacter(CreateCharacterFromNeeds());
        Debug.Log("ONAPPQUIT");
    }
}
