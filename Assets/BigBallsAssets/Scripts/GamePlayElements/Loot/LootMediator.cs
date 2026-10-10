using BigBalls.GameplayObjects;
using BigBalls.Services;

public class LootMediator : ILootMediator
{
    private readonly IPlayerExperience _playerExperience;
    private readonly IEntityRepository _entityRepository;
    private readonly IWindowService _windowService;
    private IWalletModel _walletModel;
    private IPlayerProvider _playerProvider;

    public LootMediator (IWalletModel walletModel, IPlayerProvider playerProvider, IPlayerExperience playerExperience, IEntityRepository entityRepository, IWindowService windowService)
    {
        _walletModel = walletModel;
        _playerProvider = playerProvider;
        _playerExperience = playerExperience;
        _entityRepository = entityRepository;
        _windowService = windowService;
    }

    public void RegisterLoot (Loot loot)
    {
        switch (loot.Type)
        {
            case LootType.Coin:
                _walletModel.AddCoins(loot.Value);
                break;

            case LootType.HealthPotion:
                StatHolder statHolder = _entityRepository.GetStatHolder(_playerProvider.Player);
                statHolder[StatType.Health].AddCurrentValue(loot.Value);
                break;

            case LootType.Experience:
                _playerExperience.AddExperience(loot.Value);
                break;

            case LootType.Ball:
                _windowService.Open<SplitingPanel>();
                break;
        }
    }
}
