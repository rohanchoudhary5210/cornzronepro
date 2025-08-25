using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnMangerMultiPlayer : MonoBehaviour
{
     
     public static SpawnMangerMultiPlayer Instance { get; private set; }
    [SerializeField] private GameObject player1BagPrefab;
    [SerializeField] private GameObject player2BagPrefab;

    [Header("Spawn Location")]
    [SerializeField] private Transform spawnPoint;

    /// <summary>
    /// Instantiates a new sandbag at the designated spawn point based on the current player.
    /// </summary>
    /// <param name="playerNumber">The current player (1 or 2).</param>
    /// 
   void Awake()
{
    if (Instance != null && Instance != this)
    {
        Destroy(gameObject);
    }
    else
    {
        Instance = this;
    }
}
    public void SpawnSandbag(int playerNumber)
    {
       
        GameObject prefabToSpawn = null;

        // Decide which prefab to use
        if (playerNumber == 1)
        {
            prefabToSpawn = player1BagPrefab;
        }
        else
        {
            prefabToSpawn = player2BagPrefab;
        }

        if (prefabToSpawn != null && spawnPoint != null)
        {
            //Debug.Log($"Spawning bag for Player {playerNumber} at {spawnPoint.position}");
            Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            //Debug.LogError("SpawnManager is missing a bag prefab or spawn point reference!");
        }
    }
}
