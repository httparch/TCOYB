using UnityEngine;
using System;
using System.Collections.Generic;

public enum RewardState
{
    NotReady,
    Active,
    Claimed
}

public enum RewardType
{
    Money,
    Outfit,
    Decorative,
    Food
}

[Serializable]
public class Reward
{
    public string rewardName;
    public Sprite rewardImage;
    public RewardType type;
    public int dayNumber;
    public RewardState state = RewardState.NotReady;

    public int moneyAmount;

    public Furniture_SO decorativeItem;

}

[Serializable]
public class DailyRewardData
{
    public int currentDayIndex;
    public string lastClaimedDate;
    public List<RewardState> rewardStates = new List<RewardState>();
}
public class DailyReward : MonoBehaviour
{
    [Header("Configure in Inspector")]
    public List<Reward> rewards;
    

    private int currentDayIndex;
    private string lastClaimedDate;
    //private const string saveFileName = "daily_reward";
  
    void Start()
    {
        Refresh();
    }

    public void Refresh()
    {
        InitializeRewards();
        LoadProgress();
        UpdateRewardStates();
        CheckDailyRewardAvailability();

        FindObjectOfType<UIDailyReward>()?.PopulateDailyRewardsUI();
    }
    void InitializeRewards()
    {
        if (rewards.Count != Configs.totalDaysDailyReward)
        {
            rewards.Clear();
            for (int i = 0; i < Configs.totalDaysDailyReward; i++)
            {
                rewards.Add(new Reward
                {
                    rewardName = "Day " + (i + 1),
                    rewardImage = null, // Assign manually or through other metadata
                    dayNumber = i + 1,
                    state = RewardState.NotReady
                });
            }
        }
    }

    void LoadProgress()
    {
        if (DatabaseManager.instance.database.SaveExists(Configs._RewardFileName))
        {
            DailyRewardData data = DatabaseManager.instance.database.LoadData<DailyRewardData>(Configs._RewardFileName);
            currentDayIndex = data.currentDayIndex;
            lastClaimedDate = data.lastClaimedDate;

            for (int i = 0; i < rewards.Count; i++)
            {
                if (i < data.rewardStates.Count)
                    rewards[i].state = data.rewardStates[i];
                else
                    rewards[i].state = RewardState.NotReady;
            }
        }
        else
        {
            currentDayIndex = 0;
            lastClaimedDate = "";
        }
    }

    void SaveProgress()
    {
        DailyRewardData data = new DailyRewardData
        {
            currentDayIndex = currentDayIndex,
            lastClaimedDate = lastClaimedDate,
        };

        foreach (var reward in rewards)
        {
            data.rewardStates.Add(reward.state);
        }

        DatabaseManager.instance.database.SaveData(data, Configs._RewardFileName);
    }

    void CheckDailyRewardAvailability()
    {
        DateTime today = DateTime.Now.Date;
       // DateTime now = DateTime.Now;
        
        if (DateTime.TryParse(lastClaimedDate, out DateTime lastDate))
        {
            TimeSpan timeSinceLastClaim = today - lastDate;

            if (timeSinceLastClaim.TotalDays >= 1 && currentDayIndex < Configs.totalDaysDailyReward)
            {
                rewards[currentDayIndex].state = RewardState.Active;
            }
            else
            {
                Debug.Log("Reward already claimed today or not enough time has passed.");
            }
        }
        else
        {
            // First time user or no last claim
            if (currentDayIndex < Configs.totalDaysDailyReward)
            {
                rewards[currentDayIndex].state = RewardState.Active;
            }
        }
    }

    void UpdateRewardStates()
    {
        for (int i = 0; i < rewards.Count; i++)
        {
            if (i < currentDayIndex)
            {
                rewards[i].state = RewardState.Claimed;
            }
            else if (i == currentDayIndex && rewards[i].state == RewardState.Active)
            {
                // Keep Active if already validated by CheckDailyRewardAvailability
                continue;
            }
            else
            {
                rewards[i].state = RewardState.NotReady;
            }
        }
    }
    public void ClaimReward()
    {
        if (currentDayIndex >= Configs.totalDaysDailyReward)
        {
            Debug.Log("All rewards claimed!");
            return;
        }

        var reward = rewards[currentDayIndex];

        if (reward.state != RewardState.Active)
        {
            Debug.Log("Today's reward is not available yet.");
            return;
        }

        // Apply reward effects based on type
        switch (reward.type)
        {
            case RewardType.Money:
                //[TODO ] ADD METHOD FOR ADDING IT TO THE INVENTORY
                CharacterManager.instance.needsController.ChangeMoney(reward.moneyAmount);
                Debug.Log("Money reward claimed!" + reward.moneyAmount);
                break;

            case RewardType.Outfit:
                
                Debug.Log("Outfit reward claimed!");
                break;

            case RewardType.Decorative:
  
                Furniture claimedFurniture = FurnitureManager.Instance.FindFurnitureById(reward.decorativeItem.furniture_id, reward.decorativeItem.furniture_type);
                if (claimedFurniture != null)
                {
                    claimedFurniture.isOwned = true;
                    Debug.Log($"[DailyReward] Claimed furniture: {claimedFurniture.furniture_name}");

                    // Save updated furniture list
                    DatabaseManager.instance.database.SaveData(FurnitureManager.Instance.AllFurniture, Configs._FurnitureFilename);

                    // Refresh UI systems
                    UIManager.Instance.EditHomeDisplay.Refresh();
                    UIManager.Instance.ShopDisplay.Refresh();
                    Debug.Log("Decorative claimed!");
                }
                
                break;
            case RewardType.Food:
               
                Debug.Log("Food claimed!");
                break;
        }

        // Finalize claim
        reward.state = RewardState.Claimed;
        lastClaimedDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
        currentDayIndex++;

        SaveProgress();
        UpdateRewardStates();

        Debug.Log($"Reward claimed: {reward.rewardName} ({reward.type})");
    }

    //[todo] type of rewards //MONEY? OUTFIT? DECORATIVES? FOOD?
}
