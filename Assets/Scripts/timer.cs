using UnityEngine;
using UnityEngine.UI;

public class timer : MonoBehaviour
{
    
    public float _timeRemaining = 20f;
    public bool _isTimerRunning = false;
    [SerializeField] private UIManager uiManager;
    public static timer Instance { get; private set; }
    public Slider timerSlider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void HandleTimer()
    {
        if (_isTimerRunning)
        {
            if (_timeRemaining > 0)
            {
                _timeRemaining -= Time.deltaTime;
                timerSlider.value -= 0.05f;
                uiManager.UpdateTimerText(_timeRemaining);
            }
            else
            {
                _timeRemaining = 0;
                _isTimerRunning = false;
                uiManager.UpdateTimerText(_timeRemaining);
                uiManager.GameOver();
                //Debug.Log("Time's up!");
            }
        }
    }
}
