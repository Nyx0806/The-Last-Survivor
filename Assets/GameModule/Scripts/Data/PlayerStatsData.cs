using UnityEngine;

[CreateAssetMenu(fileName = "PlayerStats", menuName = "Game/Player Stats")]
public class PlayerStatsData : ScriptableObject
{
    [Header("Health")]
    public float maxHP = 100f;

    [Header("Survival")]
    public float maxHunger = 100f;
    public float maxThirst = 100f;

    [Header("Decay")]
    public float hungerDecay = 1f;
    public float thirstDecay = 1.5f;

    [Header("Damage")]
    public float starvationDamage = 5f;
    public float dehydrationDamage = 7f;
}