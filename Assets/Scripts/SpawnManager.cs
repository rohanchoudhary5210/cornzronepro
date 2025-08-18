using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    public static SpawnManager Instance { get; private set; }
    
    [SerializeField] private GameObject sandbagPrefab;
    [SerializeField] private Transform spawnPoint;

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
    }

    public void SpawnSandbag()
    {
        if (sandbagPrefab != null && spawnPoint != null)
        {
            GameObject newBagObject = Instantiate(sandbagPrefab, spawnPoint.position, spawnPoint.rotation);
            
            // --- ADD THIS LINE ---
            // Register the newly spawned bag with the GameManager
            GameManager.Instance.RegisterNewBag(newBagObject.GetComponent<SandbagController>());
        }
    }
    
    public void ClearSandbags()
    {
        DestroyAllSandbags();
        SpawnSandbag();
    }

    private void DestroyAllSandbags()
    {
        SandbagController[] sandbags = FindObjectsOfType<SandbagController>();
        foreach (SandbagController sandbag in sandbags)
        {
            Destroy(sandbag.gameObject);
        }
    }
}