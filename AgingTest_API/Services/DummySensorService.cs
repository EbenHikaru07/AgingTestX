using AgingTest_API.Data;
using AgingTest_API.Models;
using Microsoft.EntityFrameworkCore;

namespace AgingTest_API.Services
{
    public class DummySensorService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;

        public DummySensorService(IServiceScopeFactory scopeFactory)
        {
            _scopeFactory = scopeFactory;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                using var scope = _scopeFactory.CreateScope();

                var context = scope.ServiceProvider
                    .GetRequiredService<APIAppDbContext>();

                try
                {
                    // ===============================
                    // DUMMY IOT DEVICE
                    // ===============================

                    string mac = "AA:BB:CC";

                    var existing = await context.tb_IoTModuleAPI
                        .FirstOrDefaultAsync(x => x.mac_address == mac);

                    if (existing == null)
                    {
                        context.tb_IoTModuleAPI.Add(new IoTModuleAPIModel
                        {
                            module_name = "ESP32",
                            mac_address = mac,
                            ip_address = "192.168.1.10",
                            created_at = DateTime.Now,
                            last_heartbeat = DateTime.Now
                        });

                        await context.SaveChangesAsync();

                        Console.WriteLine("Dummy IoT inserted");
                    }
                    else
                    {
                        existing.last_heartbeat = DateTime.Now;

                        await context.SaveChangesAsync();

                        Console.WriteLine("Heartbeat updated");
                    }

                    // ===============================
                    // DUMMY AGING LOG
                    // ===============================

                    var iot = await context.tb_IoTModuleAPI
                        .FirstOrDefaultAsync(x => x.mac_address == mac);

                    if (iot != null)
                    {
                        var aging = await context.tb_AgingDeviceAPI
                            .FirstOrDefaultAsync(x =>
                                x.id_module == iot.id_module);

                        if (aging != null)
                        {
                            var process = await context.tb_AgingProcessAPI
                                .FirstOrDefaultAsync(x =>
                                    x.id_device == aging.id_device &&
                                    x.process_status == 1);

                            if (process != null)
                            {
                                Random rnd = new Random();

                                decimal current =
                                    (decimal)(rnd.NextDouble() * 2 + 1);

                                decimal voltage =
                                    (decimal)(rnd.NextDouble() * 20 + 210);

                                int runtime = process.start_time.HasValue
                                    ? (int)(DateTime.Now -
                                        process.start_time.Value).TotalSeconds
                                    : 0;

                                context.tb_AgingLogAPI.Add(
                                    new AgingLogAPIModel
                                    {
                                        id_process = process.id_process,
                                        current_value = current,
                                        voltage_value = voltage,
                                        runtime_seconds = runtime,
                                        is_abnormal = false,
                                        log_time = DateTime.Now
                                    });

                                await context.SaveChangesAsync();

                                Console.WriteLine(
                                    $"Log inserted -> I:{current} V:{voltage}");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }

                // delay 10 detik
                await Task.Delay(10000, stoppingToken);
            }
        }
    }
}