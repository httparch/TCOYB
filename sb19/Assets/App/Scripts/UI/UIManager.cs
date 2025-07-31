
using System.Collections;
using UnityEngine;
using UnityEngine.UIElements;
public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    public UIStatus CharacterStatusDisplay;
    public UICurrency CurrencyDisplay;
    public UISettings SettingDisplay;
    public UICharacterBag CharacterBagDisplay;
    public UIShop ShopDisplay;
    public UIEditHome EditHomeDisplay;
    public UIDailyReward DailyRewardDisplay;

    public ScreenShotManager CameraShotDisplay;
    public UITree TreeDisplay;

    public UIOutfitter OutfitDisplay;
    public UIOutfitManager OutfitManagerDisplay;
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
       // DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        UIDocument UI = GetComponent<UIDocument>();
        Debug.LogWarning("[UIManager - START]");
        CharacterStatusDisplay.Initialize(UI);
        CurrencyDisplay.Initialize(UI);
        SettingDisplay.Initialize(UI);
        CharacterBagDisplay.Initialize(UI);

        EditHomeDisplay.Initialize(UI);
        ShopDisplay.Initialize(UI);
        //[CharacterManager]
        CameraShotDisplay.Initialize(UI);
        TreeDisplay.Initialize(UI);
        DailyRewardDisplay.Initialize(UI);
        OutfitDisplay.Initialize(UI);
        OutfitManagerDisplay.initialize(UI);
    }

    public void Restart()
    {

    }

}


