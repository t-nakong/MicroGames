using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

public class Timer : MonoBehaviour
{
    UnityEvent timeEnd_Event;
    public Text Countdown_Text;
    public float Duration;

    private float _startTime;
    private bool _timerOn;

    public void StartTimer()
    {
        _startTime = Time.time;
        _timerOn = true;
    }

    public float GetTimeRemaining()
    {
        return Time.time - _startTime;
    }

    public void UpdateCountdown()
    {
        float timeRemaining = Mathf.Clamp(GetTimeRemaining(), 0, 60);
        int minutes = Mathf.FloorToInt(timeRemaining / 60F);
        int seconds = Mathf.FloorToInt(timeRemaining - minutes * 60);

        Countdown_Text.text = string.Format("{0:0}:{1:00}", minutes, seconds);
    }

    public virtual void TimerEnded()
    {
        Debug.Log("Timer Ended");
    }

    private void CheckTimerEnd()
    {
        if (GetTimeRemaining() >= Duration)
        {
            _timerOn = false;
            timeEnd_Event.Invoke();
        }
    }
    
    // Start is called before the first frame update
    void Start()
    {
        _startTime = 0f;
        _timerOn = false;

        if (timeEnd_Event == null)
            timeEnd_Event = new UnityEvent();

        timeEnd_Event.AddListener(TimerEnded);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateCountdown();
        CheckTimerEnd();
    }
}
