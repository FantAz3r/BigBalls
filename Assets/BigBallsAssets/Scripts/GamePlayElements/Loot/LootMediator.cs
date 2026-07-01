using BigBalls.GameplayObjects;
using BigBalls.Services;

public class LootMediator : ILootMediator
{
    private IWalletModel _walletModel;
    private IPlayerProvider _playerProvider;
    private readonly IPlayerExperience _playerExperience;
    private readonly IEntityRepository _entityRepository;

    public LootMediator(IWalletModel walletModel, IPlayerProvider playerProvider, IPlayerExperience playerExperience, IEntityRepository entityRepository)
    {
        _walletModel = walletModel;
        _playerProvider = playerProvider;
        _playerExperience = playerExperience;
        _entityRepository = entityRepository;
    }

    public void RegisterLoot(Loot loot)
    {
        switch (loot.Type)
        {
            case LootType.Coin:
                _walletModel.AddCoins(loot.Value);
                break;

            case LootType.HealthPotion:
                StatHolder statHolder = _entityRepository.Get(_playerProvider.Player);
                statHolder[StatType.Health].AddCurrentValue(loot.Value);
                break;

            case LootType.Experience:
                _playerExperience.AddExperience(loot.Value);
                break;

            case LootType.Ball:
                // _ballModel.AddBall(loot.BallData);
                break;
        }
    }
}
