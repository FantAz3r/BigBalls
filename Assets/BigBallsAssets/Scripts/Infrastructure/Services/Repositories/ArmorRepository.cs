using BigBalls.Configs;
using BigBalls.Saves;
using BigBalls.Services;
using System.Collections.Generic;

public class ArmorRepository : ItemRepository<ItemType, ArmorModel, ArmorConfig, ArmorSaveData>
{
    private readonly Dictionary<ItemType, ArmorConfig> _armorData;
    public ArmorRepository(IResourceLoader resourceLoader, ISaveService saveService) : base(resourceLoader, saveService)
    {
        _armorData = resourceLoader.Load<CardsData>().Armors;
        Initialize();
    }

    protected override ArmorModel CreateModel(ItemType type, ArmorConfig config, CardSaveData saveData)
       => new ArmorModel((int)type, config, saveData.NoneGameLevel, saveData.ItemExp);

    protected override List<ArmorSaveData> GetSaveDataFromProgress() => GameProgress.Armors;


    protected override Dictionary<ItemType, ArmorConfig> LoadConfigs() => _armorData;

    protected override void SaveGameProgress(List<ArmorSaveData> saveData) => GameProgress.Armors = saveData;
}
