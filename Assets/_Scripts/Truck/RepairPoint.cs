using UnityEngine;

public class RepairPoint : MonoBehaviour
{
    private BreakdownSystem breakdown;

    private void Awake()
    {
        breakdown = GetComponentInParent<BreakdownSystem>();
    }

    private void OnTriggerStay(Collider other)
    {
        var player = other.GetComponent<PlayerRole>();
        if (player == null || !player.isLocalPlayer) return;

        if (Input.GetKey(KeyCode.E) && breakdown != null)
        {
            breakdown.CmdStartRepair(player.netId);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var player = other.GetComponent<PlayerRole>();
        if (player == null) return;

        breakdown?.CmdStopRepair(player.netId);
    }
}