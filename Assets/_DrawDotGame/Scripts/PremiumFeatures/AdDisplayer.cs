﻿using UnityEngine;
using System.Collections;
using System;

#if EASY_MOBILE
using EasyMobile;
#endif

namespace DrawDotGame
{
    public class AdDisplayer : MonoBehaviour
    {
        #if EASY_MOBILE
        public static AdDisplayer Instance { get; private set; }

        public static event System.Action CompleteRewardedAdToRecoverLostGame;
        public static event System.Action CompleteRewardedAdToEarnCoins;

        [Header("Banner Ad Display Config")]
        [Tooltip("Whether or not to show banner ad")]
        public bool showBannerAd = true;
        public BannerAdPosition bannerAdPosition = BannerAdPosition.Bottom;

        [Header("Interstitial Ad Display Config")]
        [Tooltip("Whether or not to show interstitial ad")]
        public bool showInterstitialAd = true;
        [Tooltip("Show interstitial ad every [how many] games")]
        public int gamesPerInterstitial = 3;
        [Tooltip("How many seconds after game over that interstitial ad is shown")]
        public float showInterstitialDelay = 2f;

        [Header("Rewarded Ad Display Config")]
        [Tooltip("Check to allow watching ad to earn hearts")]
        public bool useRewardedAds = true;
        [Tooltip("How many hearts the user earns after watching a rewarded ad")]
        public int rewardedHearts = 3;
        [Tooltip("Minimum time (minutes) to wait until serving the next rewarded ad")]
        public float rewardedAdLimitTime = 3;

        private static int gameCount = 0;
        private const string LAST_REWARDED_AD_TIME_PPK = "SGLIB_LAST_REWARDED_AD_TIME";

        void OnEnable()
        {
            GameManager.GameEnded += OnGameEnded;
        }

        void OnDisable()
        {
            GameManager.GameEnded -= OnGameEnded;
        }

        void Awake()
        {
            if (Instance)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        void Start()
        {
            // Show banner ad
            if (showBannerAd && !Advertising.IsAdRemoved())
            {
                Advertising.ShowBannerAd(bannerAdPosition);
            }
        }

        void OnGameEnded(bool isWin, bool isFirstWin)
        {       
            // Show interstitial ad
            if (showInterstitialAd && !Advertising.IsAdRemoved())
            {
                gameCount++;

                if (gameCount >= gamesPerInterstitial)
                {
                    if (Advertising.IsInterstitialAdReady())
                    {
                        // Show default ad after some optional delay
                        StartCoroutine(ShowInterstitial(showInterstitialDelay));

                        // Reset game count
                        gameCount = 0;
                    }
                }
            }
        }

        IEnumerator ShowInterstitial(float delay = 0f)
        {        
            if (delay > 0)
                yield return new WaitForSeconds(delay);

            Advertising.ShowInterstitialAd();
        }

        public bool IsRewardedAdLimitTimePast()
        {
            DateTime epoch = new DateTime(1970, 1, 1);
            DateTime lastTime = Utilities.GetTime(LAST_REWARDED_AD_TIME_PPK, epoch);
            TimeSpan timePast = DateTime.Now.Subtract(lastTime);
            return timePast.Minutes >= rewardedAdLimitTime;
        }

        public bool CanShowRewardedAd()
        {
            return Advertising.IsRewardedAdReady() && IsRewardedAdLimitTimePast();
        }

        public void ShowRewardedAdToRecoverLostGame()
        {
            if (CanShowRewardedAd())
            {
                Advertising.RewardedAdCompleted += OnCompleteRewardedAdToRecoverLostGame;
                ShowRewardedAd();
            }
        }

        void OnCompleteRewardedAdToRecoverLostGame(RewardedAdNetwork adNetwork, AdLocation location)
        {
            // Unsubscribe
            Advertising.RewardedAdCompleted -= OnCompleteRewardedAdToRecoverLostGame;

            // Fire event
            if (CompleteRewardedAdToRecoverLostGame != null)
            {
                CompleteRewardedAdToRecoverLostGame();
            }
        }

        public void ShowRewardedAdToEarnCoins()
        {
            if (CanShowRewardedAd())
            {
                Advertising.RewardedAdCompleted += OnCompleteRewardedAdToEarnCoins;
                ShowRewardedAd();
            }
        }

        void ShowRewardedAd()
        {
            Advertising.ShowRewardedAd();
            Utilities.StoreTime(LAST_REWARDED_AD_TIME_PPK, DateTime.Now); 
        }

        void OnCompleteRewardedAdToEarnCoins(RewardedAdNetwork adNetwork, AdLocation location)
        {
            // Unsubscribe
            Advertising.RewardedAdCompleted -= OnCompleteRewardedAdToEarnCoins;

            // Fire event
            if (CompleteRewardedAdToEarnCoins != null)
            {
                CompleteRewardedAdToEarnCoins();
            }
        }



        #endif
    }
}
