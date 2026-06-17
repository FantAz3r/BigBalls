public class LootMediator : ILootMediator
{
    private IWalletModel _walletModel;
    
    public LootMediator(IWalletModel walletModel)
    {
        _walletModel = walletModel;
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
