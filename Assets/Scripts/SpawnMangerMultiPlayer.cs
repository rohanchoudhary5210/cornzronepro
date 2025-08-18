// 
using UnityEngine;

public class SpawnMangerMultiPlayer : MonoBehaviour
{
    [Header("Sandbag Prefabs")]
    [SerializeField] private GameObject player1SandbagPrefab; // Drag the red bag prefab here
    [SerializeField] private GameObject player2SandbagPrefab; // Drag the blue bag prefab here

    [Header("Spawn Location")]
    [SerializeField] private Transform spawnPoint; // Create an empty GameObject for this

    /// <summary>
    /// This function now returns the GameObject it creates.
    /// </summary>
    /// <param name="playerNumber">The current player (1 or 2).</param>
    /// <returns>The newly spawned sandbag GameObject.</returns>
    public GameObject SpawnSandbag(int playerNumber)
    {
        // 1. Choose which prefab to use based on the current player.
        GameObject prefabToSpawn = (playerNumber == 1) ? player1SandbagPrefab : player2SandbagPrefab;

        // 2. Check if everything is set up in the Inspector to prevent errors.
        if (prefabToSpawn == null || spawnPoint == null)
        {
            Debug.LogError("A Sandbag Prefab or the Spawn Point is not assigned in the Inspector!", this);
            return null; // Return nothing to prevent a crash.
        }

        // 3. Create an instance of the chosen prefab at the spawn point's position and rotation.
        GameObject newBagInstance = Instantiate(prefabToSpawn, spawnPoint.position, spawnPoint.rotation);

        // 4. Return the new sandbag so the GameManager can track it.
        return newBagInstance;
    }
}