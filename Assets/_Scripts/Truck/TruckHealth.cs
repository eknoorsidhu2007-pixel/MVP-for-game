using Mirror;
using UnityEngine;

public class TruckHealth : NetworkBehaviour
{
    [Header("Health")]
    [SyncVar] public float maxHP = 500f;
    [SyncVar] public float currentHP = 500f;
    [SyncVar] public bool isDestroyed = false;

    [Header("Breakdown Thresholds")]
    public float breakdownThreshold = 100f; // HP where breakdown triggers
    public bool inBreakdown = false;

    [Header("Events")]
    public static System.Action OnTruckDestroyed;
    public static System.Action OnBreakdownTriggered;
    public static System.Action OnBreakdownResolved;

    [Server]
    public void TakeDamage(float damage)
    {
        if (isDestroyed) return;
        
        currentHP -= damage;
        if (currentHP <= 0)
        {
            currentHP = 0;
            isDestroyed = true;
            OnTruckDestroyed?.Invoke();
            GameManager.Instance?.EndRun(false);
            return;
        }

        if (!inBreakdown && currentHP <= breakdownThreshold)
        {
            inBreakdown = true;
            OnBreakdownTriggered?.Invoke();
            GameManager.Instance?.TriggerBreakdown();
        }
    }

    [Server]
    public void Repair(float amount)
    {
        if (isDestroyed) return;
        
        currentHP = Mathf.Min(currentHP + amount, maxHP);
        
        if (inBreakdown && currentHP > breakdownThreshold)
        {
            inBreakdown = false;
            OnBreakdownResolved?.Invoke();
            GameManager.Instance?.ResolveBreakdown();
        }
    }

    [ClientRpc]
    public void RpcUpdateHealth(float hp)
    {
        currentHP = hp;
    }
}