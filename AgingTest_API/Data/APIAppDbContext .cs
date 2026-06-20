//using AgingTest_API.Models;
using AgingTest_API.Models;
using Microsoft.EntityFrameworkCore;
//using static AgingTest_API.Models.UserModel;
using System.Collections.Generic;

namespace AgingTest_API.Data
{
    public class APIAppDbContext : DbContext
    {
        public APIAppDbContext(DbContextOptions<APIAppDbContext> options)
            : base(options)
        {
        }
        public DbSet<AgingDeviceAPIModel> tb_AgingDeviceAPI { get; set; }
        public DbSet<IoTModuleAPIModel> tb_IoTModuleAPI { get; set; }

        public DbSet<AgingProcessAPIModel> tb_AgingProcessAPI { get; set; }

        public DbSet<TestConfigurationAPIModel> tb_TestConfigurationAPI { get; set; }

        public DbSet<AgingLogAPIModel> tb_AgingLogAPI { get; set; }
    }
}
