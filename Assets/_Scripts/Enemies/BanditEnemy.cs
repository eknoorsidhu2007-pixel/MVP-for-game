using UnityEngine;

public class BanditEnemy : EnemyAI
{
    [Header("Bandit")]
    public float shootRange = 30f;
    public GameObject projectilePrefab;
    public Transform firePoint;

    [ServerCallback]
    protected override void Update()
    {
        if (truck == null) return;

        float dist = Vector3.Distance(transform.position, truck.transform.position);

        if (dist <= shootRange && dist > attackRange)
        {
            // Shoot at truck
            agent.isStopped = true;
            transform.LookAt(truck.transform.position);
            
            if (Time.time - lastAttackTime > attackCooldown)
            {
                Shoot();
                lastAttackTime = Time.time;
            }
        }
        else
        {
            base.Update();
        }
    }

    [Server]
    private void Shoot()
    {
        if (firePoint == null) return;
        
        Vector3 dir = (truck.transform.position - firePoint.position).normalized;
        GameObject proj = Instantiate(projectilePrefab, firePoint.position, Quaternion.LookRotation(dir));
        Projectile p = proj.GetComponent<Projectile>();
        p?.Initialize(damage, netId, shootRange * 1.5f);
        NetworkServer.Spawn(proj);
    }
}