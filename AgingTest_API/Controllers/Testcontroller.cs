using AgingTest_API.Data;
using AgingTest_API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
namespace AgingTest_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TestController : ControllerBase
    {
        private readonly APIAppDbContext _context;
        private readonly StatusProcess _statusProcessConfig;

        public TestController(APIAppDbContext context, IOptionsSnapshot<StatusProcess> statusProcessConfig)
        {
            _context = context;
            _statusProcessConfig = statusProcessConfig.Value;
        }
        public class StatusProcess
        {
            public int Waiting { get; set; }
            public int Running { get; set; }
            public int Completed { get; set; }
            public int Stopped { get; set; }
            public int Error { get; set; }
        }

        [HttpPost("AgingParamLog")]
        public async Task<IActionResult> InsertLog([FromBody] SensorLogRequest request)
        {
            try
            {
                if (request == null)
                    return BadRequest(new { success = false, message = "Request kosong" });
                // 🔥 Cari IoT Device
                var iot = await _context.tb_IoTModuleAPI
                    .FirstOrDefaultAsync(x => x.mac_address == request.MacAddress);

                if (iot == null)
                    return NotFound(new { success = false, message = "IoT Device tidak ditemukan" });
                // 🔥 Cari Aging Device
                var aging = await _context.tb_AgingDeviceAPI
                    .FirstOrDefaultAsync(x => x.id_module == iot.id_module);

                if (aging == null)
                    return NotFound(new { success = false, message = "Aging Device tidak ditemukan" });
                // 🔥 Cari Process Running
                var process = await _context.tb_AgingProcessAPI
                    .FirstOrDefaultAsync(x =>
                        x.id_device == aging.id_device &&
                        x.process_status == 1);

                if (process == null)
                    return BadRequest(new { success = false, message = "Tidak ada process aktif" });
                // 🔥 Ambil Config
                var config = await _context.tb_TestConfigurationAPI
                    .FirstOrDefaultAsync(x => x.id_config == process.id_config);

                if (config == null)
                    return BadRequest(new { success = false, message = "Config tidak ditemukan" });
                // 🔥 Validasi abnormal
                bool abnormal =
                    (config.current_min.HasValue && request.Current < config.current_min) ||
                    (config.current_max.HasValue && request.Current > config.current_max) ||
                    (config.voltage_min.HasValue && request.Voltage < config.voltage_min) ||
                    (config.voltage_max.HasValue && request.Voltage > config.voltage_max);
                // 🔥 Runtime
                int runtime = process.start_time.HasValue
                    ? (int)(DateTime.Now - process.start_time.Value).TotalSeconds
                    : 0;
                // 🔥 Insert Log
                _context.tb_AgingLogAPI.Add(new AgingLogAPIModel
                {
                    id_process = process.id_process,
                    current_value = request.Current,
                    voltage_value = request.Voltage,
                    runtime_seconds = runtime,
                    is_abnormal = abnormal,
                    log_time = DateTime.Now
                });

                // 🔥 Update heartbeat
                iot.last_heartbeat = DateTime.Now;

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Log berhasil disimpan",
                    process_id = process.id_process,
                    aging_device = aging.device_name,
                    iot_device = iot.id_module,
                    current = request.Current,
                    voltage = request.Voltage,
                    abnormal,
                    runtime_seconds = runtime
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost("iot-device")]
        public async Task<IActionResult> PostIoTDevice([FromBody] IoTDeviceRequest request)
        {
            if (request == null ||
                string.IsNullOrEmpty(request.mac_address) ||
                string.IsNullOrEmpty(request.ip_address))
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Data tidak lengkap"
                });
            }

            try
            {
                var mac = request.mac_address.Trim().ToUpper();

                var existing = await _context.tb_IoTModuleAPI
                    .FirstOrDefaultAsync(x => x.mac_address == mac);

                if (existing != null)
                {
                    // UPDATE
                    existing.module_name = request.module_name ?? existing.module_name;
                    existing.ip_address = request.ip_address;
                    existing.last_heartbeat = DateTime.Now;
                    existing.updated_at = DateTime.Now;

                    await _context.SaveChangesAsync();

                    return Ok(new
                    {
                        success = true,
                        message = "Device updated",
                        id = existing.id_module
                    });
                }

                var device = new IoTModuleAPIModel
                {
                    module_name = request.module_name,
                    mac_address = mac,
                    ip_address = request.ip_address,
                    last_heartbeat = DateTime.Now,
                    created_at = DateTime.Now
                };

                _context.tb_IoTModuleAPI.Add(device);
                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Device inserted",
                    id = device.id_module
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet("iot-device")]
        public async Task<IActionResult> GetAllIoTDevice()
        {
            var data = await _context.tb_IoTModuleAPI
                .OrderByDescending(x => x.created_at)
                .Select(x => new
                {
                    x.id_module,
                    x.module_name,
                    x.mac_address,
                    x.ip_address,
                    x.module_status,
                    x.last_heartbeat
                })
                .ToListAsync();

            return Ok(new
            {
                success = true,
                total = data.Count,
                data = data
            });
        }
    }

    public class IoTDeviceRequest
    {
        public string? module_name { get; set; }
        public string? mac_address { get; set; }
        public string? ip_address { get; set; }
    }
    public class SensorLogRequest
    {
        public string? MacAddress { get; set; }
        public decimal Current { get; set; }
        public decimal Voltage { get; set; }
    }
}
