using UnityEngine;

[CreateAssetMenu(fileName = "BodyPart_SO", menuName = "ScriptableObjects/BodyPart_SO")]
public class BodyPart_SO : ScriptableObject
{
    public int skin_id;
    public BodyPartType partType;
    public string skin_name;
    //public Sprite sprite;
    public bool isOwned;
    public bool isActive;
    public int itemCost;
    public string outfitFor;
}