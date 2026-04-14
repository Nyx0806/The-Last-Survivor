using UnityEngine;
using System.Collections.Generic;

public class EnemyPool : MonoBehaviour
{
    public static EnemyPool Instance;

    public GameObject enemyPrefab;
    public int poolSize = 10;

    private Queue<GameObject> pool = new Queue<GameObject>();
    private List<GameObject> allEnemies = new List<GameObject>();

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        for (int i = 0; i < poolSize; i++)
        {
            CreateEnemy();
        }
    }

    void CreateEnemy()
    {
        GameObject enemy = Instantiate(enemyPrefab);
        DontDestroyOnLoad(enemy);
        enemy.SetActive(false);

        pool.Enqueue(enemy);
        allEnemies.Add(enemy);
    }

    public GameObject GetEnemy(Vector3 spawnPosition, Quaternion spawnRotation)
    {
        while (pool.Count > 0)
        {
            GameObject enemy = pool.Dequeue();

            if (enemy != null)
            {
                UnityEngine.AI.NavMeshAgent agent = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>();
                if (agent != null) agent.enabled = false;

                enemy.transform.position = spawnPosition;
                enemy.transform.rotation = spawnRotation;

                enemy.SetActive(true);
                if (agent != null) agent.enabled = true;

                enemy.GetComponent<EnemyStats>()?.ResetEnemy();
                enemy.GetComponent<EnemyAI>()?.ResetAI();
                
                return enemy;
            }
        }

        // fallback nếu pool rỗng
        CreateEnemy();
        return GetEnemy(spawnPosition, spawnRotation); 
    }

    public void ReturnEnemy(GameObject enemy)
    {
        if (enemy == null) return;

        enemy.SetActive(false);
        pool.Enqueue(enemy);
    }

    public void ResetPool()
    {
        foreach (GameObject enemy in allEnemies)
        {
            if (enemy == null) continue;

            enemy.SetActive(false);
            enemy.GetComponent<EnemyStats>()?.ResetEnemy();

            if (!pool.Contains(enemy))
                pool.Enqueue(enemy);
        }
    }

    public int GetAliveEnemyCount()
    {
        int count = 0;

        foreach (var enemy in allEnemies)
        {
            if (enemy != null && enemy.activeInHierarchy)
                count++;
        }

        return count;
    }
}
