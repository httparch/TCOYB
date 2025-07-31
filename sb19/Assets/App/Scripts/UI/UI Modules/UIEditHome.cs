using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using System;

public class UIEditHome : MonoBehaviour
{
    //private VisualElement App;
    // private VisualElement TreeContainer;
    /*
    [System.Serializable]
    public class FurnitureImageMapping
    {
        public string furnitureName;
        public Sprite image;
    }*/

     private List<RoomManager.FurnitureImageMapping> furnitureImages => RoomManager.Instance.GetAllFurnitureImages();
    private Dictionary<string, Sprite> furnitureImageDict = new Dictionary<string, Sprite>();

    private VisualElement collectionsButton;
    public static VisualElement collectionsPanel;
    private VisualElement backButton;

    private Button YesButton, NoButton;
    private VisualElement PromptCollectionPanel;

    private VisualElement CollectionsContainer;

    private ScrollView CollectionScrollView;

    [SerializeField] private VisualTreeAsset itemTemplate;

    private Dictionary<string, VisualElement> activeItemPerType = new();

    Action yesHandler = null;
    Action noHandler = null;
    public void Initialize(UIDocument doc)
    {
        collectionsButton = doc.rootVisualElement.Q<VisualElement>("Collections");
        collectionsPanel = doc.rootVisualElement.Q<VisualElement>("CollectionsPanel");
        backButton = doc.rootVisualElement.Q<VisualElement>("BackButton");

        CollectionScrollView = doc.rootVisualElement.Q<ScrollView>("CollectionScrollView");
        CollectionsContainer = CollectionScrollView.Q<VisualElement>("CollectionsContainer");

        YesButton = doc.rootVisualElement.Q<Button>("YesButton");
        NoButton = doc.rootVisualElement.Q<Button>("NoButton");
        PromptCollectionPanel = doc.rootVisualElement.Q<VisualElement>("PromptCollectionPanel");

        activeItemPerType.Clear();

        furnitureImageDict.Clear();

        foreach (var entry in furnitureImages)
        {
            if (!furnitureImageDict.ContainsKey(entry.furnitureName))
            {
                furnitureImageDict.Add(entry.furnitureName, entry.image);
            }
        }

        CollectionsContainer.Clear();
        FurnitureManager.Instance.LoadOrInitializeFurnitures();

        foreach(var furniture in FurnitureManager.Instance.AllFurniture)
        {
            if (furniture.isOwned)
            {
                Debug.LogWarning("[Item] " + furniture.furniture_id);
                AddOwnedItemInCollection(furniture);
            }
        }


        collectionsButton.RegisterCallback<ClickEvent>(OnCollectionsClicked);
        backButton.RegisterCallback<ClickEvent>(OnBackButtonClicked);
    }

     void AddOwnedItemInCollection(Furniture furniture)
    {
        var itemInstance = itemTemplate.CloneTree();

        var itemNameLabel = itemInstance.Q<Label>("ItemName");
        var imageElement = itemInstance.Q<VisualElement>("Image");
        var itemContainer = itemInstance.Q<VisualElement>("ItemContainer");

        if (itemNameLabel == null) Debug.LogError("ItemName label not found!");
        if (imageElement == null) Debug.LogError("Image element not found!");

        if (itemNameLabel != null)
            itemNameLabel.text = furniture.furniture_name;

        if (furnitureImageDict.TryGetValue(furniture.furniture_name, out var sprite))
        {
            imageElement.style.backgroundImage = new StyleBackground(sprite);
        }

        if (FurnitureManager.Instance.IsFurnitureActive(furniture))
        {
            itemContainer.AddToClassList("active");
            activeItemPerType[furniture.furniture_type] = itemContainer;
        }


        itemInstance.RegisterCallback<ClickEvent>(evt =>
        {

            if (FurnitureManager.Instance.IsFurnitureActive(furniture))
            {
                Debug.Log($"[UI] {furniture.furniture_name} is already active. Ignoring click.");
                return;
            }

            if (yesHandler != null) YesButton.clicked -= yesHandler;
            if (noHandler != null) NoButton.clicked -= noHandler;

            PromptCollectionPanel.style.display = DisplayStyle.Flex;

            yesHandler = () =>
            {
                Debug.Log($"Clicked: {furniture.furniture_name}");

                if (activeItemPerType.TryGetValue(furniture.furniture_type, out var previouslyActive))
                {
                    previouslyActive.RemoveFromClassList("active");
                }


                itemContainer.AddToClassList("active");
                activeItemPerType[furniture.furniture_type] = itemContainer;

                FurnitureManager.Instance.SetActiveFurniture(furniture.furniture_type, furniture.furniture_id);
                RoomManager.Instance.ApplyAllActiveFurniture(furniture.furniture_type);
                PromptCollectionPanel.style.display = DisplayStyle.None;
                //[todo] add visual input when furniture is placed;
            };

            noHandler = () =>
            {
                PromptCollectionPanel.style.display = DisplayStyle.None;
            };

            YesButton.clicked += yesHandler;
            NoButton.clicked += noHandler;
        });

        CollectionsContainer.Add(itemInstance);
    }
    public void Refresh()
    {
        activeItemPerType.Clear();
        CollectionsContainer.Clear();

        FurnitureManager.Instance.LoadOrInitializeFurnitures();

        foreach (var furniture in FurnitureManager.Instance.AllFurniture)
        {
            if (furniture.isOwned)
            {
                AddOwnedItemInCollection(furniture);
            }
        }

        Debug.Log("[UIEditHome] Refreshed.");
    }

    private void OnCollectionsClicked(ClickEvent evt)
    {
        collectionsPanel.style.display = DisplayStyle.Flex;
        Camera.main.GetComponent<CameraMovement>().LockCamera();
        Debug.LogWarning("EDIT HOME CLICKED");
    }

    private void OnBackButtonClicked(ClickEvent evt)
    {
        collectionsPanel.style.display = DisplayStyle.None;
        Camera.main.GetComponent<CameraMovement>().UnlockCamera();
    }
}
