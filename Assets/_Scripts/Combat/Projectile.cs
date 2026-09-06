using Mirror;
using UnityEngine;

public class Projectile : NetworkBehaviour
{
    [SyncVar] public float damage;
    [SyncVar] public uint ownerNetId;
    [SyncVar] public float maxRange;
    
    private Vector3 startPos;
    private float speed = 100f;

    public void Initialize(float dmg, uint owner, float range)
    {
        damage = dmg;
        ownerNetId = owner;
        maxRange = range;
        startPos = transform.position;
    }

    private void Update()
    {
        if (!isServer) return;

        transform.position += transform.forward * speed * Time.deltaTime;

        if (Vector3.Distance(startPos, transform.position) > maxRange)
        {
            NetworkServer.Destroy(gameObject);
            return;
        }

        // Simple raycast check
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, speed * Time.deltaTime))
        {
            OnHit(hit.collider);
        }
    }

    [Server]
    private void OnHit(Collider other)
    {
        // Damage truck
        var truckHealth = other.GetComponent<TruckHealth>();
        if (truckHealth != null)
        {
            truckHealth.TakeDamage(damage);
        }

        // Damage enemy
        var enemy = other.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            enemy.TakeDamage(damage);
        }

        // Damage player
        var player = other.GetComponent<PlayerRole>();
        if (player != null && player.netId != ownerNetId)
        {
            // Implement player damage if needed
        }

        NetworkServer.Destroy(gameObject);
    }
}