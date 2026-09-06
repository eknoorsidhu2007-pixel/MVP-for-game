using UnityEngine;

public class MonsterEnemy : EnemyAI
{
    [Header("Monster")]
    public float leapRange = 8f;
    public float leapForce = 10f;

    [Server]
    protected override void Attack()
    {
        // Melee attack on truck
        base.Attack();
        
        // Visual leap effect could go here
    }
}