using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManagerMultiplayer : MonoBehaviour
{
    public static GameManagerMultiplayer Instance { get; private set; }
    public IAudioManager audioManager;

    [SerializeField] private UIManagerMultiPlayer uiManager;
    [SerializeField] private SpawnMangerMultiPlayer spawnManager;

    private int _player1Score = 0;
    private int _player2Score = 0;
    private int _currentPlayer = 1;

    private int _player1BagsThrown = 0;
    private int _player2BagsThrown = 0;
    private const int BAGS_PER_PLAYER = 4;

    private string p1points;
    private string p2points;

    private List<GameObject> _bagsInPlay = new List<GameObject>();
    private Dictionary<GameObject, int> _bagOwners = new Dictionary<GameObject, int>();

    // --- NEW: A dictionary to store the board state before a throw ---
    private Dictionary<GameObject, int> _boardStateBeforeThrow = new Dictionary<GameObject, int>();


    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
    }

    void Start()
    {
        StartNewRound();
        audioManager = FindAnyObjectByType<AudioManager>();
    }
    
    /// <summary>
    /// This function now compares the current board state to the state before the throw.
    /// </summary>
    public void EvaluateBoardState()
    {
        if (_currentPlayer == 1) { _player1BagsThrown++; }
        else { _player2BagsThrown++; }

        _player1Score = 0;
        _player2Score = 0;
        List<string> p1ScoresList = new List<string>();
        List<string> p2ScoresList = new List<string>();

        foreach (GameObject bag in _bagsInPlay)
        {
            if (bag == null) continue;

            SandbagMultiPlayer bagController = bag.GetComponent<SandbagMultiPlayer>();
            int bagOwner = _bagOwners[bag];
            int points = 0;

            // --- THIS IS THE NEW ADVANCED SCORING LOGIC ---
            
            // 1. Get the bag's state from BEFORE the throw
            int previousValue = 0;
            _boardStateBeforeThrow.TryGetValue(bag, out previousValue);
            bool wasOnBoard = (previousValue == 1);

            // 2. Check the bag's CURRENT state and apply new rules
            if (bagController.HasScoredInHole)
            {
                if (wasOnBoard)
                {
                    
                    // If the bag WAS on the board and is NOW in the hole, it was pushed in.
                    points = 2;
                }
                else
                {
                    // Otherwise, it was thrown directly in.
                    points = 3;
                }
            }
            else if (bagController.HasLandedOnBoard && !bagController.HasHitGround)
            {
                points = 1;
            }

            // Add the calculated points to the correct player
            if (bagOwner == 1)
            {
                _player1Score += points;
                p1ScoresList.Add(points.ToString());
            }
            else
            {
                _player2Score += points;
                p2ScoresList.Add(points.ToString());
            }
        }
    
        p1points = string.Join(" ", p1ScoresList);
        p2points = string.Join(" ", p2ScoresList);
    
        UpdateUI();
        StartCoroutine(HandleNextAction());
    }

    /// <summary>
    /// This new function saves the current state of every bag.
    /// </summary>
    private void PrepareForNewThrow()
    {
        _boardStateBeforeThrow.Clear();
        foreach (GameObject bag in _bagsInPlay)
        {
            if (bag == null) continue;

            SandbagMultiPlayer bagController = bag.GetComponent<SandbagMultiPlayer>();
            int currentBagValue = 0;
            if (bagController.HasScoredInHole) { currentBagValue = 3; }
            else if (bagController.HasLandedOnBoard && !bagController.HasHitGround) { currentBagValue = 1; }
            
            _boardStateBeforeThrow[bag] = currentBagValue;
        }
        
        // After saving the state, spawn the new bag for the current player
        SpawnNewBagForCurrentPlayer();
    }
    
    private void SpawnNewBagForCurrentPlayer()
    {
        GameObject newBag = spawnManager.SpawnSandbag(_currentPlayer);
        if (newBag != null)
        {
            _bagsInPlay.Add(newBag);
            _bagOwners.Add(newBag, _currentPlayer);
        }
    }
    
    private IEnumerator HandleNextAction()
    {
        yield return new WaitForSeconds(0.1f);

        if (_player1BagsThrown >= BAGS_PER_PLAYER && _player2BagsThrown >= BAGS_PER_PLAYER)
        {
            EndRound();
        }
        else
        {
            StartCoroutine(SwitchPlayerSequence());
        }
    }

    private IEnumerator SwitchPlayerSequence()
    {
        _currentPlayer = (_currentPlayer == 1) ? 2 : 1;

        uiManager.ShowTurnPanel($"Player {_currentPlayer}'s Turn");
        yield return new WaitForSeconds(0.3f);
        uiManager.HideTurnPanel();
        
        // We now call PrepareForNewThrow which also handles spawning.
        PrepareForNewThrow(); 
        UpdateUI();
    }
    
    private void EndRound()
    {
        audioManager.PlayClip(5);
        uiManager.ShowEndOfRoundPanel(_player1Score, _player2Score);
    }

    public void StartNewRound()
    {
        foreach (GameObject bag in _bagsInPlay)
        {
            if (bag != null) { Destroy(bag); }
        }
        _bagsInPlay.Clear();
        _bagOwners.Clear();

        _currentPlayer = 1;
        _player1BagsThrown = 0;
        _player1Score = 0;
        _player2BagsThrown = 0;
        _player2Score = 0;
        p1points = string.Empty;
        p2points = string.Empty;

        UpdateUI();
        // We now call PrepareForNewThrow which also handles spawning.
        PrepareForNewThrow();
    }

    private void UpdateUI()
    {
        uiManager.UpdatePlayerScores(_player1Score, _player2Score, p1points, p2points);
        uiManager.SetTurnText($"Player {_currentPlayer}'s Turn");

        int bagsRemaining = 0;
        if (_currentPlayer == 1) { bagsRemaining = BAGS_PER_PLAYER - _player1BagsThrown; }
        else { bagsRemaining = BAGS_PER_PLAYER - _player2BagsThrown; }
        uiManager.UpdateBagsRemaining(bagsRemaining);
    }
    
    public void Menu()
    {
        SceneManager.LoadScene(0);
    }
}