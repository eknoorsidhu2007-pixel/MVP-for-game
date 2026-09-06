using Mirror;
using UnityEngine;

public class GunnerTurret : NetworkBehaviour
{
    [Header("Turret")]
    public Transform turretBase;
    public Transform barrel;
    public float rotationSpeed = 90f;
    public float maxUpAngle = 30f;
    public float maxDownAngle = 10f;

    [Header("Firing")]
    public WeaponBase mountedWeapon;
    public Transform muzzle;
    public float baseSpread = 2f;

    [SyncVar] public uint currentGunnerNetId;

    private void Update()
    {
        if (!isLocalPlayer) return;
        
        // Only gunner can aim
        var localPlayer = NetworkClient.localPlayer?.GetComponent<PlayerRole>();
        if (localPlayer == null || localPlayer.assignedRole != RoleType.Gunner) return;

        AimTurret();
        if (Input.GetButton("Fire1"))
        {
            Fire();
        }
    }

    private void AimTurret()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out RaycastHit hit, 200f))
        {
            Vector3 targetDir = hit.point - turretBase.position;
            Quaternion lookRot = Quaternion.LookRotation(targetDir);
            
            // Smooth rotation
            turretBase.rotation = Quaternion.RotateTowards(
                turretBase.rotation, 
                Quaternion.Euler(0, lookRot.eulerAngles.y, 0), 
                rotationSpeed * Time.deltaTime
            );

            float xAngle = lookRot.eulerAngles.x;
            if (xAngle > 180) xAngle -= 360;
            xAngle = Mathf.Clamp(xAngle, -maxDownAngle, maxUpAngle);
            barrel.localRotation = Quaternion.Euler(xAngle, 0, 0);
        }
    }

    private void Fire()
    {
        var localPlayer = NetworkClient.localPlayer?.GetComponent<PlayerRole>();
        float spread = baseSpread * localPlayer.GetGunnerSpread();
        
        Vector3 dir = barrel.forward + new Vector3(
            Random.Range(-spread, spread),
            Random.Range(-spread, spread),
            Random.Range(-spread, spread)
        );
        
        mountedWeapon.Fire(muzzle.position, dir.normalized, localPlayer);
    }

    [Command(requiresAuthority = false)]
    public void CmdClaimTurret(uint netId)
    {
        currentGunnerNetId = netId;
    }
}