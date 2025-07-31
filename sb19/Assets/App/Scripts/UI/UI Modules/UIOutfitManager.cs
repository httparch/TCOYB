using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

public class UIOutfitManager : MonoBehaviour
{
    public static UIOutfitManager instance;

    public VisualElement head, torso, leftfoot, rightfoot, leftarm, rightarm;

    [Header("Scene References")]
    [SerializeField] private SpriteRenderer headRenderer;
    [SerializeField] private SpriteRenderer torsoRenderer;
    [SerializeField] private SpriteRenderer leftFootRenderer;
    [SerializeField] private SpriteRenderer rightFootRenderer;
    [SerializeField] private SpriteRenderer leftArmRenderer;
    [SerializeField] private SpriteRenderer rightArmRenderer;

    [Header("UI Toolkit Template")]
    [SerializeField] private VisualTreeAsset bodyPartItemTemplate;
    private ScrollView outfitShopScrollView;

    private Button headButton, torsoButton, leftArmButton, rightArmButton, leftFootButton, rightFootButton;

    private VisualElement Outfits,PromptOutfitDim, OutfitContainer;

    private VisualElement itemImage;
    private Label buyItText, costText;
    private Button yesShopButton, noShopButton;
    private BodyPartImageMapping currentlySelectedToBuy;
    // private CharacterSkinApplier skinApplier;
    [SerializeField] private CharacterSkinApplier skinApplier;
    private BodyPartType currentFilter = BodyPartType.Head;
    public void initialize(UIDocument doc)
    {
        instance = this;

        head = doc.rootVisualElement.Q<VisualElement>("head");
        torso = doc.rootVisualElement.Q<VisualElement>("torso");
        leftfoot = doc.rootVisualElement.Q<VisualElement>("leftfoot");
        rightfoot = doc.rootVisualElement.Q<VisualElement>("rightfoot");
        leftarm = doc.rootVisualElement.Q<VisualElement>("leftarm");
        rightarm = doc.rootVisualElement.Q<VisualElement>("rightarm");

        Outfits = doc.rootVisualElement.Q<VisualElement>("Outfits");
        OutfitContainer = doc.rootVisualElement.Q<VisualElement>("OutfitContainer");
        outfitShopScrollView = doc.rootVisualElement.Q<ScrollView>("OutfitShopScrollView");
        //skinApplier = FindObjectOfType<CharacterSkinApplier>();

        headButton = doc.rootVisualElement.Q<Button>("headButton");
        torsoButton = doc.rootVisualElement.Q<Button>("torsoButton");
        leftArmButton = doc.rootVisualElement.Q<Button>("leftArmButton");
        rightArmButton = doc.rootVisualElement.Q<Button>("rightArmButton");
        leftFootButton = doc.rootVisualElement.Q<Button>("leftFootButton");
        rightFootButton = doc.rootVisualElement.Q<Button>("rightFottButton");

        PromptOutfitDim = doc.rootVisualElement.Q<VisualElement>("PromptOutfitDim");

        itemImage = doc.rootVisualElement.Q<VisualElement>("ItemImage");
        buyItText = doc.rootVisualElement.Q<Label>("BuyItOutfitText");
        costText = doc.rootVisualElement.Q<Label>("CostOutfitText");
        yesShopButton = doc.rootVisualElement.Q<Button>("YesShopOutfitButton");
        noShopButton = doc.rootVisualElement.Q<Button>("NoShopOutfitButton");
        PromptOutfitDim.style.display = DisplayStyle.None;

        noShopButton.clicked += () => {
            PromptOutfitDim.style.display = DisplayStyle.None;
        };

        headButton.clicked += () => OnFilterClicked(BodyPartType.Head);
        torsoButton.clicked += () => OnFilterClicked(BodyPartType.Torso);
        leftArmButton.clicked += () => OnFilterClicked(BodyPartType.LeftArm);
        rightArmButton.clicked += () => OnFilterClicked(BodyPartType.RightArm);
        leftFootButton.clicked += () => OnFilterClicked(BodyPartType.LeftFoot);
        rightFootButton.clicked += () => OnFilterClicked(BodyPartType.RightFoot);

        if (BodyPartManager.Instance.AllBodyParts.Count == 0)
        {
            Debug.LogWarning("[UIOutfitManager] No body parts found. Forcing load...");
            BodyPartManager.Instance.LoadOrInitializeParts();
        }

        UpdateSkinPreview();
        OnFilterClicked(BodyPartType.Head);
    }
    private void OnFilterClicked(BodyPartType type)
    {
        currentFilter = type;
        SetActiveButton(type);
        PopulateOutfitUI(type);
    }

    private void SetActiveButton(BodyPartType activeType)
    {
        Dictionary<BodyPartType, Button> buttonMap = new()
    {
        { BodyPartType.Head, headButton },
        { BodyPartType.Torso, torsoButton },
        { BodyPartType.LeftArm, leftArmButton },
        { BodyPartType.RightArm, rightArmButton },
        { BodyPartType.LeftFoot, leftFootButton },
        { BodyPartType.RightFoot, rightFootButton },
    };

        foreach (var kvp in buttonMap)
        {
            kvp.Value.style.opacity = kvp.Key == activeType ? 0.5f : 1f;
        }
    }

    private void PopulateOutfitUI(BodyPartType filterType)
    {
        OutfitContainer.Clear();
        string currentBuddy = CharacterManager.instance?.needsController?.name?.ToLower();

        var visualParts = BodyPartSwapManager.Instance.GetAllPartsOfType(filterType)
             .Where(v =>
                 filterType != BodyPartType.Head ||  // Apply filter only to head
                 (currentBuddy != null && v.outfitFor.ToLower() == currentBuddy))
             .ToList();


        var ownedParts = BodyPartManager.Instance.AllBodyParts
            .Where(p => p.partType == filterType)
            .ToList();

        foreach (var visual in visualParts)
        {
            var matchingOwned = ownedParts.FirstOrDefault(p => p.skin_id == visual.skinId && p.isOwned);
            bool isOwned = matchingOwned != null;
            bool isActive = isOwned && matchingOwned.isActive;

            if (bodyPartItemTemplate == null)
            {
                Debug.LogError("[UIOutfitManager] bodyPartItemTemplate is NOT assigned!");
                return;
            }

            var item = bodyPartItemTemplate.CloneTree();
            var icon = item.Q<VisualElement>("BodyPartIcon");

            var sprite = visual.sprite;
            if (sprite == null)
            {
                Debug.LogWarning($"[Missing Sprite] No sprite found for {visual.partType} with ID {visual.skinId}");
                continue;
            }

            SetVisualSprite(icon, sprite);

            // ✅ Apply correct color and opacity based on owned/active status
            if (!isOwned)
            {
                // ❌ Not owned = red
                icon.style.opacity = 1f;
                icon.style.unityBackgroundImageTintColor = Color.red;
            }
            else if (isActive)
            {
                // ✅ Active = white with transparency
                icon.style.opacity = 0.5f;
                icon.style.unityBackgroundImageTintColor = Color.white;
            }
            else
            {
                // ✅ Owned (but not active) = white
                icon.style.opacity = 1f;
                icon.style.unityBackgroundImageTintColor = Color.white;
            }

            if (isOwned)
            {
                item.RegisterCallback<ClickEvent>((evt) =>
                {
                    Debug.Log($"[Equip Clicked] {visual.partType} | ID: {visual.skinId}");
                    BodyPartManager.Instance.SetActivePart(visual.partType, visual.skinId);
                    skinApplier.ApplySkin(visual.skinId, visual.partType);
                    PopulateOutfitUI(filterType); // Refresh UI
                    UpdateSkinPreview();
                });
            }
            else
            {
                item.RegisterCallback<ClickEvent>((evt) =>
                {
                    Debug.Log($"[Purchase Prompt] Not owned: {visual.partType} | ID: {visual.skinId}");

                    // Show Prompt
                    PromptOutfitDim.style.display = DisplayStyle.Flex;

                    // Fill prompt with data
                    SetVisualSprite(itemImage, visual.sprite);
                    buyItText.text = $"Buy {visual.partName}?";
                    costText.text = $"Cost: {GetCostOfItem(visual.skinId, visual.partType)} coins";

                    // Button callback
                    yesShopButton.clicked -= ConfirmPurchase;
                    yesShopButton.clicked += ConfirmPurchase;

                    // Store current visual to purchase
                    currentlySelectedToBuy = visual;
                });
            }

            OutfitContainer.Add(item);
        }
    }

    private int GetCostOfItem(int skinId, BodyPartType type)
    {
        var so = BodyPartManager.Instance
            .AllBodyParts
            .FirstOrDefault(p => p.skin_id == skinId && p.partType == type);

        if (so != null)
            return so.itemCost;

        Debug.LogWarning($"[GetCostOfItem] No SO found for {type} ID: {skinId}");
        return 0;
    }

    private void ConfirmPurchase()
    {
        if (currentlySelectedToBuy == null) return;

        // Simulate deducting coins & saving ownership
        Debug.Log($"[Purchased] {currentlySelectedToBuy.partType} | ID: {currentlySelectedToBuy.skinId}");

        // Update BodyPartManager to mark it as owned
        var newPart = new BodyPart
        {
            partType = currentlySelectedToBuy.partType,
            skin_id = currentlySelectedToBuy.skinId,
            isOwned = true,
            isActive = false
        };
        BodyPartManager.Instance.AllBodyParts.Add(newPart);


        BodyPartManager.Instance.SetActivePart(newPart.partType, newPart.skin_id);

        // Refresh UI
        PopulateOutfitUI(currentFilter);
        UpdateSkinPreview();

        // Hide prompt
        PromptOutfitDim.style.display = DisplayStyle.None;
    }

    public void UpdateSkinPreview()
    {
        ApplySpriteToRenderer(BodyPartType.Head, headRenderer);
        ApplySpriteToRenderer(BodyPartType.Torso, torsoRenderer);
        ApplySpriteToRenderer(BodyPartType.LeftArm, leftArmRenderer);
        ApplySpriteToRenderer(BodyPartType.RightArm, rightArmRenderer);
        ApplySpriteToRenderer(BodyPartType.LeftFoot, leftFootRenderer);
        ApplySpriteToRenderer(BodyPartType.RightFoot, rightFootRenderer);

        // Also update UI icons beside renderers
        SetVisualSprite(head, headRenderer.sprite);
        SetVisualSprite(torso, torsoRenderer.sprite);
        SetVisualSprite(leftarm, leftArmRenderer.sprite);
        SetVisualSprite(rightarm, rightArmRenderer.sprite);
        SetVisualSprite(leftfoot, leftFootRenderer.sprite);
        SetVisualSprite(rightfoot, rightFootRenderer.sprite);
    }


    public void SetVisualSprite(VisualElement target, Sprite sprite)
    {
        if (target == null || sprite == null || sprite.texture == null) return;

        Rect spriteRect = sprite.rect;
        Texture2D originalTexture = sprite.texture;

        Texture2D cropped = new Texture2D((int)spriteRect.width, (int)spriteRect.height);
        Color[] pixels = originalTexture.GetPixels(
            Mathf.FloorToInt(spriteRect.x),
            Mathf.FloorToInt(spriteRect.y),
            Mathf.FloorToInt(spriteRect.width),
            Mathf.FloorToInt(spriteRect.height)
        );
        cropped.SetPixels(pixels);
        cropped.Apply();
        cropped.filterMode = FilterMode.Point;

        target.style.backgroundImage = new StyleBackground(cropped);
        //target.style.unityBackgroundScaleMode = ScaleMode.ScaleToFit;
        /*
        if (target.resolvedStyle.width == 0 || target.resolvedStyle.height == 0)
        {
            target.style.width = cropped.width;
            target.style.height = cropped.height;
        }*/
        target.style.width = 256;
        target.style.height = 256;
    }


    private void ApplySpriteToRenderer(BodyPartType type, SpriteRenderer renderer)
    {
        var part = BodyPartManager.Instance.GetActivePart(type);
        if (part != null)
        {
            var sprite = BodyPartSwapManager.Instance.GetSprite(type, part.skin_id);
            if (sprite != null && renderer != null)
            {
                renderer.sprite = sprite;
            }
            else
            {
                Debug.LogWarning($"[UIOutfitManager] Missing sprite or renderer for {type}, ID: {part.skin_id}");
            }
        }
    }
}
