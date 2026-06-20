namespace AgingTest.Services
{
    public interface INotificationService
    {
        Task SendAsync(int userId, string title, string message);
        Task SendToAllAsync(string title, string message);
    }
}
