using UnityEngine;

public enum ItemType
{
    Food,
    Water,
    Medkit,
    Ammo
}

[CreateAssetMenu(menuName = "Game/Item")]
public class ItemData : ScriptableObject
{
    public string itemName;

    public ItemType itemType;

    public float value;

    public GameObject worldPrefab;
}