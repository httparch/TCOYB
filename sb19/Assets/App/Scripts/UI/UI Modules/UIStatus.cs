
using UnityEngine;
using UnityEngine.UIElements;
public class UIStatus : MonoBehaviour
{

    public GameObject UI;
    public GameObject MiniGame;

    public GameObject LoungeRoom;
    public GameObject Bedroom;
    public GameObject OutfitRoom;

    private VisualElement HungerBar;
    private VisualElement HappinessBar;
    private VisualElement EnergyBar;

    private Label ExpText, LevelText;

    private VisualElement ExpBar;
    private VisualElement Hunger, Happiness, Energy;

    private VisualElement Phone;

    public VisualElement DimLight;

    public VisualElement HideDisplayButton;

    public Label CharacterName;
    public void Initialize(UIDocument doc)
    {
        Hunger = doc.rootVisualElement.Q<VisualElement>("Hunger");
        Happiness = doc.rootVisualElement.Q<VisualElement>("Happiness");
        Energy = doc.rootVisualElement.Q<VisualElement>("Energy");

        ExpBar = doc.rootVisualElement.Q<VisualElement>("ExpBar");

        HungerBar = doc.rootVisualElement.Q<VisualElement>("HungerBar");
        HappinessBar = doc.rootVisualElement.Q<VisualElement>("HappinessBar");
        EnergyBar = doc.rootVisualElement.Q<VisualElement>("EnergyBar");

        Phone = doc.rootVisualElement.Q<VisualElement>("Phone");

        ExpText = doc.rootVisualElement.Q<Label>("ExpText");
        LevelText = doc.rootVisualElement.Q<Label>("LevelText");

        DimLight = doc.rootVisualElement.Q<VisualElement>("Dim");

        CharacterName = doc.rootVisualElement.Q<Label>("CharacterName");

        HideDisplayButton = doc.rootVisualElement.Q<VisualElement>("HideDisplayButton");

        Debug.Log("WORKING!");

        HideDisplayButton.RegisterCallback<ClickEvent>(OnButtonClicked);
        Hunger.RegisterCallback<ClickEvent>(OnHungerClicked);
        Happiness.RegisterCallback<ClickEvent>(OnHappinessClicked);
        Energy.RegisterCallback<ClickEvent>(OnEnergyClicked);
    }
    
    private void OnButtonClicked(ClickEvent evt)
    {
        var app = UIManager.Instance.TreeDisplay.App;
        if (app.style.display == DisplayStyle.None) app.style.display = DisplayStyle.Flex;
        else app.style.display = DisplayStyle.None;
   
    }

    public void UpdateNameDisplay(string name)
    {
        if (name == null) return;
        CharacterName.text = name;
    }
    public void UpdateStatusDisplay(float food, float happiness, float energy, int exp, int level)
    {
        if (ExpBar == null) return;
        else
        {
            
            if (level >= Configs.MAX_LEVEL) {
                LevelText.text = "MAX";
                ExpText.text = "MAX";
                ExpBar.style.width = Length.Percent(100);
            }
            else if(level < Configs.MAX_LEVEL)
            {
                float maxValue = Configs.MAX_EXP_CAP;
                float barPercentage = Mathf.Clamp01(exp / maxValue);
                ExpBar.style.width = Length.Percent(100 * barPercentage);
                ExpText.text = exp.ToString();
                LevelText.text = "Level: " + level;
            }
            
            
        }

        HungerBar.style.height = Length.Percent(GetBarPercentage(food) * 100f);
        HungerBar.MarkDirtyRepaint();

        HappinessBar.style.height = Length.Percent(GetBarPercentage(happiness) * 100f);
        HappinessBar.MarkDirtyRepaint();

        EnergyBar.style.height = Length.Percent(GetBarPercentage(energy) * 100f);
        EnergyBar.MarkDirtyRepaint();

        Debug.Log("[HAPPINESS]" + happiness);
        Debug.Log("[ENERGY]" + energy);
        Debug.Log("[EXP]" + exp);
    }
    // create method update images passing (food,happiness,energer)
    public float GetBarPercentage(float amount)
    {
        float maxValue = 100f;
        return Mathf.Clamp01(amount / maxValue);
    }

    public void OnHungerClicked(ClickEvent evt)
    {
        CharacterManager.instance.needsController.ChangeFood(20);
        CharacterManager.instance.needsController.ChangeHappiness(5);
        CharacterManager.instance.needsController.ChangeEnergy(1);
    }

    public void sleepingState()
    {
        LoungeRoom.SetActive(false);
        Bedroom.SetActive(true);
        OutfitRoom.SetActive(false);
        DimLight.AddToClassList("dim-light");
    }

    public void outfittingState()
    {
        LoungeRoom.SetActive(false);
        Bedroom.SetActive(false);
        OutfitRoom.SetActive(true);
    }
    public void awakeState()
    {
        LoungeRoom.SetActive(true);
        Bedroom.SetActive(false);
        OutfitRoom.SetActive(false);
        DimLight.RemoveFromClassList("dim-light");
    }

    public void OnEnergyClicked(ClickEvent evt)
    {
        CharacterManager.instance.needsController.Sleep();
        if (CharacterManager.instance.needsController.isSleeping)
        {
            sleepingState();
        }
        else
        {
            awakeState();
        }
    }
    
    public void OnHappinessClicked(ClickEvent evt)
    {
        if (Phone.ClassListContains("default-phone-out")) Phone.RemoveFromClassList("default-phone-out");
        else Phone.AddToClassList("default-phone-out");

    }
}
