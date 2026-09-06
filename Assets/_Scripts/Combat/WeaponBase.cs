using Mirror;
using UnityEngine;

public abstract class WeaponBase : NetworkBehaviour
{
    public float damage = 25f;
    public float fireRate = 0.5f;
    public float range = 100f;
    
    protected float lastFireTime;

    public abstract void Fire(Vector3 origin, Vector3 direction, PlayerRole shooter);
}