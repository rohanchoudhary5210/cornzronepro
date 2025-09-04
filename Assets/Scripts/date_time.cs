
using UnityEngine;
using System;
using Newtonsoft.Json;
using System.IO;
using UnityEngine.UI;
 
 
 
public class date_time : MonoBehaviour
{
    // string filepath = Application.persistentDataPath + "/playerData.json";
    // Start is called before the first frame update
    public Button[] buttons;
    public static date_time Instance { get; private set; }
    void Awake()
    {
        Instance = this;
        string filepath = Application.persistentDataPath + "/playerData.json";
        Debug.Log(filepath);
        if (!File.Exists(filepath))
        {
            playerData data = new playerData()
            {
                daysCount = 0,
                lastDate = DateTime.Now.Date,
                count = 0,
                alreadycounted = false,
                days = new int[] { 5, 10, 15, 20, 25, 30, 35 }
            };
            File.WriteAllText(filepath, JsonConvert.SerializeObject(data));
        }
        currentPlayerData = JsonConvert.DeserializeObject<playerData>(File.ReadAllText(filepath));
        CalculateRewards(currentPlayerData, filepath);
    }
    public void CalculateRewards(playerData data, string filepath)
    {
        if ((DateTime.UtcNow.Date - data.lastDate.Date).TotalDays > 1)
        {
            data.daysCount = 0;
            data.lastDate = DateTime.UtcNow.Date;
            data.alreadycounted = false; // Reset the already counted flag
            data.count = 1;
        }
        else if ((DateTime.UtcNow.Date - data.lastDate.Date).TotalDays == 1)
        {
            data.daysCount += 1;
            data.lastDate = DateTime.UtcNow.Date;
            data.alreadycounted = false; // Reset the already counted flag
            data.count = 1;
        }
        else
        {
 
            data.lastDate = DateTime.UtcNow.Date;
            data.count = 0;
        }
 
        // allotRewards(data.daysCount+1);
        File.WriteAllText(filepath, JsonConvert.SerializeObject(data, Formatting.Indented));
        allotRewards(data, filepath);
    }
    public void allotRewards(playerData data, string filepath)
    {
        if (!data.alreadycounted && (DateTime.UtcNow.Date - data.lastDate.Date).TotalDays == 0)
        {
            buttons[data.daysCount % 7].interactable = true;
            buttons[data.daysCount % 7].onClick.AddListener(() =>
            {
                playerData updatedData = JsonConvert.DeserializeObject<playerData>(File.ReadAllText(filepath));
                CoinManager.Instance.AddCoins(data.days[data.daysCount % 7]); // Give reward
                updatedData.alreadycounted = true;
                buttons[updatedData.daysCount % 7].interactable = false;
                File.WriteAllText(filepath, JsonConvert.SerializeObject(updatedData));
                data = updatedData;
                // loadData(data, filepath);
            });
        }
        // else if (data.alreadycounted)
        else
        {
            buttons[data.daysCount % 7].interactable = false;
            //loadData(data, filepath);
        }
        File.WriteAllText(filepath, JsonConvert.SerializeObject(data, Formatting.Indented));
       
    }
    private playerData currentPlayerData;
    }
[System.Serializable]
public class playerData
{
    // public int coins;
    public int daysCount;
    public DateTime lastDate;
    // public int p1;
    // public int p2;
    // public int p3;
    public int count;
    public bool alreadycounted;
    public int[] days;
    
}