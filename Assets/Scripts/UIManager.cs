using UnityEngine;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    public IAudioManager audioManager;
    public uiItems uiItems;
    public bool isPaused = false;
    public float fadeDuration = 0.2f;
     void Start()
    {
        audioManager = FindAnyObjectByType<AudioManager>();
    }
    void Awake()
    {
        Instance = this;
        
        FadeCanvasGroup(uiItems.inGame.Screen_, true);
        CoinManager.Instance.LoadTutorialStatus();

        if (CoinManager.Instance.tutorial == 0)
        {
            StartCoroutine(ShowTutorial());
        }
    }

    IEnumerator ShowTutorial()
    {
        uiItems.tutor.Screen_.blocksRaycasts = false;
        FadeCanvasGroup(uiItems.tutor.Screen_, true);
        Debug.Log("Tutorial shown");
        yield return new WaitForSeconds(3f);
        FadeCanvasGroup(uiItems.tutor.Screen_, false);
        Debug.Log("Tutorial hidden");
        CoinManager.Instance.TutorialCompleted();
    }

    public void FadeCanvasGroup(CanvasGroup canvasGroup, bool fadeIn)
    {
        StartCoroutine(FadeRoutine(canvasGroup, fadeIn));
    }

    private IEnumerator FadeRoutine(CanvasGroup canvasGroup, bool fadeIn)
    {
        float startAlpha = canvasGroup.alpha;
        float endAlpha = fadeIn ? 1f : 0f;
        float time = 0f;
        while (time < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, time / fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = endAlpha;
        canvasGroup.interactable = fadeIn;
        canvasGroup.blocksRaycasts = fadeIn;
    }
    
    public void pauseMenu()
    {
        isPaused = true;
        fadeDuration = 0.2f; // It's better to not set this to 0 for smooth fades
        FadeCanvasGroup(uiItems.inGame.Screen_, false);
        FadeCanvasGroup(uiItems.pauseScreen.Screen_, true);
        FadeCanvasGroup(uiItems.tutor.Screen_, false);
        StartCoroutine(HoldForTime(0.3f)); // Wait for the fade to complete before pausing
         // Pause the game
    }
    IEnumerator HoldForTime(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Time.timeScale = 0f;
    }
    public void resumeGame()
    {
        isPaused = false;
        FadeCanvasGroup(uiItems.pauseScreen.Screen_, false);
        FadeCanvasGroup(uiItems.inGame.Screen_, true);

        Time.timeScale = 1f; // Resume the game
    }
    public void Settings()
    {
        fadeDuration = 0f;
        FadeCanvasGroup(uiItems.inGame.Screen_, false);
        FadeCanvasGroup(uiItems.pauseScreen.Screen_, false);
        FadeCanvasGroup(uiItems.gameOverScreen.Screen_, false);
        FadeCanvasGroup(uiItems.gameSettings.Screen_, true);
    }
    public void SettingsBack()
    {
        FadeCanvasGroup(uiItems.gameSettings.Screen_, false);
        FadeCanvasGroup(uiItems.pauseScreen.Screen_, true);
    }

    public void GameOver()
    {
        // Play game over sound
        FadeCanvasGroup(uiItems.inGame.Screen_, false);
        FadeCanvasGroup(uiItems.pauseScreen.Screen_, false);
        FadeCanvasGroup(uiItems.gameOverScreen.Screen_, true);
        CoinManager.Instance.AddCoins(int.Parse(uiItems.inGame.CoinsText.text)); // Get coins from inGame UI
        
    }
    
    public void RestartGame()
    {
        Time.timeScale = 1f; // Ensure time is resumed before loading new scene
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    public void warningbackToHome()
    {
        Time.timeScale = 1f;
        FadeCanvasGroup(uiItems.inGame.Screen_, false);
        FadeCanvasGroup(uiItems.pauseScreen.Screen_, false);
        FadeCanvasGroup(uiItems.gameOverScreen.Screen_, false);
        FadeCanvasGroup(uiItems.warning.Screen_, true);
    }

    public void warningBackToGame()
    {
        FadeCanvasGroup(uiItems.warning.Screen_, false);
        
        // If the timer has run out, show the game over screen. Otherwise, show the in-game screen.
        if (!GameManager.Instance.IsTimerRunning)
        {
            FadeCanvasGroup(uiItems.gameOverScreen.Screen_, true);
        }
        else
        {
            FadeCanvasGroup(uiItems.inGame.Screen_, true);
        }
        
        Time.timeScale = 1f;
    }

    public void backToHome()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene(0);
    }

    public void clearbags()
    {
        SpawnManager.Instance.ClearSandbags();
    }
    
    // --- UI Update Functions ---
    public void UpdateScoreText(int score)
    {
        if (uiItems.inGame.ScoreText != null)
            uiItems.inGame.ScoreText.text = score.ToString();
    }

    public void UpdateCoinsText(int coins)
    {
        if (uiItems.inGame.CoinsText != null)
        {
            uiItems.inGame.CoinsText.text = coins.ToString();
            uiItems.pauseScreen.CoinsText.text = coins.ToString();
            uiItems.gameOverScreen.CoinsText.text = coins.ToString();
        }
    }

    public void UpdateTimerText(float time)
    {
        if (uiItems.inGame.TimerText != null)
        {
            if (time < 0) time = 0;
            uiItems.inGame.TimerText.text = "Time : " + time.ToString("0");
        }
    }
}


// --- These data classes do not need to be in the same file, but it's okay if they are ---
[System.Serializable]
public class uiItems
{
    public inGame inGame;
    public pauseScreen pauseScreen;
    public gameOverScreen gameOverScreen;
    public Warning warning;
    public Tutorial tutor;
    public GameSettings gameSettings;
}

[System.Serializable]
public class pauseScreen
{
    public CanvasGroup Screen_;
    public TextMeshProUGUI CoinsText;
}

[System.Serializable]
public class inGame
{
    public CanvasGroup Screen_;
    public TextMeshProUGUI ScoreText;
    public TextMeshProUGUI CoinsText;
    public TextMeshProUGUI TimerText;
}

[System.Serializable]
public class gameOverScreen
{
    public CanvasGroup Screen_;
    public TextMeshProUGUI CoinsText;
}

[System.Serializable]
public class Warning
{
    public CanvasGroup Screen_;
}

[System.Serializable]
public class Tutorial
{
    public CanvasGroup Screen_;
}

[System.Serializable]
public class GameSettings
{
    public CanvasGroup Screen_;
}