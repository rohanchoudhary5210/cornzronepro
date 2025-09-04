using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CoinManager : MonoBehaviour
{
    public static CoinManager Instance { get; private set; }
    private static int _playerCoins = 0;
    public int tutorial;
    public static int PlayerCoins => _playerCoins;
    public const string COINS_KEY = "PlayerTotalCoins";
    public const string TUTORIAL_KEY = "TutorialCompleted";

    public void TutorialCompleted()
    {
        tutorial = 1;
        PlayerPrefs.SetInt(TUTORIAL_KEY, tutorial);
        PlayerPrefs.Save();
    }
    public void LoadTutorialStatus()
    {
        tutorial = PlayerPrefs.GetInt(TUTORIAL_KEY, 0);
        Debug.Log("Tutorial status loaded from PlayerPrefs. Value is: " + tutorial);
    }

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // If an instance
            //  already exists, destroy this new one and stop.
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadTutorialStatus();
        UpdateUI();
    }
    void Start()
    {
        tutorial = 0;
        LoadCoins();
    }
    public void LoadCoins()
    {
        _playerCoins = PlayerPrefs.GetInt(COINS_KEY, 0);
        UpdateUI();
    }
    public void SaveCoins()
    {
        PlayerPrefs.SetInt(COINS_KEY, _playerCoins);
        PlayerPrefs.Save();
    }
    public void AddCoins(int amountToAdd)
    {
        _playerCoins += amountToAdd;
        SaveCoins();
        UpdateUI();
    }
    public void UseCoins(int amountToUse)
    {
        _playerCoins -= amountToUse;
        SaveCoins();
        UpdateUI();
    }
    public void UpdateUI()
    {
        Menu.Instance.uiObjects.playPart.coins_text.text = _playerCoins.ToString();
        Menu.Instance.uiObjects.timerSubmenu.coins_text.text = _playerCoins.ToString();
        Menu.Instance.uiObjects.passPlaySubmenu.coins_text.text = _playerCoins.ToString();
        Menu.Instance.uiObjects.settings.coins_text.text = _playerCoins.ToString();
        Menu.Instance.uiObjects.about.coins_text.text = _playerCoins.ToString();
    }
    void Update()
    {

        UpdateUI();
    }   
    public int Coins
    {
        get { return _playerCoins; }
    }
}
