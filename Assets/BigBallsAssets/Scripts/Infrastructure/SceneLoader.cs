using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using BigBalls.Services;

namespace BigBalls.Infrastructure
{
    public class SceneLoader : ISceneLoader
    {
        private readonly ICoroutineRunner _coroutineRunner;
        private float _minLoadTime = 1;
        private float _maxLoadTime = 3;

        public SceneLoader(ICoroutineRunner coroutineRunner)
        {
            _coroutineRunner = coroutineRunner;
        }

        public void LoadSceneImmediately(string name, System.Action onLoaded)
        {
            _coroutineRunner.StartCoroutine(LoadScene(name, onLoaded, false));
        }

        public void LoadSceneWithLoadingScreen(string name, System.Action onLoaded)
        {
            _coroutineRunner.StartCoroutine(LoadScene(name, onLoaded, true));
        }

        private IEnumerator LoadScene(string nextScene, System.Action onLoaded, bool hasLoading)
        {
            AsyncOperation asyncLoad = null;
            Scene initialScene = SceneManager.GetActiveScene();
            Scene loadingScene = default;

            if (hasLoading)
            {
                string loadingName = LevelID.LoadScene.ToString();

                if (loadingName != initialScene.name)
                {
                    asyncLoad = SceneManager.LoadSceneAsync(loadingName, LoadSceneMode.Additive);
                }

                while (asyncLoad != null && asyncLoad.isDone == false)
                    yield return null;

                loadingScene = SceneManager.GetSceneByName(loadingName);

                if (loadingScene.IsValid() && loadingScene != initialScene)
                {
                    SceneManager.SetActiveScene(loadingScene);
                }

                yield return new WaitForSecondsRealtime(Random.Range(_minLoadTime, _maxLoadTime));
            }

            asyncLoad = SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive);

            while (asyncLoad != null && !asyncLoad.isDone)
                yield return null;

            Scene newScene = SceneManager.GetSceneByName(nextScene);

            if (newScene.IsValid())
            {
                SceneManager.SetActiveScene(newScene);

                if (initialScene.IsValid())
                {
                    yield return SceneManager.UnloadSceneAsync(initialScene);
                }

                if (hasLoading && loadingScene.IsValid() && loadingScene != initialScene)
                {
                    yield return SceneManager.UnloadSceneAsync(loadingScene);
                }
            }

            onLoaded?.Invoke();
        }
    }
}
