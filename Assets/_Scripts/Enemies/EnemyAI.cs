using Mirror;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemyAI : NetworkBehaviour
{
    [Header("Stats")]
    public float maxHealth = 100f;
    [SyncVar] public float currentHealth;
    public float damage = 10f;
    public float attackRange = 2f;
    public float attackCooldown = 2f;

    [Header("AI")]
    public float detectionRange = 50f;
    public float stopDistance = 3f;

    protected NavMeshAgent agent;
    protected TruckController truck;
    protected float lastAttackTime;

    protected virtual void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public override void OnStartServer()
    {
        currentHealth = maxHealth;
        truck = FindObjectOfType<TruckController>();
    }

    [ServerCallback]
    protected virtual void Update()
    {
        if (truck == null || GameManager.Instance?.currentState != GameState.Driving) 
        {
            agent.isStopped = true;
            return;
        }

        float distToTruck = Vector3.Distance(transform.position, truck.transform.position);

        if (distToTruck <= detectionRange)
        {
            agent.isStopped = false;
            agent.SetDestination(truck.transform.position);

            if (distToTruck <= attackRange && Time.time - lastAttackTime > attackCooldown)
            {
                Attack();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            agent.isStopped = true;
        }
    }

    [Server]
    protected virtual void Attack()
    {
        // Default: damage truck
        var health = truck.GetComponent<TruckHealth>();
        health?.TakeDamage(damage);
    }

    [Server]
    public virtual void TakeDamage(float dmg)
    {
        currentHealth -= dmg;
        if (currentHealth <= 0)
        {
            Die();
        }
    }

    [Server]
    protected virtual void Die()
    {
        NetworkServer.Destroy(gameObject);
    }
}