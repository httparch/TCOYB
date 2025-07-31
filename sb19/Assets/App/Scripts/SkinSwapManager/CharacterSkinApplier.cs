using UnityEngine;

public class CharacterSkinApplier : MonoBehaviour
{
    [SerializeField] private SpriteRenderer headRenderer, torsoRenderer;
    [SerializeField] private SpriteRenderer leftArmRenderer, rightArmRenderer;
    [SerializeField] private SpriteRenderer leftFootRenderer, rightFootRenderer;

    public void ApplySkin(int skinId, BodyPartType type)
    {
        var sprite = BodyPartSwapManager.Instance.GetSprite(type, skinId);

        switch (type)
        {
            case BodyPartType.Head:
                headRenderer.sprite = sprite;
                UIOutfitManager.instance.SetVisualSprite(UIOutfitManager.instance.head, sprite);
                break;
            case BodyPartType.Torso:
                torsoRenderer.sprite = sprite;
                UIOutfitManager.instance.SetVisualSprite(UIOutfitManager.instance.torso, sprite);
                break;
            case BodyPartType.LeftArm:
                leftArmRenderer.sprite = sprite;
                UIOutfitManager.instance.SetVisualSprite(UIOutfitManager.instance.leftarm, sprite);
                break;
            case BodyPartType.RightArm:
                rightArmRenderer.sprite = sprite;
                UIOutfitManager.instance.SetVisualSprite(UIOutfitManager.instance.rightarm, sprite);
                break;
            case BodyPartType.LeftFoot:
                leftFootRenderer.sprite = sprite;
                UIOutfitManager.instance.SetVisualSprite(UIOutfitManager.instance.leftfoot, sprite);
                break;
            case BodyPartType.RightFoot:
                rightFootRenderer.sprite = sprite;
                UIOutfitManager.instance.SetVisualSprite(UIOutfitManager.instance.rightfoot, sprite);
                break;
        }
    }
}