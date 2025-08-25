
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the core game state, including score, coins, and the game timer.
/// It acts as the central hub for game logic.
/// </summary>
public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // --- Game State ---
    public int Score { get; private set; }
    public int Coins { get; private set; }
    [SerializeField] private float _timeRemaining = 30f;
    public bool _isTimerRunning = false;

    // --- Dependencies ---
    // Assign these in the Unity Inspector
    [SerializeField] private UIManager uiManager;
    [SerializeField] private SpawnManager spawnManager;

    void Awake()
    {
        // Singleton pattern to ensure only one instance exists
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    void Start()
    {
        spawnManager.SpawnSandbag();
        // Initialize UI and start the game timer
        uiManager.UpdateScoreText(Score);
        uiManager.UpdateCoinsText(Coins);
        _isTimerRunning = true;

        // Spawn the first sandbag

    }

    void Update()
    {
        HandleTimer();
    }

    /// <summary>
    /// Manages the countdown timer and ends the game when time runs out.
    /// </summary>
    public void HandleTimer()
    {
        if (_isTimerRunning)
        {
            if (_timeRemaining > 0)
            {
                _timeRemaining -= Time.deltaTime;
                uiManager.UpdateTimerText(_timeRemaining);
            }
            else
            {
                _timeRemaining = 0;
                _isTimerRunning = false;
                uiManager.UpdateTimerText(_timeRemaining);
                uiManager.GameOver();
                //Debug.Log("Time's up!");
            }
        }
    }

    /// <summary>
    /// Adds a specified value to the player's score.
    /// </summary>
    /// <param name="amount">The number of points to add.</param>
    public void AddScore(int amount)
    {
        Score += amount;
        uiManager.UpdateScoreText(Score);
    }

    /// <summary>
    /// Adds a specified value to the player's coins.
    /// </summary>
    /// <param name="amount">The number of coins to add.</param>
    public void AddCoins(int amount)
    {
        Coins += amount;
        uiManager.UpdateCoinsText(Coins);
    }

    /// <summary>
    /// Adds time to the game timer.
    /// </summary>
    /// <param name="amount">The amount of time in seconds to add.</param>
    public void AddTime(float amount)
    {
        _timeRemaining += amount;
    }

    /// <summary>
    /// Called when a sandbag throw is complete and a new one is needed.
    /// </summary>
    public void RequestNewSandbag()
    {
        // You could add a delay here if you want
        spawnManager.SpawnSandbag();
    }

    /// <summary>
    /// Reloads the current scene to restart the game.
    /// </summary>
    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Menu()
    {
        SceneManager.LoadScene(0);
    }

}
