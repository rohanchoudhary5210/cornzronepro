using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

/// <summary>
/// This version has been updated with turn-by-turn logic.
/// Player 1 throws one bag, then Player 2 throws one bag, and so on.
/// </summary>
public class GameManagerMultiplayer : MonoBehaviour
{
    public static GameManagerMultiplayer Instance { get; private set; }

    // --- Dependencies (Assign in Inspector) ---
    [SerializeField] private UIManagerMultiPlayer uiManager;
    [SerializeField] private SpawnMangerMultiPlayer spawnManager;

    // --- Game State ---
    private int _player1Score = 0;
    private int _player2Score = 0;
    private int _currentPlayer = 1;


    // --- New State Variables for Turn-by-Turn Logic ---
    private int _player1BagsThrown = 0;
    private int _player2BagsThrown = 0;
    private const int BAGS_PER_PLAYER = 4;

    private string p1points;
    private string p2points;

    private List<GameObject> _bagsInPlay = new List<GameObject>();

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

    void Start()
    {
         spawnManager.SpawnSandbag(_currentPlayer); 
         UpdateUI(); // Also update the UI to show the initial state
    }

    public void RecordThrow(int points, GameObject bag)
    {
        _bagsInPlay.Add(bag);

        // Add score and increment the bag count for the current player
        if (_currentPlayer == 1)
        {
            _player1Score += points;
            _player1BagsThrown++;
            p1points += points.ToString();
        }
        else
        {
            _player2Score += points;
            _player2BagsThrown++;
            p2points += points.ToString();
        }

        UpdateUI();
        StartCoroutine(HandleNextAction());
    }

    /// <summary>
    /// *** UPDATED LOGIC ***
    /// This now checks if the round is over or if it's time to switch to the other player.
    /// </summary>
    private IEnumerator HandleNextAction()
    {
        // Wait a moment for the player to see the result
        yield return new WaitForSeconds(0.1f);

        // Check if both players have thrown all their bags
        if (_player1BagsThrown >= BAGS_PER_PLAYER && _player2BagsThrown >= BAGS_PER_PLAYER)
        {
            EndRound();
        }
        else
        {
            // If the round is not over, switch players
            StartCoroutine(SwitchPlayerSequence());
        }
    }

    /// <summary>
    /// *** UPDATED LOGIC ***
    /// This sequence now handles the alternating turns.
    /// </summary>
    private IEnumerator SwitchPlayerSequence()
    {
        // Switch to the other player
        _currentPlayer = (_currentPlayer == 1) ? 2 : 1;

        // Show the turn panel to inform the players
        uiManager.ShowTurnPanel($"Player {_currentPlayer}'s Turn");
        yield return new WaitForSeconds(0.3f);

        uiManager.HideTurnPanel();
        UpdateUI();

        // Spawn the bag for the next player
        spawnManager.SpawnSandbag(_currentPlayer);
    }

    private void EndRound()
    {
        uiManager.ShowEndOfRoundPanel(_player1Score, _player2Score);
    }

    public void StartNewRound()
    {
        // Destroy bags from the previous round
        foreach (GameObject bag in _bagsInPlay)
        {
            if (bag != null)
            {
                Destroy(bag);
            }
        }
        _bagsInPlay.Clear();

        // Reset state for the new round
        _currentPlayer = 1;
        _player1BagsThrown = 0;
        _player1Score = 0;
        _player2BagsThrown = 0;
        _player2Score = 0;
        p1points = string.Empty;
        p2points = string.Empty;

        UpdateUI();
        spawnManager.SpawnSandbag(_currentPlayer);
    }

    /// <summary>
    /// *** UPDATED LOGIC ***
    /// Now calculates bags remaining for the specific player whose turn it is.
    /// </summary>
    private void UpdateUI()
    {
        uiManager.UpdatePlayerScores(_player1Score, _player2Score, p1points, p2points);
        uiManager.SetTurnText($"Player {_currentPlayer}'s Turn");

        int bagsRemaining = 0;
        if (_currentPlayer == 1)
        {
            bagsRemaining = BAGS_PER_PLAYER - _player1BagsThrown;
        }
        else
        {
            bagsRemaining = BAGS_PER_PLAYER - _player2BagsThrown;
        }
        uiManager.UpdateBagsRemaining(bagsRemaining);
    }
    public void Menu()
    {
        SceneManager.LoadScene(0);
    }
}
