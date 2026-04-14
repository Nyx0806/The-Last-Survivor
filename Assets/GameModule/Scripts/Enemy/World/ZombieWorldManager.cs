using UnityEngine;
using System.Collections;
using UnityEngine.AI;

public class ZombieWorldManager : MonoBehaviour
{
    [Header("Spawn Timing")]
    public float minSpawnDelay = 3f;
    public float maxSpawnDelay = 8f;

    [Header("Spawn Distance")]
    public float safeRadius = 12f;
    public float spawnRadius = 35f;

    [Header("Difficulty")]
    public int baseMaxAlive = 10;
    public int difficultyIncreasePerMinute = 3;

    [Header("Noise System")]
    public float noiseLevel = 0f;
    public float noiseDecayRate = 2f;
    public int noiseSpawnBonus = 5;

    private Transform player;
    private Camera cam;

    void Start()
    {
        FindPlayer();
        cam = Camera.main;

        StartCoroutine(SpawnLoop());
    }

    void Update()
    {
        if (player == null)       
            FindPlayer();

        if (cam == null)
            cam = Camera.main;
        
        DecayNoise();
    }

    void DecayNoise()
    {
        if (noiseLevel > 0)
            noiseLevel -= noiseDecayRate * Time.deltaTime;

        noiseLevel = Mathf.Clamp(noiseLevel, 0, 100);
    }

    IEnumerator SpawnLoop()
    {
        while (true)
        {
            float delay = Random.Range(minSpawnDelay, maxSpawnDelay);
            yield return new WaitForSeconds(delay);

            TrySpawn();
        }
    }

    void TrySpawn()
    {
        if (player == null || EnemyPool.Instance == null)
            return;

        int alive = EnemyPool.Instance.GetAliveEnemyCount();
        int maxAlive = GetDynamicMaxAlive();

        if (alive >= maxAlive)
            return;

        Vector3 spawnPos;
        if (!TryGetValidSpawnPosition(out spawnPos))
            return;

        if (IsVisible(spawnPos))
            return;

        EnemyPool.Instance.GetEnemy(spawnPos, Quaternion.identity);
    }

    bool TryGetValidSpawnPosition(out Vector3 validPosition)
    {
        Vector3 dir = Random.insideUnitSphere;
        dir.y = 0;
        dir.Normalize();

        float distance = Random.Range(safeRadius, spawnRadius);
        Vector3 randomPos = player.position + dir * distance;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPos, out hit, 5f, NavMesh.AllAreas))
        {
            validPosition = hit.position;
            return true;
        }

        validPosition = Vector3.zero;
        return false;
    }

    int GetDynamicMaxAlive()
    {
        int minutesAlive = Mathf.FloorToInt(Time.timeSinceLevelLoad / 60f);

        int difficultyBonus = minutesAlive * difficultyIncreasePerMinute;

        int noiseBonus = Mathf.FloorToInt(noiseLevel / 10f) * noiseSpawnBonus;

        return baseMaxAlive + difficultyBonus + noiseBonus;
    }

    Vector3 GetSpawnPosition()
    {
        Vector3 dir = Random.insideUnitSphere;
        dir.y = 0;
        dir.Normalize();

        float distance = Random.Range(safeRadius, spawnRadius);

        Vector3 spawnPos = player.position + dir * distance;

        return spawnPos;
    }

    bool IsVisible(Vector3 position)
    {
        if (cam == null)
            cam = Camera.main;

        if (cam == null)
            return false;

        Vector3 viewportPoint = cam.WorldToViewportPoint(position);

        return viewportPoint.x > 0 && viewportPoint.x < 1 &&
               viewportPoint.y > 0 && viewportPoint.y < 1 &&
               viewportPoint.z > 0;
    }

    public void AddNoise(float amount)
    {
        noiseLevel += amount;
    }

    void FindPlayer()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
    }
}