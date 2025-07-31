using System;

[Serializable]
public class Character 
{
    public string name;

    public string lastTimeFed;
    public string lastTimeHappy;
    public string lastTimeGainedEnergy;
    public string lastPerfectMoodRewardDate;
    public string lastTimeRoomCleaned;

    public float food, happiness, energy;

    public int exp;
    public int gold;
    public int diamond;

    public bool isSleeping;

    public int level;
    public void Initialize(string name, DateTime lastFed, DateTime lastHappy, DateTime lastEnergy, DateTime lastPerfectMood, DateTime lastTimeCleaned,
                       float food, float happiness, float energy, int exp, int gold,
                       int diamond, int level, bool sleeping)
    {

        this.name = name;

        lastTimeFed = lastFed.ToString("o");
        lastTimeHappy = lastHappy.ToString("o");
        lastTimeGainedEnergy = lastEnergy.ToString("o");
        lastPerfectMoodRewardDate = lastPerfectMood.ToString("o");
        lastTimeRoomCleaned = lastTimeCleaned.ToString("o");

        this.food = food;
        this.happiness = happiness;
        this.energy = energy;
        this.exp = exp;
        this.gold = gold;
        this.diamond = diamond;
        this.level = level;
        this.isSleeping = sleeping;
    }

    public DateTime GetLastTimeFed() => DateTime.Parse(lastTimeFed);
    public DateTime GetLastTimeHappy() => DateTime.Parse(lastTimeHappy);
    public DateTime GetLastTimeGainedEnergy() => DateTime.Parse(lastTimeGainedEnergy);
    public DateTime GetLastTimePerfectMood() => DateTime.Parse(lastPerfectMoodRewardDate);

    public DateTime GetLastTimeRoomCleaned() => DateTime.Parse(lastTimeRoomCleaned);

}
