using UnityEngine;

public class ItemPickup : MonoBehaviour
{
    public ItemData itemData;

    private ItemSpawnPoint spawnPoint;

    void Start()
    {
        spawnPoint = GetComponentInParent<ItemSpawnPoint>();
    }

    public void Pickup(PlayerStats player)
    {
        switch (itemData.itemType)
        {
            case ItemType.Food:
                player.Eat(itemData.value);
                break;

            case ItemType.Water:
                player.Drink(itemData.value);
                break;

            case ItemType.Medkit:
                player.Heal(itemData.value);
                break;

            case ItemType.Ammo:
            WeaponHandler weapon = player.GetComponent<WeaponHandler>();
            if (weapon != null)
            {
                weapon.AddAmmo((int)itemData.value);
            }
            break;
        }

        if (spawnPoint != null)
            spawnPoint.ItemTaken();

        Destroy(gameObject);
    }
}