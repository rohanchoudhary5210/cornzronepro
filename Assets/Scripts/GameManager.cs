
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using SmoothShakeFree;
using JetBrains.Annotations;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    // --- Game State ---
    public int Score { get; private set; }
    public int Coins { get; private set; }
    public bool _isTimerRunning = false;
    public float _timeRemaining = 20f;
    public Image timerBar;
    public Sprite[] timerImage;
    public Slider timerSlider;
    public int currentgamecoins;

    [Header("Power-up Settings")]
    public bool powerupActive1 = false;
    public GameObject objectToShake;

    [Header("Wind Settings")]
    //wind magnitude
    [Tooltip("Set wind magnitude (recommended values between 0.05 to 0.11)")]
    [Range(0.05f, 0.11f)]
    public float xWindVal;

    [Tooltip("Enable or disable wind effect ")]
    public bool windEnabled;

    [Tooltip("Set wind direction: true for right to left, false for left to right")]
    public bool windDirectionRightToLeft;

    [SerializeField] private UIManager uiManager;
    [SerializeField] private SpawnManager spawnManager;

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
        currentgamecoins=0;
    }

    void Start()
    {
        spawnManager.SpawnSandbag();
        uiManager.UpdateCoinsText(Coins);
        _isTimerRunning = true;
        windEnabled = true;
        xWindVal = Random.Range(0.05f, 0.07f);
        int val = Random.Range(0, 2);
        windDirectionRightToLeft = false;
    }

    void Update()
    {
        HandleTimer();
    }
    public void HandleTimer()
    {
        if (_isTimerRunning)
        {
            if (_timeRemaining > 0)
            {
                _timeRemaining -= Time.deltaTime;
                uiManager.UpdateTimerText(_timeRemaining);
                if (_timeRemaining < 8)
                {
                    timerBar.sprite = timerImage[0];
                }
                else
                {
                    timerBar.sprite = timerImage[1];
                }
            }
            else
            {
                _timeRemaining = 0;
                _isTimerRunning = false;
                uiManager.UpdateTimerText(_timeRemaining);
                uiManager.GameOver();
                //Debug.Log("Time's up!");
            }
            timerSlider.value = _timeRemaining / 20f;
        }
    }

    /// Adds time to the game timer.
    public void AddTime(float amount)
    {
        _timeRemaining += amount;
    }

    /// Called when a sandbag throw is complete and a new one is needed.
    public void RequestNewSandbag()
    {
        spawnManager.SpawnSandbag();
    }

    /// Reloads the current scene to restart the game.
    public void ResetScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    public void Menu()
    {
        SceneManager.LoadScene(0);
    }
    public async Task powerup1()
    {
        if (CoinManager.Instance.Coins >= 5)
        {
            CoinManager.Instance.UseCoins(5);
            uiManager.UpdateCoinsText(CoinManager.Instance.Coins);
            objectToShake.GetComponent<SmoothShake>().StartShake();
            Handheld.Vibrate();
        }
        else
        {
            Debug.Log("Not enough coins");
        }
    }
    public async Task powerup2()
    {
        if (CoinManager.Instance.Coins >= 5)
        {
            CoinManager.Instance.UseCoins(5);
            uiManager.UpdateCoinsText(CoinManager.Instance.Coins);
            await wind();
        }
        else
        {
            Debug.Log("Not enough coins");
        }
    }
    public async Task powerup3()
    {
        if (CoinManager.Instance.Coins >= 5)
        {
            CoinManager.Instance.UseCoins(5);
            uiManager.UpdateCoinsText(CoinManager.Instance.Coins);
            await Timer();
        }
        else
        {
            Debug.Log("Not enough coins");
        }
    }
    public async Task Timer()
    {
        powerupActive1 = true;
        await Task.Delay(5000);
        powerupActive1 = false;

    }    
    public async Task wind()
    {
        windEnabled = true;
        await Task.Delay(5000);
        windEnabled = false;

    }    
}
