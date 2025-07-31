using System;
using UnityEditor.Experimental;
using UnityEngine;
[Serializable]
public class Furniture 
{
    public int furniture_id;
    public string furniture_type;
    public string furniture_name;
    public bool isOwned;
    public bool isActive;
    public int itemCost;
    //public Sprite image;

    public static Furniture FROMSO(Furniture_SO data)
    {
        return new Furniture
        {
            furniture_id = data.furniture_id,
            furniture_type = data.furniture_type,
            furniture_name = data.furniture_name,
            isOwned = data.isOwned,
            isActive = data.isActive,
            itemCost = data.itemCost
        };
    }

}
