using UnityEngine;

public class PlayerStats : MonoBehaviour
{
    public PlayerStatsData statsData;
    [Header("References")]
    public WeaponHandler weaponHandler;

    public float currentHP;
    public float hunger;
    public float thirst;

    [Header("Hit Feedback (Hiệu ứng khi bị đánh)")]
    public Animator anim;
    public AudioSource audioSource;
    public AudioClip[] hitSounds;

    private float damageTimer;

    private PlayerStatsUI statsUI;

    void Start()
    {
        currentHP = statsData.maxHP;
        hunger = statsData.maxHunger;
        thirst = statsData.maxThirst;

        statsUI = FindAnyObjectByType<PlayerStatsUI>();
    }

    void Update()
    {
        UpdateSurvival();
    }

    void UpdateSurvival()
    {
        hunger -= statsData.hungerDecay * Time.deltaTime;
        thirst -= statsData.thirstDecay * Time.deltaTime;

        hunger = Mathf.Clamp(hunger, 0, statsData.maxHunger);
        thirst = Mathf.Clamp(thirst, 0, statsData.maxThirst);

        damageTimer += Time.deltaTime;

        if (damageTimer >= 1f)
        {
            damageTimer = 0f;

            if (hunger <= 0)
                TakeDamage(statsData.starvationDamage);

            if (thirst <= 0)
                TakeDamage(statsData.dehydrationDamage);
        }
    }

    public void TakeDamage(float dmg)
    {
        currentHP -= dmg;

        if (statsUI != null) 
        {
            statsUI.ShowDamageFlash();
        }
        
        if (anim != null) anim.SetTrigger("Hit");

        if (weaponHandler != null)
        {
            weaponHandler.StaggerWeapon(0.5f); 
        }

        if (audioSource != null && hitSounds.Length > 0)
        {
            int randomIndex = Random.Range(0, hitSounds.Length);
            audioSource.PlayOneShot(hitSounds[randomIndex]); 
        }

        if (currentHP <= 0) Die();
    }

    public void Heal(float amount)
    {
        currentHP = Mathf.Clamp(currentHP + amount, 0, statsData.maxHP);
    }

    public void Eat(float amount)
    {
        hunger = Mathf.Clamp(hunger + amount, 0, statsData.maxHunger);
    }

    public void Drink(float amount)
    {
        thirst = Mathf.Clamp(thirst + amount, 0, statsData.maxThirst);
    }

    void Die()
    {
        Debug.Log("Player Dead");

        if (GameManager.Instance != null)
            GameManager.Instance.GameOver();
    }
}