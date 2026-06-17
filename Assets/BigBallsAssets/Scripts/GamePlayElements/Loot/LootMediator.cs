using BigBalls.GameplayObjects;
using BigBalls.Services;

public class LootMediator : ILootMediator
{
    private IWalletModel _walletModel;
    private Stat _health;
    private IPlayerProvider _iPlayerProvider;
    
    public LootMediator(IWalletModel walletModel, IPlayerProvider playerProvider )
    {
        _walletModel = walletModel;
        _iPlayerProvider = playerProvider;
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
                // _experienceModel.AddExperience(loot.Value);
                break;

            case LootType.Ball:
                // _ballModel.AddBall(loot.BallData);
                break;
        }
    }
}
