using UnityEngine;

/// <summary>
/// Responsible only for spawning new sandbags.
/// It is controlled by the GameManager.
/// </summary>
public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }
    // Assign the sandbag prefab and spawn point in the Unity Inspector
    [SerializeField] private GameObject sandbagPrefab;
    [SerializeField] private Transform spawnPoint;

    /// <summary>
    /// Instantiates a new sandbag at the designated spawn point.
    /// </summary>

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
    public void SpawnSandbag()
    {
        if (sandbagPrefab != null && spawnPoint != null)
        {
            // Instantiate the new sandbag and let it handle its own logic.
            Instantiate(sandbagPrefab, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            //Debug.LogError("SpawnManager is missing Sandbag Prefab or Spawn Point reference!");
        }
    }
    public void ClearSandbags()
    {
        DestroyAllSandbags();
        SpawnSandbag();
    }
   private void DestroyAllSandbags()
{
    // Find all objects with the SandbagController script
    SandbagController[] sandbags = FindObjectsOfType<SandbagController>();
    
    // Loop through the found sandbags and destroy their GameObjects
    foreach (SandbagController sandbag in sandbags)
    {
        Destroy(sandbag.gameObject);
    }
}
}
