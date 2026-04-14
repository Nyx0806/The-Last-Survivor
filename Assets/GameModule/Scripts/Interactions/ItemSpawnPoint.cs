using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ItemSpawnPoint : MonoBehaviour
{
    public LootTable lootTable;
    public float respawnTime = 40f;
    public float spawnRadius = 2f;
    public int maxItems = 3;
    public float delayBetweenSpawns = 0.5f;

    private List<GameObject> currentItems = new List<GameObject>();
    private float timer;
    private bool isSpawning = false;

    void Start()
    {
        StartCoroutine(SpawnSequence());
    }

    void Update()
    {
        currentItems.RemoveAll(item => item == null);

        if (currentItems.Count < maxItems && !isSpawning)
        {
            timer += Time.deltaTime;
            if (timer >= respawnTime)
            {
                StartCoroutine(SpawnSequence());
                timer = 0;
            }
        }
    }

    IEnumerator SpawnSequence()
    {
        isSpawning = true;
        
        while (currentItems.Count < maxItems)
        {
            GameObject itemPrefab = lootTable.GetRandomItem();
            if (itemPrefab == null) break;

            Vector3 randomOffset = new Vector3(
                Random.Range(-spawnRadius, spawnRadius), 
                0, 
                Random.Range(-spawnRadius, spawnRadius)
            );
            
            GameObject newItem = Instantiate(
                itemPrefab,
                transform.position + randomOffset,
                Quaternion.identity,
                transform
            );
            
            currentItems.Add(newItem);

            // Chờ một khoảng thời gian trước khi spawn item tiếp theo
            yield return new WaitForSeconds(delayBetweenSpawns);
        }

        isSpawning = false;
    }

    public void ItemTaken()
    {
        timer = 0;
    }

    // Vẽ vòng tròn mô phỏng vùng spawn
    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        int segments = 32;
        float angleStep = 360f / segments;
        Vector3 lastPoint = transform.position + new Vector3(spawnRadius, 0, 0);
        for (int i = 1; i <= segments; i++)
        {
            float angle = i * angleStep * Mathf.Deg2Rad;
            Vector3 nextPoint = transform.position + new Vector3(Mathf.Cos(angle) * spawnRadius, 0, Mathf.Sin(angle) * spawnRadius);
            Gizmos.DrawLine(lastPoint, nextPoint);
            lastPoint = nextPoint;
        }
    }
}