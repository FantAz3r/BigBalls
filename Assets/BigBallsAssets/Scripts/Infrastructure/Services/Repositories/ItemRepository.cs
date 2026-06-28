using System;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.Saves;
using BigBalls.Services;

public abstract class ItemRepository<TKey, TModel, TConfig, TSaveData> : IResetble
    where TKey : Enum
    where TModel : ICardModel
    where TConfig : ItemConfig
    where TSaveData : CardSaveData
{
    protected readonly IResourceLoader ResourceLoader;
    protected readonly Dictionary<TKey, TModel> Models = new();

    private readonly ISaveService _saveService;
    private readonly Dictionary<TKey, CardSaveData> _saveDatas = new();

    protected GameProgress GameProgress;

    protected ItemRepository (IResourceLoader resourceLoader, ISaveService saveService)
    {
        ResourceLoader = resourceLoader;
        _saveService = saveService;
        GameProgress = saveService.GameProgress;

        saveService.RegisterResetable(this);
    }

    public IReadOnlyDictionary<TKey, TModel> AllModels => Models;

    public void Initialize ()
    {
        Models.Clear();
        _saveDatas.Clear();

        var configs = LoadConfigs();

        foreach (var config in configs)
        {
            _saveDatas[config.Key] = new CardSaveData((int) (object) config.Key, false, 0);
        }

        LoadDataFromSave();
        CreateModels(configs);
    }

    public void Reset () => Initialize();

    public void Save ()
    {
        var saves = new List<TSaveData>();

        foreach (var model in Models.Values)
        {
            TSaveData save = (TSaveData) model.CreateSaveData();
            saves.Add(save);
        }

        SaveGameProgress(saves);
    }

    protected abstract Dictionary<TKey, TConfig> LoadConfigs ();

    protected abstract TModel CreateModel (TKey type, TConfig config, CardSaveData saveData);

    protected abstract List<TSaveData> GetSaveDataFromProgress ();

    protected abstract void SaveGameProgress (List<TSaveData> saveData);

    private void LoadDataFromSave ()
    {
        var savedItems = GetSaveDataFromProgress();

        if (savedItems == null || savedItems.Count == 0)
            return;

        foreach (var savedItem in savedItems)
        {
            var key = (TKey) (object) savedItem.Id;

            if (_saveDatas.ContainsKey(key))
            {
                _saveDatas[key] = savedItem;
            }
        }
    }

    private void CreateModels (Dictionary<TKey, TConfig> configs)
    {
        foreach (var kvp in _saveDatas)
        {
            TKey type = kvp.Key;
            CardSaveData saveData = kvp.Value;

            if (configs.TryGetValue(type, out var config))
            {
                TModel model = CreateModel(type, config, saveData);
                model.InitFromData(saveData);
                Models[type] = model;
            }
        }
    }
}
