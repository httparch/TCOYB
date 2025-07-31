using UnityEngine;

[CreateAssetMenu(fileName = "Furniture_SO", menuName = "ScriptableObjects/Furniture_SO")]
public class Furniture_SO : ScriptableObject
{
    public int furniture_id;
    public string furniture_type;
    public string furniture_name;
    public bool isOwned;
    public bool isActive;
    public int itemCost;
}

