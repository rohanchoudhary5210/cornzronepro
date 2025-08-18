using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;


using UnityEngine.SceneManagement;

public class UIManagerMultiPlayer : MonoBehaviour
{
    public static UIManagerMultiPlayer Instance { get; private set; }

    public UIObject uiObject;

    [SerializeField] private TextMeshProUGUI bagsRemainingText;


    public float fadeDuration = 0.5f;
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
    [Header("In-Game UI")]

    [Header("Panels")]

    // [SerializeField] private TextMeshProUGUI turnPanelText;
    [SerializeField] private GameObject endOfRoundPanel;
    [SerializeField] private TextMeshProUGUI endOfRoundSummaryText;



    void Start()
    {
        FadeCanvasGroup(uiObject.inGame.Screen_, true);
        FadeCanvasGroup(uiObject.pausePanel.Screen_, false);
        FadeCanvasGroup(uiObject.gameOver.Screen_, false);
        FadeCanvasGroup(uiObject.turnPanel.Screen_, false);
        CoinManager.Instance.LoadTutorialStatus();

        if (CoinManager.Instance.tutorial == 0)
        {
            StartCoroutine(ShowTutorial());
        }
    }
    IEnumerator ShowTutorial()
    {
        uiObject.tutor.Screen_.blocksRaycasts = false;
        FadeCanvasGroup(uiObject.tutor.Screen_, true);
        Debug.Log("Tutorial shown");
        yield return new WaitForSeconds(3f);
        FadeCanvasGroup(uiObject.tutor.Screen_, false);
        Debug.Log("Tutorial hidden");

    // --- ADD THIS LINE ---
    // NOW that the tutorial is finished, we mark it as complete.
    CoinManager.Instance.TutorialCompleted();
    }

    public void UpdatePlayerScores(int p1Score, int p2Score, string p1points, string p2points)
    {
        uiObject.inGame.player1ScoreText.text = p1Score.ToString();
        uiObject.inGame.player2ScoreText.text = p2Score.ToString();
        uiObject.gameOver.player1ScoreText.text = p1Score.ToString();
        uiObject.gameOver.player2ScoreText.text = p2Score.ToString();
        uiObject.inGame.player1PointsText.text = p1points;
        uiObject.inGame.player2PointsText.text = p2points;
    }

    public void Settings()
{
    fadeDuration = 0f;
    FadeCanvasGroup(uiObject.inGame.Screen_, false);
    FadeCanvasGroup(uiObject.pausePanel.Screen_, false);
    FadeCanvasGroup(uiObject.gameOver.Screen_, false);
    FadeCanvasGroup(uiObject.gameSettings.Screen_, true);
}
public void SettingsBack()
{
    FadeCanvasGroup(uiObject.gameSettings.Screen_, false);
    FadeCanvasGroup(uiObject.pausePanel.Screen_, true);
}

    public void SetTurnText(string text)
    {
        uiObject.turnPanel.turnText.text = text;
    }

    public void UpdateBagsRemaining(int count)
    {
        bagsRemainingText.text = $"Bags Left: {count}";
    }

    /// <summary>
    /// Shows the panel with a message (e.g., "Player 2's Turn").
    /// </summary>
    /// 
    public void ShowTurnPanel(string message)
    {
        uiObject.turnPanel.turnText.text = message;
        //Debug.Log("Showing Turn Panel: " + message);
        FadeCanvasGroup(uiObject.turnPanel.Screen_, true);
    }


    /// <summary>
    /// Hides the turn panel. Called by the GameManager.
    /// </summary>
    public void HideTurnPanel()
    {
        FadeCanvasGroup(uiObject.turnPanel.Screen_, false);
        //Debug.Log("Hiding Turn Panel");
    }

    public void ShowEndOfRoundPanel(int p1Score, int p2Score)
    {
        if (p1Score > p2Score)
        {
            uiObject.gameOver.gameOverText.text = "Red Wins";
        }
        else if (p2Score > p1Score)
        {
            uiObject.gameOver.gameOverText.text = "Blue Wins";
        }
        else
        {
            uiObject.gameOver.gameOverText.text = "You Have a Tie!";
        }

        FadeCanvasGroup(uiObject.gameOver.Screen_, true);
        FadeCanvasGroup(uiObject.inGame.Screen_, false);
    }


    public void RetryGame()
    {
        FadeCanvasGroup(uiObject.gameOver.Screen_, false);
        FadeCanvasGroup(uiObject.pausePanel.Screen_, false);
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void pauseMenu()
    {
        FadeCanvasGroup(uiObject.inGame.Screen_, false);
        FadeCanvasGroup(uiObject.pausePanel.Screen_, true);
        FadeCanvasGroup(uiObject.tutor.Screen_, false);
    }

    public void resumeGame()
    {
        FadeCanvasGroup(uiObject.pausePanel.Screen_, false);
        FadeCanvasGroup(uiObject.inGame.Screen_, true);
    }
    public void backToHome()
    {
        FadeCanvasGroup(uiObject.gameOver.Screen_, false);
        FadeCanvasGroup(uiObject.pausePanel.Screen_, false);
        FadeCanvasGroup(uiObject.inGame.Screen_, false);
        SceneManager.LoadScene(0);
    }



}
[System.Serializable]

public class UIObject
{
    public InGame inGame;
    public PausePanel pausePanel;
    public GameOver gameOver;
    public TurnPanel turnPanel;
    public Tutorials tutor;
    public GameSetting gameSettings;
}
[System.Serializable]

public class InGame
{
    public CanvasGroup Screen_;
    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;
    public TextMeshProUGUI player1PointsText;
    public TextMeshProUGUI player2PointsText;
}
[System.Serializable]
public class PausePanel
{
    public CanvasGroup Screen_;
}
[System.Serializable]
public class GameOver
{
    public CanvasGroup Screen_;
    public TextMeshProUGUI gameOverText;
    public TextMeshProUGUI player1ScoreText;
    public TextMeshProUGUI player2ScoreText;
}

[System.Serializable]
public class TurnPanel
{
    public CanvasGroup Screen_;
    public TextMeshProUGUI turnText;
}
[System.Serializable]
public class Tutorials
{
    public CanvasGroup Screen_;
}

[System.Serializable]
public class GameSetting
{
    public CanvasGroup Screen_;
}
