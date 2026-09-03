using Mirror;
using UnityEngine;
using System;

public enum GameState { Prepare, Driving, Breakdown, Payout }

public class GameManager : NetworkBehaviour
{
    public static GameManager Instance;

    [SyncVar] public GameState currentState = GameState.Prepare;
    [SyncVar] public float elapsedRunTime = 0f;
    [SyncVar] public int currentDay = 1;
    [SyncVar] public bool quotaMet = false;
    [SyncVar] public bool runWon = false;

    [Header("Timing")]
    public float dayDuration = 120f; // 2 minutes day
    public float nightDuration = 120f; // 2 minutes night
    public int maxDays = 7;

    [Header("Events")]
    public static event Action<GameState> OnGameStateChanged;
    public static event Action<int> OnDayChanged;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnStartServer()
    {
        currentState = GameState.Prepare;
    }

    [ServerCallback]
    private void Update()
    {
        if (currentState == GameState.Driving || currentState == GameState.Breakdown)
        {
            elapsedRunTime += Time.deltaTime;
            int newDay = Mathf.FloorToInt(elapsedRunTime / (dayDuration + nightDuration)) + 1;
            if (newDay != currentDay && newDay <= maxDays)
            {
                currentDay = newDay;
                OnDayChanged?.Invoke(currentDay);
            }

            if (currentDay > maxDays)
            {
                EndRun(false); // Time ran out
            }
        }
    }

    [Server]
    public void StartRun()
    {
        if (currentState != GameState.Prepare) return;
        RpcTransitionState(GameState.Driving);
    }

    [Server]
    public void TriggerBreakdown()
    {
        if (currentState == GameState.Driving)
        {
            RpcTransitionState(GameState.Breakdown);
        }
    }

    [Server]
    public void ResolveBreakdown()
    {
        if (currentState == GameState.Breakdown)
        {
            RpcTransitionState(GameState.Driving);
        }
    }

    [Server]
    public void EndRun(bool won)
    {
        runWon = won;
        quotaMet = won;
        RpcTransitionState(GameState.Payout);
    }

    [ClientRpc]
    private void RpcTransitionState(GameState newState)
    {
        currentState = newState;
        OnGameStateChanged?.Invoke(newState);
    }

    public bool IsNight()
    {
        float cycleTime = elapsedRunTime % (dayDuration + nightDuration);
        return cycleTime >= dayDuration;
    }

    public float GetCycleProgress()
    {
        return elapsedRunTime % (dayDuration + nightDuration);
    }
}