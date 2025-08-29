using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;


public class Menu : MonoBehaviour
{

    public Sprite[] sprites;
    private Coroutine aboutAnimationCoroutine;
    public Image musicButtonImage;
    public Image soundButtonImage;
    public static Menu Instance { get; private set; }
    public int index = 1;

    void Awake()
    {
        Instance = this;
        GameOn();
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

    public void GameOn()
    {
        FadeCanvasGroup(uiObjects.HomeScreen.Screen_, true);
        FadeCanvasGroup(uiObjects.playPart.Screen_, false);
        FadeCanvasGroup(uiObjects.passPlaySubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.timerSubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.settings.Screen_, false);
        FadeCanvasGroup(uiObjects.about.Screen_, false);
        FadeCanvasGroup(uiObjects.dailyRewards.Screen_, false);
        FadeCanvasGroup(uiObjects.dailyRewardsMenu.Screen_, false);
    }
    public void play()
    {
        FadeCanvasGroup(uiObjects.HomeScreen.Screen_, false);
        FadeCanvasGroup(uiObjects.playPart.Screen_, true);
        FadeCanvasGroup(uiObjects.passPlaySubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.timerSubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.settings.Screen_, false);
        FadeCanvasGroup(uiObjects.about.Screen_, false);
        FadeCanvasGroup(uiObjects.dailyRewards.Screen_, false);
        FadeCanvasGroup(uiObjects.dailyRewardsMenu.Screen_, false);
    }
    public void dailyRewards()
    {
        FadeCanvasGroup(uiObjects.HomeScreen.Screen_, false);
        FadeCanvasGroup(uiObjects.playPart.Screen_, false);
        FadeCanvasGroup(uiObjects.passPlaySubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.timerSubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.settings.Screen_, false);
        FadeCanvasGroup(uiObjects.about.Screen_, false);
        FadeCanvasGroup(uiObjects.dailyRewards.Screen_, true);
        FadeCanvasGroup(uiObjects.dailyRewardsMenu.Screen_, false);
    }
    public void settings(bool isActive)
    {
        FadeCanvasGroup(uiObjects.settings.Screen_, isActive);
    }

    public void aboutUs()
    {
        FadeCanvasGroup(uiObjects.settings.Screen_, false);
        FadeCanvasGroup(uiObjects.about.Screen_, true);
    }
    public void infoBackToSettings()
    {
        FadeCanvasGroup(uiObjects.about.Screen_, false);
        FadeCanvasGroup(uiObjects.settings.Screen_, true);
    }
    public void BackToMenu()
    {
        FadeCanvasGroup(uiObjects.HomeScreen.Screen_, true);
        FadeCanvasGroup(uiObjects.playPart.Screen_, false);
        FadeCanvasGroup(uiObjects.passPlaySubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.timerSubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.settings.Screen_, false);
        FadeCanvasGroup(uiObjects.about.Screen_, false);
        FadeCanvasGroup(uiObjects.dailyRewards.Screen_, false);
        FadeCanvasGroup(uiObjects.dailyRewardsMenu.Screen_, false);
    }
    public void timer()
    {
        FadeCanvasGroup(uiObjects.HomeScreen.Screen_, false);
        FadeCanvasGroup(uiObjects.playPart.Screen_, false);
        FadeCanvasGroup(uiObjects.passPlaySubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.timerSubmenu.Screen_, true);
        FadeCanvasGroup(uiObjects.settings.Screen_, false);
        FadeCanvasGroup(uiObjects.about.Screen_, false);
        FadeCanvasGroup(uiObjects.dailyRewards.Screen_, false);
        FadeCanvasGroup(uiObjects.dailyRewardsMenu.Screen_, false);
    }
    public void passPlay()
    {
        FadeCanvasGroup(uiObjects.HomeScreen.Screen_, false);
        FadeCanvasGroup(uiObjects.playPart.Screen_, false);
        FadeCanvasGroup(uiObjects.passPlaySubmenu.Screen_, true);
        FadeCanvasGroup(uiObjects.timerSubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.settings.Screen_, false);
        FadeCanvasGroup(uiObjects.about.Screen_, false);
        FadeCanvasGroup(uiObjects.dailyRewards.Screen_, false);
        FadeCanvasGroup(uiObjects.dailyRewardsMenu.Screen_, false);
    }
    public void backToPlay()
    {
        FadeCanvasGroup(uiObjects.HomeScreen.Screen_, false);
        FadeCanvasGroup(uiObjects.playPart.Screen_, true);
        FadeCanvasGroup(uiObjects.passPlaySubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.timerSubmenu.Screen_, false);
        FadeCanvasGroup(uiObjects.settings.Screen_, false);
        FadeCanvasGroup(uiObjects.about.Screen_, false);
        FadeCanvasGroup(uiObjects.dailyRewards.Screen_, false);
        FadeCanvasGroup(uiObjects.dailyRewardsMenu.Screen_, false);
    }
    public void street()
    {
        SceneManager.LoadScene(1);
    }
    public void streetPVP()
    {
        SceneManager.LoadScene(5);
    }
    public void rooftop()
    {
        SceneManager.LoadScene(2);
    }
    public void rooftopPVP()
    {
        SceneManager.LoadScene(6);
    }
    public void stadium()
    {
        SceneManager.LoadScene(3);
    }
    public void stadiumPVP()
    {
        SceneManager.LoadScene(7);
    }
    public void lawn()
    {
        SceneManager.LoadScene(4);
    }
    public void lawnPVP()
    {
        SceneManager.LoadScene(8);
    }
}

    [System.Serializable]
    public class UIObjects
    {
        public HomeScreen HomeScreen;
        public PlayPart playPart;
        public TimerSubmenu timerSubmenu;
        public PassPlaySubmenu passPlaySubmenu;
        public Settings settings;
        public About about;
        public DailyRewards dailyRewards;
        public DailyRewardsMenu dailyRewardsMenu;

    }




    //Menu
    [System.Serializable]
    public class HomeScreen
    {
        public CanvasGroup Screen_;
    }



//Play-Sub
[System.Serializable]
public class PlayPart
{
    public CanvasGroup Screen_;
    public TextMeshProUGUI coins_text;

    }



//Maps- Timer
[System.Serializable]
public class TimerSubmenu
{
    public CanvasGroup Screen_;
    public TextMeshProUGUI coins_text;
    }



//Maps - PassPlay
[System.Serializable]
public class PassPlaySubmenu
{
    public CanvasGroup Screen_;
    public TextMeshProUGUI coins_text;
    }


//Settings
[System.Serializable]
public class Settings
{
    public CanvasGroup Screen_;
    public GameObject Sound, Music;
     public TextMeshProUGUI coins_text; 

    }


//Info
[System.Serializable]
public class About
{
    public CanvasGroup Screen_;
        public TextMeshProUGUI coins_text;
    }


    //DailyRewards
    [System.Serializable]
    public class DailyRewards
    {
        public CanvasGroup Screen_;
    }


    //DailyRewardsMenu
    [System.Serializable]
    public class DailyRewardsMenu
    {
        public CanvasGroup Screen_;
        public TextMeshProUGUI day_text;
        public TextMeshProUGUI coins_text;
    }


