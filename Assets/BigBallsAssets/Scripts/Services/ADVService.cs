using System;
using System.Collections.Generic;
using UnityEngine;
//using YG;
using BigBalls.Infrastructure;

namespace BigBalls.Services
{
    public class ADVService : IADVServise
    {
        private const float CooldownSeconds = 60f;
        private Dictionary<string, float> _lastRunTimes = new Dictionary<string, float>();
        //private ISoundService _soundService;

        //public ADVService(ISoundService soundService)
        //{
        //    _soundService = soundService;
        //}

        public void TryShowRewardADV(string rewardID, Action callback)
        {
            float currentTime = Time.time;

            if (_lastRunTimes.TryGetValue(rewardID, out float lastRunTime))
            {
                if (currentTime - lastRunTime < CooldownSeconds)
                {
                    return;
                }
            }

            // _lastRunTimes[rewardID] = currentTime;
            // _soundService.StopAll();
            //
            // YG2.onCloseAnyAdv += OnCloseADV;
            // YG2.RewardedAdvShow(rewardID, callback);
        }

        public bool CanShowRewardADV(string rewardID)
        {
            float currentTime = Time.time;

            if (_lastRunTimes.TryGetValue(rewardID, out float lastRunTime))
            {
                if (currentTime - lastRunTime < CooldownSeconds)
                {
                    return false;
                }
            }

            return true;
        }

        public void TryShowInterstitialADV(string nextLevel)
        {
            if (nextLevel != LevelID.MainMenu.ToString() &&
                nextLevel != LevelID.None.ToString() &&
                nextLevel != LevelID.LoadScene.ToString())
            {
                //YG2.InterstitialAdvShow();
                //YG2.onCloseAnyAdv += OnCloseADV;
                // _soundService.StopAll();
            }
        }

        private void OnCloseADV()
        {
            //_soundService.ContinueAll();
            //YG2.onCloseAnyAdv -= OnCloseADV;
        }
    }
}
