using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement; // Make sure to include this for scene management
 
public class AudioControleer : MonoBehaviour
{
    
    [SerializeField] public static bool isMusicOn = true;
    [SerializeField] public static bool isSoundOn = true;
    public static AudioControleer instance;
 
    public Sprite[] sprites;
    public AudioSource musicSource;
    public Image musicButtonImage;
    public Image soundButtonImage;
 
    // Start is called before the first frame update
    private const string musicVolKey = "MusicVol";
    private const string soundVolKey = "SoundVol";
    public Menu menu;

    void Awake()
    {
        if (instance == null)
        {
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += OnSceneLoaded;
            instance = this;
            gameObject.GetComponent<AudioSource>().Play();
        }
        else
        {
            Destroy(gameObject); // Ensure only one instance exists
        }
        
    }
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {

        // Load the saved volume settings
        //musicSource.volume = PlayerPrefs.GetInt(musicVolKey, 1);
    }
    
 
    public void toggleMusic()
    {
        isMusicOn = !isMusicOn;
        if (isMusicOn)
        {
            Menu.Instance.MusicOn();
            musicSource.volume = 1f;
            PlayerPrefs.SetInt(musicVolKey, 1);
            PlayerPrefs.Save();
        }
        else
        {
            Menu.Instance.MusicOff();
            musicSource.volume = 0f;
            PlayerPrefs.SetInt(musicVolKey, 0);
            PlayerPrefs.Save();
        }
        // Save the volume setting
    }

    public void toggleSound()
    {
        isSoundOn = !isSoundOn;
        if (isSoundOn)
        {
            soundButtonImage.sprite = sprites[2];
            //isSound = true;
        }
        else
        {
            soundButtonImage.sprite = sprites[3];
           // isSound = false;
        }
 
        // if (isSound)
        // {
        //    // musicSource.Play();
        // }
        // else
        // {
        //    // musicSource.Stop();
        // }
    }
 
 
}