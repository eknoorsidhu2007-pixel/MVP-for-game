using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomNetworkManager : NetworkManager
{
    public static CustomNetworkManager Instance;

    [Header("Road to Quota")]
    public GameObject truckPrefab;
    public Transform truckSpawnPoint;

    private GameObject currentTruck;

    public override void Awake()
    {
        base.Awake();
        Instance = this;
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        // Spawn player at a random offset near the truck spawn
        Vector3 spawnPos = truckSpawnPoint.position + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
        GameObject player = Instantiate(playerPrefab, spawnPos, Quaternion.identity);
        NetworkServer.AddPlayerForConnection(conn, player);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        base.OnServerSceneChanged(sceneName);
        if (sceneName == "Game" && truckPrefab != null)
        {
            currentTruck = Instantiate(truckPrefab, truckSpawnPoint.position, truckSpawnPoint.rotation);
            NetworkServer.Spawn(currentTruck);
        }
    }

    public GameObject GetTruck() => currentTruck;
}