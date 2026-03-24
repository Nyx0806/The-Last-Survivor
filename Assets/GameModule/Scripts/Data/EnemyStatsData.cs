using UnityEngine;

[CreateAssetMenu(fileName = "EnemyStatsData", menuName = "Stats/Enemy Stats")]
public class EnemyStatsData : ScriptableObject
{
    public int maxHP = 50;
    public int damage = 10;
    public float attackRange = 1.5f;
    public float attackCooldown = 1.5f;
    public float moveSpeed = 3f;
}
