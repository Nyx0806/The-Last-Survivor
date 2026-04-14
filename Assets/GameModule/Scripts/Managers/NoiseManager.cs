using UnityEngine;

public class NoiseManager : MonoBehaviour
{
    public static NoiseManager Instance;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public void MakeNoise(Vector3 position, float radius)
    {
        if (EnemyPool.Instance == null) return;

        foreach (var enemy in FindObjectsOfType<EnemyAI>())
        {
            if (!enemy.gameObject.activeInHierarchy) continue;

            float distance = Vector3.Distance(position, enemy.transform.position);

            if (distance <= radius)
            {
                enemy.OnHeardNoise(position);
            }
        }
    }
}