using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : ISceneLoader
{
    private readonly ICoroutineRunner _coroutineRunner;
    private float _minLoadTime = 1;
    private float _maxLoadTime = 3;

    public SceneLoader(ICoroutineRunner coroutineRunner)
    {
        _coroutineRunner = coroutineRunner;
    }

    public void Load(string name, System.Action onLoaded, bool hasLoading)
    {
        _coroutineRunner.StartCoroutine(LoadScene(name, onLoaded, hasLoading));
    }

    private IEnumerator LoadScene(string nextScene, System.Action onLoaded, bool hasLoading)
    {
        if (SceneManager.GetSceneByName(LevelID.LoadScene.ToString()).isLoaded == false)
        {
            hasLoading = false;

        }

        AsyncOperation asyncLoad = null;

        Scene currentActiveScene = SceneManager.GetActiveScene();
        Scene loadingScene = default;


        if (hasLoading)
        {
            asyncLoad = SceneManager.LoadSceneAsync(LevelID.LoadScene.ToString(), LoadSceneMode.Additive);
            yield return asyncLoad;

            loadingScene = SceneManager.GetSceneByName(LevelID.LoadScene.ToString());
            SceneManager.SetActiveScene(loadingScene);

            yield return new WaitForSecondsRealtime(Random.Range(_minLoadTime, _maxLoadTime));
        }

        asyncLoad = SceneManager.LoadSceneAsync(nextScene, LoadSceneMode.Additive);
        yield return asyncLoad;

        Scene newScene = SceneManager.GetSceneByName(nextScene);

        if (newScene.IsValid())
        {
            SceneManager.SetActiveScene(newScene);
            asyncLoad = SceneManager.UnloadSceneAsync(currentActiveScene);
            yield return asyncLoad;
        }

        if (loadingScene != default && loadingScene.IsValid())
        {
            asyncLoad = SceneManager.UnloadSceneAsync(loadingScene);
            yield return asyncLoad;
        }

        onLoaded?.Invoke();
    }
}
