using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

public class UIShop : MonoBehaviour
{
    private VisualElement shopButton;
    private VisualElement shopPanel;
    private VisualElement shopBackButton, collectionButton;

    private ScrollView ShopScrollView;
    private VisualElement ShopContainer;

    private VisualElement ChairButton, PlantButton, WallDecorationButton, WallpaperButton, WindowButton;

    private Button YesShopButton, NoShopButton;
    public VisualElement PromptShopDim, FurnitureImage;

    [SerializeField] private VisualTreeAsset itemTemplate;
    private List<RoomManager.FurnitureImageMapping> furnitureImages => RoomManager.Instance.GetAllFurnitureImages();

    private Dictionary<string, Sprite> furnitureImageDict = new();
    private List<Furniture> allNotOwnedFurniture = new();
    private List<Furniture> allOwnedFurniture = new();

    private Furniture currentSelectedFurniture;
    private string lastFilteredType = Configs._chair; // track last type

    public void Initialize(UIDocument doc)
    {
        shopButton = doc.rootVisualElement.Q<VisualElement>("Shop");
        shopPanel = doc.rootVisualElement.Q<VisualElement>("ShopPanel");
        shopBackButton = doc.rootVisualElement.Q<VisualElement>("ShopBackButton");
        collectionButton = doc.rootVisualElement.Q<VisualElement>("CollectionButton");

        PromptShopDim = doc.rootVisualElement.Q<VisualElement>("PromptShopDim");
        FurnitureImage = doc.rootVisualElement.Q<VisualElement>("FurnitureImage");
        YesShopButton = doc.rootVisualElement.Q<Button>("YesShopButton");
        NoShopButton = doc.rootVisualElement.Q<Button>("NoShopButton");

        ShopScrollView = doc.rootVisualElement.Q<ScrollView>("ShopScrollView");
        ShopContainer = ShopScrollView.Q<VisualElement>("ShopContainer");

        ChairButton = doc.rootVisualElement.Q<VisualElement>("ChairButton");
        PlantButton = doc.rootVisualElement.Q<VisualElement>("PlantButton");
        WallDecorationButton = doc.rootVisualElement.Q<VisualElement>("WallDecorationButton");
        WallpaperButton = doc.rootVisualElement.Q<VisualElement>("WallpaperButton");
        WindowButton = doc.rootVisualElement.Q<VisualElement>("WindowButton");

        // Convert to dictionary
        foreach (var entry in furnitureImages)
        {
            if (!furnitureImageDict.ContainsKey(entry.furnitureName))
                furnitureImageDict.Add(entry.furnitureName, entry.image);
        }

        FurnitureManager.Instance.LoadOrInitializeFurnitures();

        foreach (var furniture in FurnitureManager.Instance.AllFurniture)
        {
            if (!furniture.isOwned) allNotOwnedFurniture.Add(furniture);
            else allOwnedFurniture.Add(furniture);
        }

        // Register type filter buttons
        ChairButton.RegisterCallback<ClickEvent>(OnChairClicked);
        PlantButton.RegisterCallback<ClickEvent>(OnPlantClicked);
        WallDecorationButton.RegisterCallback<ClickEvent>(OnWallDecorationClicked);
        WallpaperButton.RegisterCallback<ClickEvent>(OnWallpaperClicked);
        WindowButton.RegisterCallback<ClickEvent>(OnWindowClicked);

        // Clear and rebind shop button events
        YesShopButton.clicked -= OnYesShopClicked;
        YesShopButton.clicked += OnYesShopClicked;

        NoShopButton.clicked -= OnNoShopClicked;
        NoShopButton.clicked += OnNoShopClicked;

        shopButton.RegisterCallback<ClickEvent>(OnShopClicked);
        shopBackButton.RegisterCallback<ClickEvent>(OnShopBackButtonClicked);
        collectionButton.RegisterCallback<ClickEvent>(OnCollectionButtonClicked);

        FilterByType(lastFilteredType);
    }

    private void FilterByType(string type)
    {
        lastFilteredType = type;
        ShopContainer?.Clear();

        foreach (var furniture in allNotOwnedFurniture)
        {
            if (furniture.furniture_type == type)
                AddShopItem(furniture);
        }

        foreach (var furniture in allOwnedFurniture)
        {
            if (furniture.furniture_type == type)
                AddShopItem(furniture);
        }

        Debug.Log($"Filtered shop by type: {type}");
    }

    private void AddShopItem(Furniture furniture)
    {
        var itemInstance = itemTemplate.CloneTree();

        var itemNameLabel = itemInstance.Q<Label>("ItemName");
        var imageElement = itemInstance.Q<VisualElement>("Image");
        var itemCostLabel = itemInstance.Q<Label>("ItemCost");
        var itemContainer = itemInstance.Q<VisualElement>("ItemContainer");
        var itemCostContainer = itemInstance.Q<VisualElement>("ItemCostContainer");

        itemNameLabel.text = furniture.furniture_name;

        if (imageElement != null && furnitureImageDict.TryGetValue(furniture.furniture_name, out var sprite) && sprite != null)
        {
            imageElement.style.backgroundImage = new StyleBackground(sprite);
        }

        itemCostLabel.text = $"{furniture.itemCost}";

        if (furniture.isOwned)
        {
            itemContainer.SetEnabled(false);
            itemContainer.AddToClassList("Owned");
        }

        itemCostContainer.RegisterCallback<ClickEvent>(evt =>
        {
            if (CharacterManager.instance.needsController.gold >= furniture.itemCost)
            {
                currentSelectedFurniture = furniture;
                PromptShopDim.style.display = DisplayStyle.Flex;

                if (furnitureImageDict.TryGetValue(furniture.furniture_name, out var selectedSprite) && selectedSprite != null)
                {
                    FurnitureImage.style.backgroundImage = new StyleBackground(selectedSprite);
                }
            }
            else
            {
                Debug.LogWarning("Not enough gold");
            }
        });

        ShopContainer.Add(itemInstance);
    }

    private void OnYesShopClicked()
    {
        if (currentSelectedFurniture == null)
        {
            Debug.LogWarning("No furniture selected.");
            return;
        }

        if (CharacterManager.instance.needsController.gold < currentSelectedFurniture.itemCost)
        {
            Debug.LogWarning("Tried to buy without enough gold");
            return;
        }

        CharacterManager.instance.needsController.ChangeMoney(-currentSelectedFurniture.itemCost);
        currentSelectedFurniture.isOwned = true;

        DatabaseManager.instance.database.SaveData(FurnitureManager.Instance.AllFurniture, Configs._FurnitureFilename);

        allOwnedFurniture.Add(currentSelectedFurniture);
        allNotOwnedFurniture.Remove(currentSelectedFurniture);

        UIManager.Instance.EditHomeDisplay.Refresh();
        Refresh();

        PromptShopDim.style.display = DisplayStyle.None;
    }

    private void OnNoShopClicked()
    {
        PromptShopDim.style.display = DisplayStyle.None;
    }

    public void Refresh()
    {
        allNotOwnedFurniture.Clear();
        allOwnedFurniture.Clear();
        ShopContainer?.Clear();
        currentSelectedFurniture = null;

        FurnitureManager.Instance.LoadOrInitializeFurnitures();

        foreach (var furniture in FurnitureManager.Instance.AllFurniture)
        {
            if (!furniture.isOwned) allNotOwnedFurniture.Add(furniture);
            else allOwnedFurniture.Add(furniture);
        }

        FilterByType(lastFilteredType);
        Debug.Log("[UIShop] Refreshed.");
    }

    private void OnShopClicked(ClickEvent evt)
    {
        shopPanel.style.display = DisplayStyle.Flex;
        Camera.main.GetComponent<CameraMovement>().LockCamera();
        Refresh();
        Debug.LogWarning("SHOP CLICKED");
    }

    private void OnCollectionButtonClicked(ClickEvent evt)
    {
        shopPanel.style.display = DisplayStyle.None;
        Camera.main.GetComponent<CameraMovement>().LockCamera();
        UIEditHome.collectionsPanel.style.display = DisplayStyle.Flex;
    }

    private void OnShopBackButtonClicked(ClickEvent evt)
    {
        shopPanel.style.display = DisplayStyle.None;
        Camera.main.GetComponent<CameraMovement>().UnlockCamera();
    }

    // Button filter method groups
    private void OnChairClicked(ClickEvent evt) => FilterByType(Configs._chair);
    private void OnPlantClicked(ClickEvent evt) => FilterByType(Configs._plants);
    private void OnWallDecorationClicked(ClickEvent evt) => FilterByType(Configs._wallDecoration);
    private void OnWallpaperClicked(ClickEvent evt) => FilterByType(Configs._wallpaper);
    private void OnWindowClicked(ClickEvent evt) => FilterByType(Configs._window);
}
