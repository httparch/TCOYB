using System;
using UnityEngine;

[Serializable]
public class BodyPart
{
    public int skin_id;
    public BodyPartType partType;
    public string skin_name;
    public string outfitFor;
    //public Sprite sprite;
    public bool isOwned;
    public bool isActive;
    public int itemCost;

    public static BodyPart FromSO(BodyPart_SO data)
    {
        return new BodyPart
        {
            skin_id = data.skin_id,
            partType = data.partType,
            skin_name = data.skin_name,
           // sprite = data.sprite,
            outfitFor = data.outfitFor,
            isOwned = data.isOwned,
            isActive = data.isActive,
            itemCost = data.itemCost
        };
    }
}
