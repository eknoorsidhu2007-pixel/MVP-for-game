using Mirror;
using UnityEngine;

public class EconomyManager : NetworkBehaviour
{
    public static EconomyManager Instance;

    [Header("Payout")]
    public float basePayout = 1000f;
    public float cargoValueMultiplier = 10f;

    [SyncVar] public float finalPayout;
    [SyncVar] public float cargoBonus;
    [SyncVar] public float penalty;

    private void Awake()
    {
        Instance = this;
    }

    [Server]
    public void CalculatePayout(bool won, float cargoPercent, int daysTaken)
    {
        if (!won)
        {
            finalPayout = 0;
            cargoBonus = 0;
            penalty = basePayout;
            return;
        }

        cargoBonus = cargoPercent * basePayout;
        float timeBonus = (7 - daysTaken) * 50f; // Early arrival bonus
        
        finalPayout = basePayout + cargoBonus + timeBonus;
        penalty = 0;
    }

    [ClientRpc]
    public void RpcShowPayout(float payout, float cargo, float penaltyAmt, bool won)
    {
        // Trigger UI - hook up to PayoutPhase
        PayoutPhase.Instance?.Show(payout, cargo, penaltyAmt, won);
    }
}