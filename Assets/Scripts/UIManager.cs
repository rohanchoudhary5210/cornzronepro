using UnityEngine;
using TMPro;
using System.Collections;
using UnityEditor;
using UnityEngine.SceneManagement;

/// <summary>
/// Handles all UI updates. It gets data from other scripts but doesn't manage game state itself.
/// </summary>

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }
    // --- UI Element References ---
    // Assign these in the Unity Inspector
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

        // --- ADD THIS LINE ---
        // NOW that the tutorial is finished, we mark it as complete.
        CoinManager.Instance.TutorialCompleted();
    }
    public uiItems uiItems;
    public bool isPaused = false;
    public float fadeDuration = 0.2f;
    public void FadeCanvasGroup(CanvasGroup canvasGroup, bool fadeIn)
    {
        // StopAllCoroutines();
        StartCoroutine(FadeRoutine(canvasGroup, fadeIn));
    }
    private IEnumerator FadeRoutine(CanvasGroup canvasGroup, bool fadeIn)
    {
        float startAlpha = canvasGroup.alpha;
        float endAlpha = fadeIn ? 1f : 0f;
        float time = 0f;
        while (time < fadeDuration)
        {
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, time /
    fadeDuration);
            time += Time.deltaTime;
            yield return null;
        }
        canvasGroup.alpha = endAlpha;
        canvasGroup.interactable = fadeIn;
        canvasGroup.blocksRaycasts = fadeIn;
    }
    public void HoldFor(float seconds)
    {
        StartCoroutine(Holdseconds(seconds));
    }
    IEnumerator Holdseconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);
        Time.timeScale = 0.00001f;
    }

    /// <summary>
    /// Updates the score display on the screen.
    /// </summary>
    /// <param name="score">The new score to display.</param>
    public void UpdateScoreText(int score)
    {
        if (uiItems.inGame.ScoreText != null)
        {
            uiItems.inGame.ScoreText.text = score.ToString();
        }
    }

    /// <summary>
    /// Updates the coins display on the screen.
    /// </summary>
    /// <param name="coins">The new coin count to display.</param>
    public void UpdateCoinsText(int coins)
    {
        if (uiItems.inGame.CoinsText != null)
        {
            uiItems.inGame.CoinsText.text = coins.ToString();
            uiItems.pauseScreen.CoinsText.text = coins.ToString();
            uiItems.gameOverScreen.CoinsText.text = coins.ToString();
        }
    }

    /// <summary>
    /// Updates the timer display on the screen.
    /// </summary>
    /// <param name="time">The time remaining.</param>
    public void UpdateTimerText(float time)
    {
        if (uiItems.inGame.TimerText != null)
        {
            // Ensure time doesn't display as negative
            if (time < 0) time = 0;
            // Format to a whole number
            uiItems.inGame.TimerText.text = "Time : " + time.ToString("0");
        }
    }

    /// <summary>
    /// Makes the Game Over panel visible.
    /// </summary>
    public void ShowGameOverPanel()
    {
        FadeCanvasGroup(uiItems.gameOverScreen.Screen_, true);
    }
    public void pauseMenu()
    {
        //isPaused = true;
        fadeDuration = 0;
        FadeCanvasGroup(uiItems.inGame.Screen_, false);
        FadeCanvasGroup(uiItems.pauseScreen.Screen_, true);
        FadeCanvasGroup(uiItems.tutor.Screen_, false);
        Time.timeScale = 0f; // Pause the game
        if (GameManager.Instance._isTimerRunning == false)
        {
            Time.timeScale = 1f; // Pause the game
            FadeCanvasGroup(uiItems.pauseScreen.Screen_, false);
            FadeCanvasGroup(uiItems.warning.Screen_, false);
            FadeCanvasGroup(uiItems.gameOverScreen.Screen_, true);
        }
    }

    public void resumeGame()
    {

        FadeCanvasGroup(uiItems.pauseScreen.Screen_, false);
        FadeCanvasGroup(uiItems.inGame.Screen_, true);
        Time.timeScale = 1f; // Resume the game
    }
    public void GameOver()
    {
        FadeCanvasGroup(uiItems.inGame.Screen_, false);
        FadeCanvasGroup(uiItems.pauseScreen.Screen_, false);
        FadeCanvasGroup(uiItems.gameOverScreen.Screen_, true);
        CoinManager.Instance.AddCoins(int.Parse(uiItems.gameOverScreen.CoinsText.text));
    }
    public void RestartGame()
    {
        FadeCanvasGroup(uiItems.gameOverScreen.Screen_, false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        Time.timeScale = 1f;
    }
    public void warningbackToHome()
    {

        FadeCanvasGroup(uiItems.inGame.Screen_, false);
        FadeCanvasGroup(uiItems.pauseScreen.Screen_, false);
        FadeCanvasGroup(uiItems.gameOverScreen.Screen_, false);
        FadeCanvasGroup(uiItems.warning.Screen_, true);
        Time.timeScale = 1f;
    }
    public void warningBackToGame()
    {
        FadeCanvasGroup(uiItems.warning.Screen_, false);
        FadeCanvasGroup(uiItems.inGame.Screen_, true);
        FadeCanvasGroup(uiItems.pauseScreen.Screen_, false);
        FadeCanvasGroup(uiItems.gameOverScreen.Screen_, false);
        Time.timeScale = 1f;
        if (GameManager.Instance._isTimerRunning == false)
        {
            FadeCanvasGroup(uiItems.pauseScreen.Screen_, false);
            FadeCanvasGroup(uiItems.warning.Screen_, false);
            FadeCanvasGroup(uiItems.gameOverScreen.Screen_, true);
        }
    }
    public void backToHome()
    {
        FadeCanvasGroup(uiItems.gameOverScreen.Screen_, false);
        FadeCanvasGroup(uiItems.pauseScreen.Screen_, false);
        FadeCanvasGroup(uiItems.inGame.Screen_, false);
        SceneManager.LoadScene(0);
    }
    public void clearbags()
    {
        SpawnManager.Instance.ClearSandbags();
    }
}

[System.Serializable]
public class uiItems
{
    public inGame inGame;
    public pauseScreen pauseScreen;
    public gameOverScreen gameOverScreen;
    public Warning warning;
    public Tutorial tutor;
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

