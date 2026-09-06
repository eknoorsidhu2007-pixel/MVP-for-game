using Mirror;
using UnityEngine;

public class PlayerRole : NetworkBehaviour
{
    [SyncVar] public RoleType assignedRole = RoleType.None;
    [SyncVar] public bool roleLocked = false;

    [Header("Role Bonuses")]
    public float engineerRepairMultiplier = 3f;
    public float gunnerAccuracyBonus = 0.8f; // Lower spread
    public float gunnerFireRateMultiplier = 1.5f;

    [Header("State")]
    public bool isInTruck = true;
    public bool isInSeat = false;

    private void Update()
    {
        if (!isLocalPlayer) return;
        HandleRoleInput();
    }

    private void HandleRoleInput()
    {
        if (assignedRole == RoleType.Driver && isInSeat)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            
            TruckController truck = FindObjectOfType<TruckController>();
            if (truck != null)
            {
                truck.CmdSetInputs(h, v, netId);
            }
        }
    }

    public void EnterSeat(SeatType seat)
    {
        isInSeat = true;
        if (seat == SeatType.Driver)
        {
            assignedRole = RoleType.Driver; // Auto-assign if entering driver seat
        }
    }

    public void ExitSeat()
    {
        if (assignedRole == RoleType.Driver)
        {
            TruckController truck = FindObjectOfType<TruckController>();
            truck?.CmdReleaseDriver(netId);
        }
        isInSeat = false;
    }

    public float GetRepairSpeed()
    {
        return assignedRole == RoleType.Engineer ? engineerRepairMultiplier : 1f;
    }

    public float GetGunnerSpread()
    {
        return assignedRole == RoleType.Gunner ? gunnerAccuracyBonus : 1.5f;
    }

    public float GetGunnerFireRate()
    {
        return assignedRole == RoleType.Gunner ? gunnerFireRateMultiplier : 1f;
    }
}

public enum SeatType { Driver, Navigator, Gunner, Passenger }