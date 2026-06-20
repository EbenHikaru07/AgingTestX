using AgingTest.Models;
using Microsoft.EntityFrameworkCore;
using static AgingTest.Models.UserModel;

namespace AgingTest.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options)
            : base(options)
        {
        }

        public DbSet<UserModel> tb_users { get; set; }
        public DbSet<NotificationModel> Notifications { get; set; }
        public DbSet<DataLampu> DataLampu { get; set; }
        public DbSet<AgingDeviceModel> AgingDevices { get; set; }
        public DbSet<IoTModuleModel> IoTModules { get; set; }
        public DbSet<AgingProcessModel> AgingProcess { get; set; }
        public DbSet<TestConfigurationModel> TestConfiguration { get; set; }
        public DbSet<AgingLogModel> AgingLog { get; set; }
        public DbSet<AgingAbnormalModel> AgingAbnormal { get; set; }

    }
}
