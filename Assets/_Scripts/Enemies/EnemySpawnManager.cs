using Mirror;
using UnityEngine;
using System.Collections.Generic;

public class EnemySpawnManager : NetworkBehaviour
{
    public static EnemySpawnManager Instance;

    [Header("Spawns")]
    public Transform[] banditSpawnPoints;
    public Transform[] monsterSpawnPoints;
    public GameObject banditPrefab;
    public GameObject monsterPrefab;

    [Header("Timing")]
    public float daySpawnInterval = 15f;
    public float nightSpawnInterval = 6f;
    public int maxEnemies = 20;

    [Header("Ambush")]
    public Transform[] ambushPoints; // Roadside outposts
    public float ambushTriggerDistance = 30f;
    private bool[] ambushTriggered;

    private float nextSpawnTime;
    private int currentEnemyCount = 0;

    private void Awake()
    {
        Instance = this;
    }

    public override void OnStartServer()
    {
        ambushTriggered = new bool[ambushPoints.Length];
    }

    [ServerCallback]
    private void Update()
    {
        if (GameManager.Instance?.currentState != GameState.Driving) return;

        HandleAmbushes();

        if (Time.time < nextSpawnTime) return;
        if (currentEnemyCount >= maxEnemies) return;

        float interval = GameManager.Instance.IsNight() ? nightSpawnInterval : daySpawnInterval;
        nextSpawnTime = Time.time + interval;

        SpawnEnemyWave();
    }

    [Server]
    private void SpawnEnemyWave()
    {
        TruckController truck = FindObjectOfType<TruckController>();
        if (truck == null) return;

        bool spawnMonsters = GameManager.Instance.IsNight();
        int count = spawnMonsters ? Random.Range(2, 5) : Random.Range(1, 3);

        for (int i = 0; i < count; i++)
        {
            Transform[] points = spawnMonsters ? monsterSpawnPoints : banditSpawnPoints;
            if (points.Length == 0) continue;

            Transform spawn = points[Random.Range(0, points.Length)];
            Vector3 pos = spawn.position + Random.insideUnitSphere * 10f;
            pos.y = spawn.position.y;

            GameObject prefab = spawnMonsters ? monsterPrefab : banditPrefab;
            GameObject enemy = Instantiate(prefab, pos, Quaternion.identity);
            NetworkServer.Spawn(enemy);
            currentEnemyCount++;
        }
    }

    [Server]
    private void HandleAmbushes()
    {
        TruckController truck = FindObjectOfType<TruckController>();
        if (truck == null) return;

        for (int i = 0; i < ambushPoints.Length; i++)
        {
            if (ambushTriggered[i]) continue;

            float dist = Vector3.Distance(truck.transform.position, ambushPoints[i].position);
            if (dist <= ambushTriggerDistance)
            {
                ambushTriggered[i] = true;
                TriggerAmbush(ambushPoints[i]);
            }
        }
    }

    [Server]
    private void TriggerAmbush(Transform point)
    {
        // Spawn bandits at this outpost
        for (int i = 0; i < 3; i++)
        {
            Vector3 pos = point.position + new Vector3(Random.Range(-5f, 5f), 0, Random.Range(-5f, 5f));
            GameObject enemy = Instantiate(banditPrefab, pos, Quaternion.identity);
            NetworkServer.Spawn(enemy);
            currentEnemyCount++;
        }
    }

    [Server]
    public void RegisterEnemyDeath()
    {
        currentEnemyCount = Mathf.Max(0, currentEnemyCount - 1);
    }
}