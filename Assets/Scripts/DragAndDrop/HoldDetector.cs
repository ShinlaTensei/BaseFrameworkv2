using System;
using System.Collections;
using System.Collections.Generic;
using Base.Helper;
using Base.Logging;
using Base.Services;
using UnityEngine;
using UnityEngine.EventSystems;

public class HoldDetector : BaseMono, IPointerDownHandler, IPointerUpHandler
{
    /// <summary>
    /// The time determine a press action turn to hold
    /// </summary>
    [SerializeField] private float m_timePressToHold = .25f;
    private float m_holdTime;
    private bool m_isHold;
    private bool m_isPress;

    public Signal<bool> HoldEvent = new Signal<bool>();


    private void LateUpdate()
    {
        if (m_isPress)
        {
            m_holdTime -= Time.smoothDeltaTime;
            if (m_holdTime <= 0)
            {
                m_holdTime = 0;
                m_isHold = true;
                m_isPress = false;
            }
        }

        if (m_isHold)
        {
            HoldEvent.Dispatch(true);
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        m_holdTime = m_timePressToHold;
        m_isPress = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        m_isPress = false;
        m_isHold = false;
        m_holdTime = m_timePressToHold;
        HoldEvent.Dispatch(false);
    }
}
