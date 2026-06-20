using AgingTest.Data;
using AgingTest.Hubs;
using AgingTest.Models;
using Microsoft.AspNetCore.SignalR;

namespace AgingTest.Services
{
    public class NotificationService : INotificationService
    {
        private readonly AppDbContext _context;
        private readonly IHubContext<NotificationHub> _hubContext;

        public NotificationService(
            AppDbContext context,
            IHubContext<NotificationHub> hubContext)
        {
            _context = context;
            _hubContext = hubContext;
        }

        public async Task SendAsync(int userId, string title, string message)
        {
            var notif = new NotificationModel
            {
                title = title,
                message = message,
                id_user = userId,
                is_read = false,
                created_at = DateTime.Now
            };

            _context.Notifications.Add(notif);
            await _context.SaveChangesAsync();

            await _hubContext.Clients.Group(userId.ToString())
                .SendAsync("ReceiveNotification", new
                {
                    id = notif.id_notif,
                    title = notif.title,
                    message = notif.message,
                    createdAt = notif.created_at.ToString("dd MMM yyyy HH:mm:ss")
                });
        }

        public async Task SendToAllAsync(string title, string message)
        {
            var users = _context.tb_users.ToList();

            foreach (var user in users)
            {
                var notif = new NotificationModel
                {
                    title = title,
                    message = message,
                    id_user = user.id_user,
                    is_read = false,
                    created_at = DateTime.Now
                };

                _context.Notifications.Add(notif);
            }

            await _context.SaveChangesAsync();

            await _hubContext.Clients.All
                .SendAsync("ReceiveNotification", new
                {
                    title,
                    message,
                    createdAt = DateTime.Now.ToString("dd MMM yyyy HH:mm:ss")
                });
        }
    }
}