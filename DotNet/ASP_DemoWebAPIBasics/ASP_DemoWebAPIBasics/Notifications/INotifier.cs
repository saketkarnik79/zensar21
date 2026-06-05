namespace ASP_DemoWebAPIBasics.Notifications
{
    public interface INotifier
    {
        void Send(string message);
    }
}
