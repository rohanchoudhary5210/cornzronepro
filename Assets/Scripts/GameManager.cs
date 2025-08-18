using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField] private UIManager uiManager;
    [SerializeField] private SpawnManager spawnManager;
    
    public int Score { get; private set; }
    public int Coins { get; private set; }
    [SerializeField] private float _timeRemaining = 30f;
    private bool _isTimerRunning = false;
    public bool IsTimerRunning => _isTimerRunning;
    public IAudioManager audioManager;
    private List<SandbagController> _bagsInPlay = new List<SandbagController>();
    private Dictionary<SandbagController, int> _boardStateBeforeThrow = new Dictionary<SandbagController, int>();

    void Awake()
    {
        if (Instance != null && Instance != this) { Destroy(gameObject); }
        else { Instance = this; }
    }

    void Start()
    {
        audioManager = FindAnyObjectByType<AudioManager>();
        Time.timeScale = 1f;
        _isTimerRunning = true;
        uiManager.UpdateScoreText(Score);
        uiManager.UpdateCoinsText(Coins);
        PrepareForNewThrow();

    }

    void Update()
    {
        HandleTimer();
    }

    public void RegisterNewBag(SandbagController newBag)
    {
        if (newBag != null)
        {
            _bagsInPlay.Add(newBag);
        }
    }

    public void EvaluateBoardState()
    {
        Score = 0;
        Coins = 0;

        foreach (SandbagController bag in _bagsInPlay)
        {
            bool wasOnBoard = _boardStateBeforeThrow.ContainsKey(bag) && _boardStateBeforeThrow[bag] == 1;
            bool wasInHole = _boardStateBeforeThrow.ContainsKey(bag) && _boardStateBeforeThrow[bag] == 3;

            if (bag.HasScoredInHole)
            {
                Score += 3;

                // *** YOUR NEW TIME RULE IS IMPLEMENTED HERE ***
                // Only add time if the bag was NOT already in the hole before this throw.
                if (!wasInHole)
                {
                    AddTime(10f);
                    Debug.Log("New cornhole! +10 seconds.");
                }
                
                if (wasOnBoard)
                {
                    Coins += 20;
                }
                else
                {
                    Coins += 30;
                }
            }
            else if (bag.HasLandedOnBoard && !bag.HasHitGround)
            {
                Score += 1;
                //Coins += 10;
            }
        }

        uiManager.UpdateScoreText(Score);
        uiManager.UpdateCoinsText(Coins);
        
        RequestNewSandbag();
    }
    
    private void PrepareForNewThrow()
    {
        _boardStateBeforeThrow.Clear();
        foreach (SandbagController bag in _bagsInPlay)
        {
            int currentBagValue = 0;
            if (bag.HasScoredInHole) { currentBagValue = 3; }
            else if (bag.HasLandedOnBoard && !bag.HasHitGround) { currentBagValue = 1; }
            _boardStateBeforeThrow[bag] = currentBagValue;
        }
        
        spawnManager.SpawnSandbag();
    }

    public void RequestNewSandbag()
    {
        PrepareForNewThrow();
    }
    
    #region Other Methods
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
                audioManager.PlayClip(5); 
                uiManager.GameOver();
            }
        }
    }

    public void AddTime(float amount)
    {
        _timeRemaining += amount;
    }
    
    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Menu()
    {
        SceneManager.LoadScene(0);
    }
    #endregion
}