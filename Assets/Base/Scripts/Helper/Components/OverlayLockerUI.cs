using System;
using System.Collections;
using System.Collections.Generic;
using Base.Helper;
using Base.Logging;
using Base.Module;
using UnityEngine;
using UnityEngine.UI;

namespace Base.Helper
{
    public class OverlayLockerUI : BaseMono
    {
        [SerializeField] private Transform m_root;
        [SerializeField] private Graphic   m_blockRaycast;
        [SerializeField] private Image     m_loadingIndicator;
        [SerializeField] private float     m_checkNetworkInterval = 1f;

        private bool m_hasLocked  = false;
        private bool m_hasLoading = false;
        private bool m_hasNetwork = false;

        private float m_time = 0;

        protected override void Start()
        {
            if (m_loadingIndicator) m_loadingIndicator.gameObject.SetActive(false);
            if (m_blockRaycast) m_blockRaycast.gameObject.SetActive(false);
        }

        private void Update()
        {
            bool isLocked  = InputLocker.IsInputLocked;
            bool isLoading = InputLocker.HasLoading;

            if (m_hasLocked != isLocked)
            {
                m_hasLocked = isLocked;
                if (m_blockRaycast)
                {
                    m_blockRaycast.gameObject.SetActive(m_hasLocked);
                }
            }

            if (m_hasLoading != isLoading)
            {
                m_hasLoading = isLoading;
                if (m_loadingIndicator)
                {
                    
                }
            }

            m_time -= Time.deltaTime;
            bool hasNetwork;
            if (m_time <= 0)
            {
                m_time     = m_checkNetworkInterval;
                hasNetwork = UtilsClass.CheckForInternetConnection();
                
                if (m_hasNetwork != hasNetwork)
                {
                    m_hasNetwork = hasNetwork;
                    if (!m_hasNetwork)
                    {
                        PDebug.Error("[NETWORK ERROR] No Internet Connection");
                    }
                }
            }
        }
    } 
}

