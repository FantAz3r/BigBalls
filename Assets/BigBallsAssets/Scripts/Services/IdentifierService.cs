using BigBalls.Factories;

namespace BigBalls.Services
{
    public class IdentifierService : IIdentifierService
    {
        private int _id;

        public int ID { get { return ++_id; } }
    }
}
