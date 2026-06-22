using System;
using System.Collections.Generic;
using BigBalls.Configs;
using BigBalls.Saves;
using BigBalls.Services;
using BigBalls.StaticData;

public abstract class ItemRepository<TKey, TModel, TConfig, TSaveData>
    where TKey : Enum
    where TModel : ICard
    where TConfig : ItemConfig
    where TSaveData : CardSaveData
{ 
    protected readonly ISaveService SaveService;
    protected readonly IResourceLoader ResourceLoader;
    protected readonly GameProgress GameProgress;

    protected readonly Dictionary<TKey, TModel> Models = new();

    private readonly Dictionary<TKey, CardSaveData> _saveDatas = new();

    protected ItemRepository (IResourceLoader resourceLoader, ISaveService saveService)
    {
        ResourceLoader = resourceLoader;
        SaveService = saveService;
        GameProgress = saveService.GameProgress;
    }

    public IReadOnlyDictionary<TKey, TModel> AllModels => Models;

    public TModel GetModel (TKey type)
    {
        Models.TryGetValue(type, out var model);
        return model;
    }

    //public void AddOrUpdateModel (TModel model) => Models[model.Config.Type] = model;

    public void Initialize ()
    {
        var configs = LoadConfigs();
        foreach (var config in configs)
        {
            _saveDatas.Add(config.Key, new CardSaveData((int) (object) config.Key, false, 0));
        }

        LoadDataFromSave();
        CreateModels(configs);
    }

    public void Save ()
    {
        var saves = new List<TSaveData>();

        foreach (var model in Models.Values)
        {
            saves.Add(model.CraeateSaveData() as TSaveData);
        }

        UpdateGameProgress(saves);
        SaveService.Save(GameProgress);
    }

    protected abstract Dictionary<TKey, TConfig> LoadConfigs ();

    protected abstract TModel CreateModel (TKey type, TConfig config, CardSaveData saveData);

    protected abstract List<TSaveData> GetSaveDataFromProgress ();

    protected abstract void UpdateGameProgress (List<TSaveData> saveData);

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
