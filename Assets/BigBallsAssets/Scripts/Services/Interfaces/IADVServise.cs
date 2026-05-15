using System;

namespace BigBalls.Services
{
    public interface IADVServise : IService
    {
        void TryShowRewardADV(string rewardID, Action callback);
        bool CanShowRewardADV(string rewardID);
        void TryShowInterstitialADV(string nextLevel);
    }
}
