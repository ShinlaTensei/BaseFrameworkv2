using System;
using System.Collections;
using System.Collections.Generic;
using Base;
using Base.Helper;
using Base.Pattern;
using Base.Services;
using TMPro;
using UnityEngine;

public class GlobalConfig : MonoBehaviour
{
    [SerializeField] private TMP_Text                  m_countDownText;

    private TimerData m_timer;

    private void Awake()
    {
        ServiceLocator.Get<SoundService>().Init();
        ServiceLocator.Get<TimingService>().SetTimeInitialize(DateTime.Now);
        m_timer = new TimerDataMutable(ServiceLocator.Get<TimingService>().GetCurrentTime(), 3600);
        m_timer.SetupTimer(OnTimeRunning, OnTimeFinished);
    }

    private void OnDestroy()
    {
        m_timer.StopRunningTimer();
    }

    private void OnTimeRunning(int secondsLeft)
    {
        TimeSpan timeLeft = ServiceLocator.Get<TimingService>().TimeLeftUntil(m_timer.GetFinishDateTime());
        m_countDownText.text = string.Format("{0:00}:{1:00}", timeLeft.Minutes, timeLeft.Seconds);
    }

    private void OnTimeFinished()
    {
        
    }
}
