using Mirror;
using UnityEngine;

public class CargoSystem : NetworkBehaviour
{
    [SyncVar] public float maxCargo = 100f;
    [SyncVar] public float currentCargo = 100f;
    [SyncVar] public int cratesStolen = 0;

    [Header("Events")]
    public static System.Action<float> OnCargoLost;

    [Server]
    public void LoseCargo(float amount)
    {
        currentCargo = Mathf.Max(0, currentCargo - amount);
        OnCargoLost?.Invoke(amount);
    }

    [Server]
    public void StealCrate()
    {
        cratesStolen++;
        LoseCargo(maxCargo * 0.1f); // 10% per stolen crate
    }

    public float GetCargoPercent() => currentCargo / maxCargo;
}