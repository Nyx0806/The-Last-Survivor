using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class EnemyStats : MonoBehaviour
{
    [SerializeField] private EnemyStatsData statsData;
    private int currentHP;
    private bool isDead = false;

    [SerializeField] private AudioSource srcHitAudio;
    [SerializeField] private AudioClip hitAudio;

    private Renderer enemyRenderer;
    private Color originalColor;
    private Coroutine flashRoutine;

    private EnemyAI enemyAI;
    private Animator animator;

    private void Awake()
    {
        currentHP = statsData.maxHP;
        enemyRenderer = GetComponentInChildren<Renderer>();
        animator = GetComponentInChildren<Animator>();
        originalColor = enemyRenderer.material.color;

        enemyAI = GetComponent<EnemyAI>();
    }

    public void TakeDamage(int damage)
    {
        if (isDead) return;

        currentHP -= damage;
        if(hitAudio) srcHitAudio.PlayOneShot(hitAudio);

        if (animator != null)
        {
            animator.ResetTrigger("Hit"); 
            animator.SetTrigger("Hit");
        }

        if (enemyAI != null)
        {
            enemyAI.StunEnemy(0.5f); // Tùy chỉnh số 0.5f này cho khớp với độ dài anim Hit của bạn
        }

        Debug.Log("Enemy HP: " + currentHP);

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);
        flashRoutine = StartCoroutine(FlashHit());

        if (currentHP <= 0)
        {
            Die();
        }
    }

    IEnumerator FlashHit()
    {
        enemyRenderer.material.color = Color.red;
        yield return new WaitForSeconds(0.1f);
        enemyRenderer.material.color = originalColor;
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        if (animator != null) animator.SetBool("isDead", true);

        StartCoroutine(DeathRoutine());
    }

    private IEnumerator DeathRoutine()
    {
        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null) agent.enabled = false;

        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = false;

        yield return new WaitForSeconds(5f);
        EnemyPool.Instance.ReturnEnemy(gameObject);
    }

    public void ResetEnemy()
    {
        currentHP = statsData.maxHP;
        isDead = false;
        ResetVisual();

        if (animator != null) animator.SetBool("isDead", false);

        UnityEngine.AI.NavMeshAgent agent = GetComponent<UnityEngine.AI.NavMeshAgent>();
        if (agent != null) agent.enabled = true;
        
        Collider col = GetComponent<Collider>();
        if (col != null) col.enabled = true;
    }

    void ResetVisual()
    {
        if (enemyRenderer != null)
            enemyRenderer.material.color = originalColor;
    }

    private void OnEnable()
    {
        StopAllCoroutines();
        ResetVisual();
    }

    void OnDisable()
    {
        StopAllCoroutines();    
    }
}
