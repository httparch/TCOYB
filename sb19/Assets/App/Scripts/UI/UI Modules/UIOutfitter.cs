using UnityEngine;
using UnityEngine.UIElements;

public class UIOutfitter : MonoBehaviour
{
    private VisualElement App, DailyRewardButton, HideDisplayButton, Outfitter, OutfitterPanel;
    private Button SaveButton, OutfitShopButton;
    //public GameObject OutfitRoom;
    public void Initialize(UIDocument doc)
    {
        App = doc.rootVisualElement.Q<VisualElement>("App");
        DailyRewardButton = doc.rootVisualElement.Q<VisualElement>("DailyRewardButton");
        HideDisplayButton = doc.rootVisualElement.Q<VisualElement>("HideDisplayButton");
        Outfitter = doc.rootVisualElement.Q<VisualElement>("Outfitter");
        OutfitterPanel = doc.rootVisualElement.Q<VisualElement>("OutfitterPanel");
        SaveButton = doc.rootVisualElement.Q<Button>("SaveButton");
        OutfitShopButton = doc.rootVisualElement.Q<Button>("OutfitShopButton");

        Outfitter.RegisterCallback<ClickEvent>(OnOutfitterClicked);
        OutfitShopButton.RegisterCallback<ClickEvent>(OnOutfitButtonClicked);
        SaveButton.RegisterCallback<ClickEvent>(OnSaveButtonClicked);

        int level = CharacterManager.instance.needsController.level;

        if (level < 30)
        {
            // Disable interaction
            Outfitter.pickingMode = PickingMode.Ignore;
            Outfitter.style.opacity = 0.5f;
            Outfitter.tooltip = "Unlocks at Level 30";
        }
    }

    public void enableOutfitShop()
    {
        Outfitter.pickingMode = PickingMode.Position; 
        Outfitter.style.opacity = 1f;
        Outfitter.tooltip = null;
        Outfitter.RegisterCallback<ClickEvent>(OnOutfitterClicked);
    }
    private void OnOutfitButtonClicked(ClickEvent evt)
    {

        Debug.LogWarning("OutfitButton CLICKED!");
    }
    private void OnSaveButtonClicked(ClickEvent evt)
    {
        App.style.display = DisplayStyle.Flex;
        DailyRewardButton.style.display = DisplayStyle.Flex;
        HideDisplayButton.style.display = DisplayStyle.Flex;
        Outfitter.style.display = DisplayStyle.Flex;
        OutfitterPanel.style.display = DisplayStyle.None;
        CharacterManager.instance.isOutfitting = false;
        Camera.main.GetComponent<CameraMovement>().UnlockCamera();
        UIManager.Instance.CharacterStatusDisplay.awakeState();
        Debug.LogWarning("SaveButton CLICKED!");
    }
    private void OnOutfitterClicked(ClickEvent evt)
    {
        App.style.display = DisplayStyle.None;
        DailyRewardButton.style.display = DisplayStyle.None;
        HideDisplayButton.style.display = DisplayStyle.None;
        Outfitter.style.display = DisplayStyle.None;
        OutfitterPanel.style.display = DisplayStyle.Flex;
        CharacterManager.instance.isOutfitting = true;
        Camera.main.GetComponent<CameraMovement>().LockCamera();
        UIManager.Instance.CharacterStatusDisplay.outfittingState();
        //Camera.main.GetComponent<CameraMovement>().isOutfitting = true;
    }
    //other display modification, like character position when outfitting, background , camera lock
}
