using System;
using System.Collections;
using UnityEngine;

public class NeedsController : MonoBehaviour
{
    public string name;
    public float food, happiness, energy;

    public int exp;
    public int gold;
    public int diamond;

    public float foodTickRate, happinessTickRate, energyTickRate;
    public DateTime lastTimeFed;
    public DateTime lastTimeHappy;
    public DateTime lastTimeGainedEnergy;
    public DateTime lastPerfectMoodRewardDate;
    public DateTime lastTimeRoomCleaned;

    private Coroutine sleepCoroutine;
    public bool isSleeping;

    public int level;

    private bool initialized = false;
    private void Start()
    {
        if (string.IsNullOrEmpty(name)) return;
    }

    
    public void Initialize(string name,
                            float food,
                            float happiness,
                            float energy,
                            float foodTickRate,
                            float happinessTickRate,
                            float energyTickRate,
                            int exp,
                            int gold,
                            int diamond,
                            int level,
                            bool isSleeping)
    {
        lastTimeFed = DateTime.Now;
        lastTimeHappy = DateTime.Now;
        lastTimeGainedEnergy = DateTime.Now;
        lastPerfectMoodRewardDate = DateTime.Now;
        lastTimeRoomCleaned = DateTime.Now;
        this.name = name;
        this.food = food;
        this.happiness = happiness;
        this.energy = energy;
        this.foodTickRate = foodTickRate;
        this.happinessTickRate = happinessTickRate;
        this.energyTickRate = energyTickRate;
        this.exp = exp;
        this.gold = gold;
        this.diamond = diamond;
        this.level = level;
        this.isSleeping = isSleeping;
        UpdateAllStatusUI();
        Debug.LogWarning("[NE CONTROLLER, 0]" + lastTimeRoomCleaned);
    }

    public void Initialize(
                            string name,
                           float food,
                           float happiness,
                           float energy,
                           float foodTickRate,
                           float happinessTickRate,
                           float energyTickRate,
                           int exp,
                           int gold,
                           int diamond,
                           int level,
                           bool isSleeping,
                           DateTime lastTimeFed,
                           DateTime lastTimeHappy,
                           DateTime lastTimeGainedEnergy,
                           DateTime lastPerfectMoodRewardDate,
                           DateTime lastTimeRoomCleaned)
    {
        this.lastTimeFed = lastTimeFed;
        this.lastTimeHappy = lastTimeHappy;
        this.lastTimeGainedEnergy = lastTimeGainedEnergy;
        this.lastPerfectMoodRewardDate = lastPerfectMoodRewardDate;
        this.lastTimeRoomCleaned = lastTimeRoomCleaned;
        this.name = name;
        this.food = food - foodTickRate * TickAmountSinceLastTimeToCurrentTime(lastTimeFed,MoodManager.instance.hourLength);
        this.happiness = happiness - happinessTickRate * TickAmountSinceLastTimeToCurrentTime(lastTimeHappy, MoodManager.instance.hourLength); ;
        //this.energy = energy - energyTickRate * TickAmountSinceLastTimeToCurrentTime(lastTimeGainedEnergy, MoodManager.instance.hourLength); ;
        this.foodTickRate = foodTickRate;
        this.happinessTickRate = happinessTickRate;
        this.energyTickRate = energyTickRate;
        this.exp = exp;
        this.gold = gold;
        this.diamond = diamond;
        this.level = level;
        this.isSleeping = isSleeping;

        int energyTicks = TickAmountSinceLastTimeToCurrentTime(lastTimeGainedEnergy, MoodManager.instance.hourLength);

        if (isSleeping)
        {
            this.energy = energy + energyTicks * energyTickRate;
            if (this.energy > Configs.MAX_ENERGY) this.energy = Configs.MAX_ENERGY;
        }
        else
        {
            this.energy = energy - energyTicks * energyTickRate;
            if (this.energy < 0) this.energy = 0;
        }

        if (this.food < 0) this.food = 0;
        if (this.happiness < 0) this.happiness = 0;
        //if (this.energy < 0) this.energy = 0;

        UpdateAllStatusUI();
        Debug.LogWarning("[NE CONTROLLER]" + lastTimeRoomCleaned);
        //[to do:] time passes 
    }
    private void Update()
    {
        if (!initialized)
        {
            if (!string.IsNullOrEmpty(name)) initialized = true;
            else return;

        }

        TryGivePerfectMoodReward();

        if (MoodManager.instance.gameHourTimer < 0)
        {
            
           // if (isSleeping)
        //    {
                //[TODO] Correct incrementation and decrementation of food and happiness when sleeping
          //      ChangeFood(-Mathf.Max(1, foodTickRate / 5));
        //        ChangeHappiness(+1);
        //    }
//else
            //{
                ChangeFood(-foodTickRate);
                ChangeHappiness(-happinessTickRate);
                ChangeEnergy(-energyTickRate);
           // }

        }
    }

    public int TickAmountSinceLastTimeToCurrentTime(DateTime lastTime, float tickRateInSeconds)
    {
        DateTime currentDateTime = DateTime.Now;
        int dayOfYearDifference = currentDateTime.DayOfYear - lastTime.DayOfYear;

        if (currentDateTime.Year > lastTime.Year || dayOfYearDifference >= 7) return 1500; //return tick that pet is dead

        int dayDifferenceSecondsAmount = dayOfYearDifference * 86400;
        if (dayOfYearDifference > 0) return Mathf.RoundToInt(dayDifferenceSecondsAmount/tickRateInSeconds);

        int hourDifferenceSecondsAmount = (currentDateTime.Hour - lastTime.Hour) * 3600;
        if (hourDifferenceSecondsAmount > 0) return Mathf.RoundToInt(hourDifferenceSecondsAmount/ tickRateInSeconds);

        int minuteDifferenceSecondsAmount = (currentDateTime.Minute - lastTime.Minute) * 60;
        if (minuteDifferenceSecondsAmount > 0) return Mathf.RoundToInt(minuteDifferenceSecondsAmount/ tickRateInSeconds);

        int secondDifferenceAmount = (currentDateTime.Second - lastTime.Second);
        return Mathf.RoundToInt(secondDifferenceAmount/ tickRateInSeconds);
    }

    public void UpdateAllStatusUI()
    {
        if (UIManager.Instance == null) return;

        UIManager.Instance.CharacterStatusDisplay.UpdateStatusDisplay(this.food, this.happiness, this.energy, exp, level);
        UIManager.Instance.CharacterStatusDisplay.UpdateNameDisplay(name);
        UIManager.Instance.CurrencyDisplay.UpdateCurrencyDisplay(gold, diamond);
        CharacterManager.instance.evolutionController.SetCharacterSprites(name);
        //UIManager.Instance.TreeDisplay.SetMainActive();
    }

    public void ChangeFood(float amount)
    {
        food += amount;

        if(amount > 0) lastTimeFed = DateTime.Now;

        if (food < 0)
        {
            food = 0;
            CharacterManager.instance.Die();
        }
        else if (food > Configs.MAX_FOOD) food = Configs.MAX_FOOD;

        UpdateAllStatusUI();
    }

    public void ChangeMoney(int amount)
    {
        gold += amount;
        UpdateAllStatusUI();
    }

    public void ChangeHappiness(float amount)
    {
        happiness += amount;
        //[TODO:] exp computation based on happiness amount
        
        if (amount > 0)
        {    
            if (exp > Configs.MAX_EXP_CAP)
            {
                level += 1;
                exp = exp - Configs.MAX_EXP_CAP;
                //add effect evolution
                CharacterManager.instance.evolutionController.ChangeCharacterSprite(level);
                 
            }
            //exp += amount / 2;
            exp += Configs.EXP_GAIN;
            lastTimeHappy = DateTime.Now;
            
        }

        if (happiness < 0)
        {
            happiness = 0;
            CharacterManager.instance.Die();
        }
        else if (happiness > Configs.MAX_HAPPINESS) happiness = Configs.MAX_HAPPINESS;

        UpdateAllStatusUI();
    }
    public void ChangeEnergy(float amount)
    {
        energy += amount;

        if (amount > 0) lastTimeGainedEnergy = DateTime.Now;

        if (energy < 0)
        {
            energy = 0;
            CharacterManager.instance.Die();
        }
        else if (energy > Configs.MAX_ENERGY) energy = Configs.MAX_ENERGY;

        UpdateAllStatusUI();

    }

    public void Sleep()
    {
        //[TODO] Compute energy when character ask for sleep
        
        if (isSleeping && sleepCoroutine == null)
        {
            sleepCoroutine = StartCoroutine(SleepRoutine());
            return;
        }

        if (!isSleeping && energy < Configs.MAX_ENERGY)
        {
            isSleeping = true;
            sleepCoroutine = StartCoroutine(SleepRoutine());
        }
        else if (isSleeping)
        {
            if (sleepCoroutine != null) StopCoroutine(sleepCoroutine);

            sleepCoroutine = null;
            isSleeping = false;
        }

    }

    private IEnumerator SleepRoutine()
    {
        while (Mathf.FloorToInt(energy) < Configs.MAX_ENERGY)
        {
            ChangeEnergy(1);
            yield return new WaitForSeconds(5f);
        }
        UIManager.Instance.CharacterStatusDisplay.awakeState();
        energy = Configs.MAX_ENERGY;
        isSleeping = false;
        sleepCoroutine = null;
    }

    private void TryGivePerfectMoodReward()
    {
        bool isMoodPerfect = Mathf.FloorToInt(happiness) >= Configs.MAX_HAPPINESS &&
                             Mathf.FloorToInt(energy) >= Configs.MAX_ENERGY;

        bool rewardedToday = lastPerfectMoodRewardDate.Date == DateTime.Now.Date;

        if (isMoodPerfect && !rewardedToday)
        {
            ChangeMoney(100);
            lastPerfectMoodRewardDate = DateTime.Now;

            Debug.Log("[Daily Reward] +100 gold granted for perfect energy and happiness.");
        }
    }

}
