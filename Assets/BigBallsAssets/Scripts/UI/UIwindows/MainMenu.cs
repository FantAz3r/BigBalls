namespace BigBalls.UI
{
    public class MainMenu : WindowBase
    {
        public override void Close()
        {
            base.Close();
            Destroy(gameObject);
        }
    }
}
