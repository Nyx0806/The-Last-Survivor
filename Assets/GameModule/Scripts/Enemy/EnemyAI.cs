using UnityEngine;
using UnityEngine.AI;

public class EnemyAI : MonoBehaviour
{
    public Transform player;
    public EnemyStatsData statsData;

    public float patrolRadius = 10f;
    public float detectionRange = 8f;
    public float losePlayerRange = 15f;
    private Vector3 noiseTarget;
    private bool investigatingNoise = false;

    [Header("Enemy Audio")]
    public AudioSource audioSource;
    
    public AudioClip[] idleSounds;  
    public AudioClip alertSound;     
    public AudioClip[] chaseSounds; 
    
    [Tooltip("Thời gian giữa các lần phát tiếng kêu (giây)")]
    public float minSoundInterval = 4f;
    public float maxSoundInterval = 8f;
    private float soundTimer;

    [Header("Combat States")]
    private bool isStunned = false; 
    private Coroutine stunCoroutine;

    private Animator animator;

    private NavMeshAgent agent;
    private Vector3 homePosition;

    private EnemyState currentState;
    private EnemyState previousState;

    private float lastAttackTime;

    enum EnemyState
    {
        Patrol,
        Chase,
        Attack
    }

    void Start()
    {
        if (player == null)
        {
            GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
            if (playerObj != null) player = playerObj.transform;
        }
        agent = GetComponent<NavMeshAgent>();
        homePosition = transform.position;
        animator = GetComponentInChildren<Animator>();
        soundTimer = Random.Range(minSoundInterval, maxSoundInterval);

        ChangeState(EnemyState.Patrol);
    }

    void Update()
    {
        HandleEnemySounds();
        if (isStunned) return;

        float distance = Vector3.Distance(transform.position, player.position);

        switch (currentState)
        {
            case EnemyState.Patrol:
                PatrolUpdate(distance);
                break;

            case EnemyState.Chase:
                ChaseUpdate(distance);
                break;

            case EnemyState.Attack:
                AttackUpdate(distance);
                break;
        }

        if (animator != null && agent != null)
        {
            animator.SetFloat("Speed", agent.velocity.magnitude);
        }
    }

    // ================= STATE UPDATES =================

    void PatrolUpdate(float distance)
    {
        if (distance <= detectionRange)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        if (!agent.isActiveAndEnabled || !agent.isOnNavMesh) return;

        if (!agent.hasPath || agent.remainingDistance <= agent.stoppingDistance)
        {
            SetRandomPatrolPoint();
        }
    }

    void ChaseUpdate(float distance)
    {
        if (!agent.isActiveAndEnabled || !agent.isOnNavMesh) return;

        if (investigatingNoise)
        {
            agent.SetDestination(noiseTarget);

            float dist = Vector3.Distance(transform.position, noiseTarget);
            if (dist < 1.5f)
            {
                investigatingNoise = false;
            }
        }
        else
        {
            agent.SetDestination(player.position);
        }

        if (distance <= statsData.attackRange)
        {
            ChangeState(EnemyState.Attack);
        }
        else if (distance > losePlayerRange)
        {
            ChangeState(EnemyState.Patrol);
        }
    }

    void AttackUpdate(float distance)
    {
        if (!agent.isActiveAndEnabled || !agent.isOnNavMesh) return;

        agent.ResetPath();
        transform.LookAt(player);

        if (distance > statsData.attackRange)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        if (Time.time - lastAttackTime >= statsData.attackCooldown)
        {
            lastAttackTime = Time.time;
            if (animator != null) animator.SetTrigger("Attack");
        }
    }

    // ================= STATE CONTROL =================

    void ChangeState(EnemyState newState)
    {
        if (currentState == newState) return;

        if (currentState == EnemyState.Patrol && newState == EnemyState.Chase)
        {
            if (audioSource != null && alertSound != null)
            {
                audioSource.Stop();
                audioSource.PlayOneShot(alertSound);
                soundTimer = alertSound.length;
            }
        }

        previousState = currentState;
        currentState = newState;

        if (agent != null && agent.isActiveAndEnabled)
        {
            if (newState == EnemyState.Patrol)
            {
                agent.speed = 2f;
            }
            else if (newState == EnemyState.Chase)
            {
                agent.speed = 3.5f;
            }
            else if (newState == EnemyState.Attack)
            {
                agent.speed = 0f;
            }
        }

        OnStateEnter(newState);
    }

    void OnStateEnter(EnemyState state)
    {
        if (agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) 
            return;
        
        switch (state)
        {
            case EnemyState.Patrol:
                SetRandomPatrolPoint();
                break;

            case EnemyState.Chase:
                agent.ResetPath();
                break;

            case EnemyState.Attack:
                agent.ResetPath();
                break;
        }
    }

    // ================= HELPERS =================

    void SetRandomPatrolPoint()
    {
        Vector3 randomDirection = Random.insideUnitSphere * patrolRadius;
        randomDirection += homePosition;

        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomDirection, out hit, patrolRadius, NavMesh.AllAreas))
        {
            agent.SetDestination(hit.position);
        }
    }

    public void OnHeardNoise(Vector3 noisePos)
    {
        if (!gameObject.activeInHierarchy || agent == null || !agent.isActiveAndEnabled || !agent.isOnNavMesh) 
            return;
        
        investigatingNoise = true;
        noiseTarget = noisePos;
        ChangeState(EnemyState.Chase);
    }

    void OnEnable()
    {
        FindPlayer();
    }

    void FindPlayer()
    {
        var playerObj = GameObject.FindGameObjectWithTag("Player");
        if (playerObj != null)
            player = playerObj.transform;
    }

    public void DealDamage()
    {
        PlayerStats playerStats = player.GetComponent<PlayerStats>();
        if (playerStats != null)
        {
            playerStats.TakeDamage(statsData.damage);
        }
    }

    public void ResetAI()
    {
        investigatingNoise = false;
        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.ResetPath();
        }
        ChangeState(EnemyState.Patrol);
    }

    void HandleEnemySounds()
    {
        if (audioSource == null) return;

        soundTimer -= Time.deltaTime;

        if (soundTimer <= 0)
        {
            if (currentState == EnemyState.Patrol && idleSounds.Length > 0)
            {
                // Phát tiếng lúc đi dạo ngẫu nhiên
                audioSource.PlayOneShot(idleSounds[Random.Range(0, idleSounds.Length)]);
                soundTimer = Random.Range(minSoundInterval, maxSoundInterval);
            }
            else if (currentState == EnemyState.Chase && chaseSounds.Length > 0)
            {
                // Phát tiếng lúc rượt đuổi
                audioSource.PlayOneShot(chaseSounds[Random.Range(0, chaseSounds.Length)]);
                soundTimer = Random.Range(2f, 4f); 
            }
            else
            {
                soundTimer = Random.Range(minSoundInterval, maxSoundInterval);
            }
        }
    }

    public void StunEnemy(float duration)
    {
        if (statsData == null || !gameObject.activeInHierarchy) return;

        if (stunCoroutine != null) StopCoroutine(stunCoroutine);
        
        stunCoroutine = StartCoroutine(StunRoutine(duration));
    }

    private System.Collections.IEnumerator StunRoutine(float duration)
    {
        isStunned = true;

        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = true; 
            agent.velocity = Vector3.zero;
        }

        yield return new WaitForSeconds(duration);

        isStunned = false; 

        if (agent != null && agent.isActiveAndEnabled && agent.isOnNavMesh)
        {
            agent.isStopped = false;
            
            if (currentState == EnemyState.Patrol) agent.speed = 1.5f;
            else if (currentState == EnemyState.Chase) agent.speed = 3.0f;
            else if (currentState == EnemyState.Attack) agent.speed = 0f;
        }
    }
}
