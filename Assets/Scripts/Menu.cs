using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using Unity.VisualScripting;

public class Menu : MonoBehaviour
{
     public Sprite[] sprites;
      private Coroutine aboutAnimationCoroutine;
     public Image musicButtonImage;
    public Image soundButtonImage;
    public static Menu Instance { get; private set; }
private IEnumerator AnimateAboutImage()
{
    float duration = 0.4f; // How long the "grow" animation should take
    float timer = 0f;

    Vector3 startScale = Vector3.zero; // Start from zero size
    Vector3 endScale = Vector3.one;    // The final, normal size (1, 1, 1)

    // This loop runs for the duration specified, not forever
    while (timer < duration)
    {
        // Calculate our animation's progress (a value from 0 to 1)
        float progress = timer / duration;
        
        // Smoothly interpolate from the start scale to the end scale
        uiObjects.about.image.transform.localScale = Vector3.Lerp(startScale, endScale, progress);
        
        // Update the timer and wait for the next frame
        timer += Time.deltaTime;
        yield return null;
    }

    // At the end, set the scale exactly to the final value to ensure it's perfect
    uiObjects.about.image.transform.localScale = endScale;
}
    void Awake()
    {
        Instance = this;
        FadeCanvasGroup(uiObjects.HomeScreen.Screen_, true);
        FadeCanvasGroup(uiObjects.playPart.Screen_, false);
        FadeCanvasGroup(uiObjects.passPlaySubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.timerSubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.settings.Screen_, false);
        FadeCanvasGroup(uiObjects.onlineSoon.Screen_, false);
        FadeCanvasGroup(uiObjects.ComingSoon.Screen_, false);
         uiObjects.HomeScreen.CoinsText.text = CoinManager.PlayerCoins.ToString();
    }

    public void MusicOn()
    {
        musicButtonImage.sprite = sprites[0];
    }
    public void MusicOff()
    {
        musicButtonImage.sprite = sprites[1];
    }
    public void SoundOn()
    {
        soundButtonImage.sprite = sprites[2];
    }   
    public void SoundOff()
    {
        soundButtonImage.sprite = sprites[3];
    }
    public UIObjects uiObjects;
    public float fadeDuration = 0.5f;
    // Start is called before the first frame update
    public void SinglePlayer()
    {
        SceneManager.LoadScene(1);
    }
    public void Multiplayer()
    {
        SceneManager.LoadScene(2);
    }
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
            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, time / fadeDuration);
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
    //for the play button in the home screen
    public void Play()
    {
        FadeCanvasGroup(uiObjects.HomeScreen.Screen_, false);
        FadeCanvasGroup(uiObjects.playPart.Screen_, true);
        FadeCanvasGroup(uiObjects.passPlaySubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.timerSubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.settings.Screen_, false);
        FadeCanvasGroup(uiObjects.ComingSoon.Screen_, false);
        FadeCanvasGroup(uiObjects.passPlaySubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.onlineSoon.Screen_, false);
        FadeCanvasGroup(uiObjects.about.Screen_, false);

    }
    public void about()
    {
        FadeCanvasGroup(uiObjects.HomeScreen.Screen_, false);
        FadeCanvasGroup(uiObjects.playPart.Screen_, false);
        FadeCanvasGroup(uiObjects.passPlaySubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.timerSubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.settings.Screen_, false);
        FadeCanvasGroup(uiObjects.ComingSoon.Screen_, false);
        FadeCanvasGroup(uiObjects.passPlaySubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.onlineSoon.Screen_, false);

        FadeCanvasGroup(uiObjects.about.Screen_, true);
         aboutAnimationCoroutine = StartCoroutine(AnimateAboutImage());


    }

    public void TimerSubmenu()
    {
        FadeCanvasGroup(uiObjects.playPart.Screen_, false);
        FadeCanvasGroup(uiObjects.timerSubmenu.Screen_, true);
        FadeCanvasGroup(uiObjects.passPlaySubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.settings.Screen_, false);
        FadeCanvasGroup(uiObjects.ComingSoon.Screen_, false);
        FadeCanvasGroup(uiObjects.passPlaySubmenu.Screen_, false);

    }
    public void PassPlaySubmenu()
    {
        FadeCanvasGroup(uiObjects.playPart.Screen_, false);
        FadeCanvasGroup(uiObjects.timerSubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.passPlaySubmenu.Screen_, true);
        FadeCanvasGroup(uiObjects.settings.Screen_, false);
        FadeCanvasGroup(uiObjects.ComingSoon.Screen_, false);
        FadeCanvasGroup(uiObjects.timerSubmenu.Screen_, false);
    }
    public void ComingSoon()
    {
        FadeCanvasGroup(uiObjects.HomeScreen.Screen_, false);
        FadeCanvasGroup(uiObjects.playPart.Screen_, false);
        FadeCanvasGroup(uiObjects.ComingSoon.Screen_, true);
        FadeCanvasGroup(uiObjects.settings.Screen_, false);
        FadeCanvasGroup(uiObjects.timerSubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.passPlaySubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.passPlaySubmenu.Screen_, false);
    }
    public void ComingSoonOnline()
    {
        FadeCanvasGroup(uiObjects.HomeScreen.Screen_, false);
        FadeCanvasGroup(uiObjects.playPart.Screen_, false);
        FadeCanvasGroup(uiObjects.onlineSoon.Screen_, true);
        FadeCanvasGroup(uiObjects.ComingSoon.Screen_, false);
        FadeCanvasGroup(uiObjects.settings.Screen_, false);
        FadeCanvasGroup(uiObjects.timerSubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.passPlaySubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.passPlaySubmenu.Screen_, false);
    }
    // for coming soon 
    public void backToHome()
    {
        FadeCanvasGroup(uiObjects.ComingSoon.Screen_, false);
        FadeCanvasGroup(uiObjects.playPart.Screen_, false);
        FadeCanvasGroup(uiObjects.settings.Screen_, false);
        FadeCanvasGroup(uiObjects.HomeScreen.Screen_, true);
        FadeCanvasGroup(uiObjects.timerSubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.passPlaySubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.onlineSoon.Screen_, false);
        FadeCanvasGroup(uiObjects.about.Screen_, false);
    }
    public void Settings()
    {
        FadeCanvasGroup(uiObjects.HomeScreen.Screen_, false);
        FadeCanvasGroup(uiObjects.playPart.Screen_, false);
        FadeCanvasGroup(uiObjects.settings.Screen_, true);
        FadeCanvasGroup(uiObjects.timerSubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.passPlaySubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.passPlaySubmenu.Screen_, false);
    }
    public void BacktoSubmenu()
    {
        FadeCanvasGroup(uiObjects.settings.Screen_, false);
        FadeCanvasGroup(uiObjects.HomeScreen.Screen_, false);
        FadeCanvasGroup(uiObjects.playPart.Screen_, true);
        FadeCanvasGroup(uiObjects.timerSubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.passPlaySubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.ComingSoon.Screen_, false);
        FadeCanvasGroup(uiObjects.onlineSoon.Screen_, false);

    }
    public void street()
    {
        SceneManager.LoadScene(1);
    }
    public void rooftop()
    {
        SceneManager.LoadScene(2);
    }
    public void stadium()
    {
        SceneManager.LoadScene(3);
    }
    public void lawn()
    {
        SceneManager.LoadScene(4);
    }
}

[System.Serializable]
public class UIObjects
{
    public HomeScreen HomeScreen;
    // public Settings Setting;
    // public InGame inGame;
    // public PausePannel pausePannel;
    // public GameOver gameOver;
    // public Score score;
    public PlayPart playPart;
    public Settings settings;
    public TimerSubmenu timerSubmenu;
    public PassPlaySubmenu passPlaySubmenu;
    public ComingSoon ComingSoon;
    public onlineSoon onlineSoon;
    public About about;
}
[System.Serializable]
public class HomeScreen
{
    public CanvasGroup Screen_;
    public TextMeshProUGUI CoinsText;

}
[System.Serializable]
public class About
{
    public CanvasGroup Screen_;
    public Image image;

}
[System.Serializable]
public class PlayPart
{
    public CanvasGroup Screen_;

}
[System.Serializable]
public class Settings
{
    public CanvasGroup Screen_;
    public GameObject Sound, Music;

}
[System.Serializable]
public class ComingSoon
{
    public CanvasGroup Screen_;
 }
 [System.Serializable]
public class onlineSoon
{
    public CanvasGroup Screen_;
 }

 [System.Serializable]
public class TimerSubmenu
{
    public CanvasGroup Screen_;
 }

 [System.Serializable]
public class PassPlaySubmenu
{
    public CanvasGroup Screen_;
 }




// [System.Serializable]
// public class Settings
// {
//     public CanvasGroup Screen_;
//     public Sprite[] Icon;
//     public GameObject Sound, Music;
 
// }
// [System.Serializable]
// public class InGame
// {
//     public CanvasGroup Screen_;
//     public TextMeshProUGUI ScoreTxt, highScoreTxt;
// }
// [System.Serializable]
// public class PausePannel
// {
//     public CanvasGroup Screen_;
// }
// [System.Serializable]
// public class GameOver
// {
//     public CanvasGroup Screen_;
//     public TextMeshProUGUI ScoreTxt, highScoreTxt;
// }
// [System.Serializable]
// public class Score
// {
//     public CanvasGroup Screen_;
//     public TextMeshProUGUI NewhighScoreTxt;
// }


