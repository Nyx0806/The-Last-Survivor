using UnityEngine;

[System.Serializable]
public class LootEntry
{
    public GameObject itemPrefab;
    public int weight = 1;
}

[CreateAssetMenu(menuName = "Loot/Loot Table")]
public class LootTable : ScriptableObject
{
    public LootEntry[] items;

    public GameObject GetRandomItem()
    {
        int totalWeight = 0;

        foreach (var item in items)
        {
            totalWeight += item.weight;
        }

        int random = Random.Range(0, totalWeight);

        int current = 0;

        foreach (var item in items)
        {
            current += item.weight;

            if (random < current)
                return item.itemPrefab;
        }

        return null;
    }
}