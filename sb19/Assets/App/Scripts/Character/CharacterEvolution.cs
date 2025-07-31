using System.Linq;
using UnityEngine;

public class CharacterEvolution : MonoBehaviour
{

    [Header("Character Evolution Sprites (Before Level 30)")]
    public Sprite[] pabloSprites;
    public Sprite[] stellSprites;
    public Sprite[] justinSprites;
    public Sprite[] joshSprites;
    public Sprite[] kenSprites;
    private Sprite[] characterEvolution;
    /*
    [Header("Sleeping Head Sprites")]
    public Sprite pabloSleepingHead;
    public Sprite stellSleepingHead;
    public Sprite justinSleepingHead;
    public Sprite joshSleepingHead;
    public Sprite kenSleepingHead;

    private Sprite currentSleepingHead;*/

    [Header("References")]
    [SerializeField] private SpriteRenderer evolutionRenderer; // For blob/pet sprite
    [SerializeField] private GameObject player; // Humanized Player GameObject (parent)
    [SerializeField] private SpriteRenderer headRenderer; // Assign Player/head here

    private string currentBuddy;
    private bool hasInitializedHead = false;

    private void Start()
    {
        if (CharacterManager.instance == null) return;
    }

    public void SetCharacterSprites(string name)
    {
        currentBuddy = name.ToLower();
        //hasInitializedHead = false;  [dont remove] reinitialized when set new buddy

        switch (currentBuddy)
        {
            case "pablo":
                characterEvolution = pabloSprites;
                //currentSleepingHead = pabloSleepingHead;
                break;
            case "stell":
                characterEvolution = stellSprites;
              //  currentSleepingHead = stellSleepingHead;
                break;
            case "justin":
                characterEvolution = justinSprites;
              //  currentSleepingHead = justinSleepingHead;
                break;
            case "josh":
                characterEvolution = joshSprites;
              //  currentSleepingHead = joshSleepingHead;
                break;
            case "ken":
                characterEvolution = kenSprites;
               // currentSleepingHead = kenSleepingHead;
                break;
        }

        ChangeCharacterSprite(CharacterManager.instance.needsController.level);

        // Assign controller from player object
        CharacterController controller = player.GetComponent<CharacterController>();
        if (controller != null)
        {
            CharacterManager.instance.character = controller;
        }
    }
    /*
    public void SetSleepingHead(bool isSleeping)
    {
        if (isSleeping)
        {
            headRenderer.sprite = currentSleepingHead;
        }
        else
        {
            var activeHead = BodyPartManager.Instance.GetActivePart(BodyPartType.Head);
            if (activeHead != null)
            {
                var headData = BodyPartSwapManager.Instance
                    .GetAllPartsOfType(BodyPartType.Head)
                    .FirstOrDefault(h => h.skinId == activeHead.skin_id);

                if (headData != null)
                {
                    headRenderer.sprite = headData.sprite;
                }
            }
        }
    }*/

    public void ChangeCharacterSprite(int level)
    {
        if (level < 30)
        {

            // Blob/pet stage
            player.SetActive(false);
            evolutionRenderer.gameObject.SetActive(true);

            int index = Mathf.Clamp(GetEvolutionIndex(level), 0, characterEvolution.Length - 1);
            evolutionRenderer.sprite = characterEvolution[index];
            evolutionRenderer.transform.localScale = Vector3.one;
        }
        else
        {

            UIManager.Instance.OutfitDisplay.enableOutfitShop();

            evolutionRenderer.gameObject.SetActive(false);
            player.SetActive(true);

            if (!hasInitializedHead)
            {
                var headParts = BodyPartSwapManager.Instance
                    .GetAllPartsOfType(BodyPartType.Head)
                    .Where(h => h.outfitFor.ToLower() == currentBuddy)
                    .ToList();

                if (headParts.Count > 0)
                {
                    var headPart = headParts.First();
                    
                    headRenderer.sprite = headPart.sprite;

                    var existing = BodyPartManager.Instance.AllBodyParts
                  .FirstOrDefault(p => p.partType == BodyPartType.Head && p.skin_id == headPart.skinId);

                    if (existing == null)
                    {
                        // Add new part and mark it as owned + active
                        BodyPartManager.Instance.AllBodyParts.Add(new BodyPart
                        {
                            partType = BodyPartType.Head,
                            skin_id = headPart.skinId,
                            isOwned = true,
                            isActive = true
                        });
                    }
                    else
                    {
                        // Update ownership and activation
                        existing.isOwned = true;
                        existing.isActive = true;
                    }

                    BodyPartManager.Instance.SetActivePart(BodyPartType.Head, headPart.skinId);
                    hasInitializedHead = true;
                }
                else
                {
                    Debug.LogWarning($"[CharacterEvolution] No head part found for {CharacterManager.instance.needsController.name}");
                }
            }

            UIOutfitManager.instance?.UpdateSkinPreview();

        }
    }
    private int GetEvolutionIndex(int level)
    {
        if (level < 10) return 0;
        else if (level < 15) return 1;
        else if (level < 23) return 2;
        else if (level < 27) return 3;
        return 4;
    }
}
