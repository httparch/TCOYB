using UnityEngine;

public static class Configs 
{
    //----------filename-------------//
    public const string _FurnitureFilename = "Furnitures";
    public const string _RewardFileName = "daily_reward";
    public const string _CharacterFileName = "character_";
    public const string _BodyPartFilename = "BodyParts";
    public const string FileType = ".json";
    //----------values--------------//
    public static int MAX_FOOD = 100;
    public static int MAX_HAPPINESS = 100;
    public static int MAX_ENERGY = 100;
    public static int MAX_LEVEL = 30;

    public static int MAX_EXP_CAP = 1000;
    public static int EXP_GAIN = 500;

    public static float foodTickRate = 0.8f;
    public static float happinessTickRate = 0.45f;
    public static float energyTickRate = 0.30f;

    public const int totalDaysDailyReward = 4;

    //----------furniture types--------------//
    public static string _chair = "Chair";
    public static string _plants = "Plants";
    public static string _window = "Window";
    public static string _wallDecoration = "WallDecoration";
    public static string _wallpaper = "Wallpaper";

}
