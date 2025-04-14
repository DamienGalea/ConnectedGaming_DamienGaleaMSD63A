using Sirenix.OdinInspector;
using UnityEngine;

[System.Serializable]
public class StoreItem
{
    [TableColumnWidth(100, Resizable = false)]
    [ReadOnly]
    public string ID;
    
    [TableColumnWidth(150)]
    public string Name;

    [TableColumnWidth(150)]
    public string ThumbnailUrl;

    [TableColumnWidth(70)]
    public float Price;

    [TableColumnWidth(70)]
    public float Discount;

    [TableColumnWidth(150)]
    public Sprite ThumbnailImage;

    public override string ToString()
    {
        return $"ID: {ID}, Name: {Name}, ThumbnailUrl: {ThumbnailUrl}, Price: {Price}";
    }
}
