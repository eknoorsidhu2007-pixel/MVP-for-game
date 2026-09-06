using Mirror;
using UnityEngine;

public class Sidearm : WeaponBase
{
    public Transform muzzlePoint;
    public GameObject projectilePrefab;

    public override void Fire(Vector3 origin, Vector3 direction, PlayerRole shooter)
    {
        if (Time.time - lastFireTime < fireRate) return;
        lastFireTime = Time.time;

        // Apply Gunner bonus
        float actualRate = fireRate / shooter.GetGunnerFireRate();
        if (Time.time - lastFireTime < actualRate) return;

        CmdFire(origin, direction, shooter.netId);
    }

    [Command]
    private void CmdFire(Vector3 origin, Vector3 dir, uint shooterNetId)
    {
        GameObject proj = Instantiate(projectilePrefab, origin, Quaternion.LookRotation(dir));
        Projectile p = proj.GetComponent<Projectile>();
        if (p != null)
        {
            p.Initialize(damage, shooterNetId, range);
        }
        NetworkServer.Spawn(proj);
    }
}