public interface ISceneLoader
{
    void Load(string name, System.Action onLoaded, bool hasLoading);
}