using BigBalls.Services;

public class LootMediator : ILootMediator
{
    private IWalletModel _walletModel;
    private IPlayerProvider _playerProvider;
    private HealHandler _healHandler;
    private readonly IPlayerExperience _playerExperience;
    
    public LootMediator(IWalletModel walletModel, IPlayerProvider playerProvider, IPlayerExperience playerExperience)
    {
        _walletModel = walletModel;
        _playerProvider = playerProvider;
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
                
                if (_healHandler == null)
                    _healHandler = _playerProvider.Player.HealHandler;
                
                _healHandler.Heal(loot.Value);
                
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
