namespace BigBalls.Services
{
    public interface IDamageService
    {
        void ApplyDamage(Ball ball, int id, float damage);
    }
}