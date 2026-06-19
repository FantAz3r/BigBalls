using BigBalls.GameplayObjects;
using BigBalls.Services;

public class LootMediator : ILootMediator
{
    private IWalletModel _walletModel;
    private IPlayerProvider _iPlayerProvider;
    private readonly IPlayerExperience _playerExperience;
    
    public LootMediator(IWalletModel walletModel, IPlayerProvider playerProvider, IPlayerExperience playerExperience)
    {
        _walletModel = walletModel;
        _iPlayerProvider = playerProvider;
        _playerExperience = playerExperience;
    }

    public void RegisterLoot(Loot loot)
    {
        switch (loot.Type)
        {
            case LootType.Coin:
                _walletModel.AddCoins(loot.Value);
                break;

            case LootType.HealthPotion:
                // _healthModel.Heal(loot.Value);
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
