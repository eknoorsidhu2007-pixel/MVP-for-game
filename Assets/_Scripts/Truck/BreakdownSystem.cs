using Mirror;
using UnityEngine;

public class BreakdownSystem : NetworkBehaviour
{
    [Header("Repair")]
    public float repairRate = 5f; // HP per second (non-engineer)
    public float repairTickRate = 0.5f;
    public Transform repairPoint;
    public float interactRange = 3f;

    [SyncVar] public bool isRepairing = false;
    [SyncVar] public float currentRepairProgress = 0f;
    [SyncVar] public uint currentRepairerNetId;

    private TruckHealth truckHealth;
    private float nextRepairTick;

    private void Awake()
    {
        truckHealth = GetComponent<TruckHealth>();
    }

    [ServerCallback]
    private void Update()
    {
        if (!isRepairing) return;
        if (GameManager.Instance?.currentState != GameState.Breakdown) return;

        if (Time.time >= nextRepairTick)
        {
            nextRepairTick = Time.time + repairTickRate;
            PerformRepairTick();
        }
    }

    [Server]
    private void PerformRepairTick()
    {
        var repairer = GetPlayerByNetId(currentRepairerNetId);
        float multiplier = repairer != null ? repairer.GetRepairSpeed() : 1f;
        float repairAmount = repairRate * repairTickRate * multiplier;

        truckHealth.Repair(repairAmount);

        if (!truckHealth.inBreakdown)
        {
            StopRepair();
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdStartRepair(uint playerNetId)
    {
        if (isRepairing) return;
        
        var player = GetPlayerByNetId(playerNetId);
        if (player == null) return;

        float dist = Vector3.Distance(player.transform.position, repairPoint.position);
        if (dist > interactRange) return;

        isRepairing = true;
        currentRepairerNetId = playerNetId;
    }

    [Command(requiresAuthority = false)]
    public void CmdStopRepair(uint playerNetId)
    {
        if (currentRepairerNetId == playerNetId)
        {
            StopRepair();
        }
    }

    [Server]
    private void StopRepair()
    {
        isRepairing = false;
        currentRepairerNetId = 0;
        currentRepairProgress = 0f;
    }

    private PlayerRole GetPlayerByNetId(uint netId)
    {
        var players = FindObjectsOfType<PlayerRole>();
        foreach (var p in players)
        {
            if (p.netId == netId) return p;
        }
        return null;
    }
}