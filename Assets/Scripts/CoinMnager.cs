using UnityEngine;
using TMPro; // Make sure to include this for TextMeshPro UI elements

/// <summary>
/// This script demonstrates how to save and load a player's coin total using PlayerPrefs.
/// </summary>
public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }
    // --- UI Reference (Optional) ---
    // Drag a TextMeshProUGUI element here in the Inspector to see the coin total update in real-time.
    [SerializeField] private TMP_Text coinDisplayText;

    // --- State ---
    private static int _playerCoins = 0;
    public int tutorial;
    public static int PlayerCoins => _playerCoins;

    // --- PlayerPrefs Key ---
    // It's good practice to use a constant for your key to avoid typos.
    public const string COINS_KEY = "PlayerTotalCoins";
    public const string TUTORIAL_KEY = "TutorialCompleted";

    public void TutorialCompleted()
    {
        // LoadTutorialStatus();
        tutorial = 1;
        PlayerPrefs.SetInt(TUTORIAL_KEY, tutorial);
        PlayerPrefs.Save();
    }
    public void LoadTutorialStatus()
    {
        tutorial = PlayerPrefs.GetInt(TUTORIAL_KEY, 0);
        Debug.Log("Tutorial status loaded from PlayerPrefs. Value is: " + tutorial);
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {

            // If an instance
            //  already exists, destroy this new one and stop.
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadTutorialStatus();
        UpdateUI();
    }
    void Start()
    {
        // When the game starts, load the saved coin total.
        tutorial = 0;
        LoadCoins();
    }

    /// <summary>
    /// Loads the number of coins from PlayerPrefs.
    /// </summary>
    public void LoadCoins()
    {
        // Get the integer value from PlayerPrefs using our key.
        // If the key doesn't exist (e.g., the first time the player starts the game),
        // it will default to 0.
        _playerCoins = PlayerPrefs.GetInt(COINS_KEY, 0);
        //Debug.Log("Loaded coins: " + _playerCoins);

        UpdateUI();
    }

    /// <summary>
    /// Saves the current coin total to PlayerPrefs.
    /// </summary>
    public void SaveCoins()
    {
        // Set the integer value in PlayerPrefs with our key.
        PlayerPrefs.SetInt(COINS_KEY, _playerCoins);

        // It's important to call Save() to make sure the data is written to the device's memory.
        PlayerPrefs.Save();
        //Debug.Log("Coins saved: " + _playerCoins);
    }

    /// <summary>
    /// Adds a specified amount of coins and then saves the new total.
    /// This is a public method, so other scripts (like your GameManager) can call it.
    /// </summary>
    /// <param name="amountToAdd">The number of coins to add.</param>
    public void AddCoins(int amountToAdd)
    {
        _playerCoins += amountToAdd;
        //Debug.Log(amountToAdd + " coins added. New total: " + _playerCoins);

        // After changing the value, immediately save it.
        SaveCoins();
        UpdateUI();
    }

    /// <summary>
    public void UpdateUI()
    {
        if (coinDisplayText != null)
        {
            coinDisplayText.text = _playerCoins.ToString();
        }
    }

    // --- Example Test Functions ---
    // You can link these to UI buttons to test the functionality in your game.

}
