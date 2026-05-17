using System;

namespace BigBalls.Infrastructure
{
    public interface ISceneLoader
    {
        void LoadSceneImmediately(string name, Action onLoaded);
        void LoadSceneWithLoadingScreen(string name, Action onLoaded);
    }
}