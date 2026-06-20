using AgingTest.Data;
using AgingTest.Models;
using AgingTest.Models.ViewModels;
using AgingTest.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AgingTest.Controllers
{

    public class MainController : Controller
    {
        private readonly INotificationService _notificationService;
        private readonly AppDbContext _context;
        private readonly StatusProcess _statusProcessConfig;

        public MainController(INotificationService notificationService, IOptionsSnapshot<StatusProcess> statusProcess, AppDbContext context)
        {
            _notificationService = notificationService;
            _context = context;
            _statusProcessConfig = statusProcess.Value;

        }

        public class StatusProcess
        {
            public int Waiting { get; set; }
            public int Running { get; set; }
            public int Completed { get; set; }
            public int Stopped { get; set; }
            public int Error { get; set; }
        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Testing()
        {
            return View();
        }
        public IActionResult DashboardTesting()
        {
            return View();
        }
        public IActionResult IoTDevice()
        {
            return View();
        }

        public async Task<IActionResult> TesNotification()
        {
            await _notificationService.SendToAllAsync(
                "Test Notifikasi",
                "Ini adalah notifikasi realtime untuk testing 🚀"
            );

            return RedirectToAction("Dashboard");
        }

        //public async Task<IActionResult> DataLampuAging()
        //{
        //    var data = await _context.DataLampu
        //        .OrderByDescending(x => x.created_at)
        //        .ToListAsync();

        //    return View(data);
        //}
        public async Task<IActionResult> IoTDeviceData()
        {
            var devices = await _context.IoTModules
                .Include(x => x.AgingDevices)
                .OrderByDescending(x => x.created_at)
                .ToListAsync();

            return View(devices);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> IoTDeviceEntry(IoTModuleModel model)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("IoTDevice");

            model.created_at = DateTime.Now;

            _context.IoTModules.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Device berhasil ditambahkan.";
            return RedirectToAction("IoTDeviceData");
        }

        // INI Entry Lampu
        public async Task<IActionResult> EntryDataLampu()
        {
            var data = await _context.DataLampu.OrderByDescending(x => x.created_at).ToListAsync();
            return View(data);
        }

        [HttpPost]
        public async Task<IActionResult> CreateLampu(DataLampu model)
        {
            if (!ModelState.IsValid) return RedirectToAction(nameof(EntryDataLampu));

            model.created_at = DateTime.Now;
            model.lamp_status = true;

            _context.DataLampu.Add(model);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Lampu berhasil ditambahkan";
            return RedirectToAction(nameof(EntryDataLampu));
        }

        [HttpPost]
        public async Task<IActionResult> EditLampu(DataLampu model)
        {
            var data = await _context.DataLampu.FindAsync(model.id_lamp);
            if (data == null) return RedirectToAction(nameof(EntryDataLampu));

            data.lamp_name = model.lamp_name;
            data.lamp_code = model.lamp_code;
            data.remarks = model.remarks;
            data.lamp_status = model.lamp_status;

            await _context.SaveChangesAsync();

            TempData["Success"] = "Lampu berhasil diupdate";
            return RedirectToAction(nameof(EntryDataLampu));
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLampu(int id)
        {
            var data = await _context.DataLampu.FindAsync(id);
            if (data == null) return RedirectToAction(nameof(EntryDataLampu));

            _context.DataLampu.Remove(data);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Lampu berhasil dihapus";
            return RedirectToAction(nameof(EntryDataLampu));
        }

        // CREATE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateLampuAging(DataLampu model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Input data tidak valid.";
                    return RedirectToAction(nameof(EntryDataLampu));
                }

                var duplicate = await _context.DataLampu
                    .AnyAsync(x => x.lamp_code == model.lamp_code);

                if (duplicate)
                {
                    TempData["Error"] = "Kode lampu sudah digunakan.";
                    return RedirectToAction(nameof(EntryDataLampu));
                }

                model.created_at = DateTime.Now;

                _context.DataLampu.Add(model);

                await _context.SaveChangesAsync();

                TempData["Success"] = "Data lampu berhasil ditambahkan.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(EntryDataLampu));
        }

        // EDIT
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditLampuAging(DataLampu model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    TempData["Error"] = "Input data tidak valid.";
                    return RedirectToAction(nameof(EntryDataLampu));
                }

                var data = await _context.DataLampu
                    .FindAsync(model.id_lamp);

                if (data == null)
                {
                    TempData["Error"] = "Data lampu tidak ditemukan.";
                    return RedirectToAction(nameof(EntryDataLampu));
                }

                var duplicate = await _context.DataLampu
                    .AnyAsync(x =>
                        x.lamp_code == model.lamp_code &&
                        x.id_lamp != model.id_lamp);

                if (duplicate)
                {
                    TempData["Error"] = "Kode lampu sudah digunakan.";
                    return RedirectToAction(nameof(EntryDataLampu));
                }

                data.lamp_name = model.lamp_name;
                data.lamp_code = model.lamp_code;
                data.lamp_status = model.lamp_status;
                data.remarks = model.remarks;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Data lampu berhasil diupdate.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(EntryDataLampu));
        }

        // DELETE
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLampuAging(int id)
        {
            try
            {
                var data = await _context.DataLampu
                    .FindAsync(id);

                if (data == null)
                {
                    TempData["Error"] = "Data lampu tidak ditemukan.";
                    return RedirectToAction(nameof(EntryDataLampu));
                }

                // VALIDASI MASIH DIPAKAI PROCESS
                var used = await _context.TestConfiguration
                    .AnyAsync(x => x.id_lamp == id);

                if (used)
                {
                    TempData["Error"] =
                        "Lampu tidak bisa dihapus karena sudah digunakan pada konfigurasi.";

                    return RedirectToAction(nameof(EntryDataLampu));
                }

                _context.DataLampu.Remove(data);

                await _context.SaveChangesAsync();

                TempData["Success"] = "Data lampu berhasil dihapus.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(EntryDataLampu));
        }

        [HttpGet]
        public async Task<IActionResult> GetParameter(int id)
        {
            var param = await _context.TestConfiguration
                .Where(p => p.id_lamp == id)
                .Select(p => new
                {
                    p.nominal_current,
                    p.nominal_voltage,
                    p.current_min,
                    p.current_max,
                    p.voltage_min,
                    p.voltage_max
                })
                .FirstOrDefaultAsync();

            return Json(param);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SaveParameterUji(
     [FromBody] TestConfigurationModel model)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Input parameter tidak valid."
                    });
                }

                var existing = await _context.TestConfiguration
                    .FirstOrDefaultAsync(p => p.id_lamp == model.id_lamp);

                if (existing == null)
                {
                    model.created_at = DateTime.Now;

                    _context.TestConfiguration.Add(model);
                }
                else
                {
                    existing.nominal_current = model.nominal_current;
                    existing.nominal_voltage = model.nominal_voltage;

                    existing.current_min = model.current_min;
                    existing.current_max = model.current_max;

                    existing.voltage_min = model.voltage_min;
                    existing.voltage_max = model.voltage_max;

                    existing.remarks = model.remarks;
                }

                await _context.SaveChangesAsync();

                return Ok(new
                {
                    success = true,
                    message = "Parameter berhasil disimpan."
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        // D E V I C E  ----  A G I N G
        public IActionResult DeviceAging()
        {
            var devices = _context.AgingDevices
                .Include(x => x.IoTModules)
                .OrderByDescending(x => x.id_device)
                .ToList();

            var usedModuleIds = _context.AgingDevices
                .Where(x => x.id_module != null)
                .Select(x => x.id_module)
                .ToList();

            ViewBag.IoTList = _context.IoTModules
                .Where(x => !usedModuleIds.Contains(x.id_module))
                .ToList();
            ViewBag.UsedModules = usedModuleIds;

            return View(devices);
        }

        // CREATE DeviceAging
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateDeviceAging(AgingDeviceModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.device_name) ||
                    string.IsNullOrEmpty(model.device_code))
                {
                    TempData["Error"] = "Nama dan kode device wajib diisi.";
                    return RedirectToAction(nameof(DeviceAging));
                }

                bool isExist = await _context.AgingDevices
                    .AnyAsync(x => x.device_code == model.device_code);

                if (isExist)
                {
                    TempData["Error"] = "Kode device sudah digunakan.";
                    return RedirectToAction(nameof(DeviceAging));
                }

                bool moduleUsed = await _context.AgingDevices
    .AnyAsync(x => x.id_module == model.id_module);

                if (moduleUsed)
                {
                    TempData["Error"] = "IoT Module sudah digunakan device lain.";
                    return RedirectToAction(nameof(DeviceAging));
                }

                model.created_at = DateTime.Now;
                model.device_status = true;

                _context.AgingDevices.Add(model);

                await _context.SaveChangesAsync();

                TempData["Success"] = "Device berhasil ditambahkan.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.InnerException?.Message ?? ex.Message;
            }

            return RedirectToAction(nameof(DeviceAging));
        }

        // EDIT DeviceAging
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditDeviceAging(AgingDeviceModel model)
        {
            try
            {
                var device = await _context.AgingDevices
                    .FirstOrDefaultAsync(x => x.id_device == model.id_device);

                if (device == null)
                {
                    TempData["Error"] = "Device tidak ditemukan.";
                    return RedirectToAction(nameof(DeviceAging));
                }

                bool duplicateCode = await _context.AgingDevices
                    .AnyAsync(x => x.device_code == model.device_code
                                && x.id_device != model.id_device);

                if (duplicateCode)
                {
                    TempData["Error"] = "Kode device sudah digunakan.";
                    return RedirectToAction(nameof(DeviceAging));
                }

                bool moduleUsed = await _context.AgingDevices
    .AnyAsync(x => x.id_module == model.id_module
                && x.id_device != model.id_device);

                if (moduleUsed)
                {
                    TempData["Error"] = "IoT Module sudah digunakan device lain.";
                    return RedirectToAction(nameof(DeviceAging));
                }

                device.device_name = model.device_name;
                device.device_code = model.device_code;
                device.id_module = model.id_module;
                device.max_current = model.max_current;
                device.max_voltage = model.max_voltage;
                device.device_status = model.device_status;
                device.remarks = model.remarks;

                // START DOWNTIME
                if (model.is_downtime == true && device.is_downtime != true)
                {
                    device.is_downtime = true;
                    device.downtime_start = DateTime.Now;
                    device.downtime_end = null;
                }

                // END DOWNTIME
                else if (model.is_downtime == false && device.is_downtime == true)
                {
                    device.is_downtime = false;
                    device.downtime_end = DateTime.Now;
                }

                device.updated_at = DateTime.Now;

                await _context.SaveChangesAsync();

                TempData["Success"] = "Device berhasil diupdate.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.InnerException?.Message ?? ex.Message;
            }

            return RedirectToAction(nameof(DeviceAging));
        }

        // DELETE DeviceAging
        public async Task<IActionResult> DeleteDeviceAging(int id)
        {
            try
            {
                var data = await _context.AgingDevices
                    .FirstOrDefaultAsync(x => x.id_device == id);

                if (data == null)
                {
                    TempData["Error"] = "Device tidak ditemukan.";
                    return RedirectToAction(nameof(DeviceAging));
                }

                _context.AgingDevices.Remove(data);

                await _context.SaveChangesAsync();

                TempData["Success"] = "Device berhasil dihapus.";
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.InnerException?.Message ?? ex.Message;
            }

            return RedirectToAction(nameof(DeviceAging));
        }

        public IActionResult Dashboard()
        {
            var now = DateTime.Now;

            var devices = _context.AgingDevices
                .Include(x => x.IoTModules)
                .ToList();

            var processList = _context.AgingProcess
                .Include(x => x.Configuration)
                .ToList();

            // UPDATE PROCESS STATUS
            foreach (var p in processList)
            {
                if (p.end_time != null && now >= p.end_time)
                {
                    p.process_status = 2;
                }
                else if (
                    p.start_time != null &&
                    now >= p.start_time &&
                    now < p.end_time)
                {
                    p.process_status = 1;
                }
            }

            _context.SaveChanges();

            var dashboard = devices.Select(d =>
            {
                var process = processList
                    .Where(x => x.id_device == d.id_device)
                    .OrderByDescending(x => x.start_time)
                    .FirstOrDefault();

                bool hasModule = d.id_module != null;

                bool deviceActive = d.device_status;

                bool moduleOnline =
                    hasModule &&
                    GetRealtimeStatus(d.IoTModules?.last_heartbeat) == "ONLINE";

                bool isDowntime =
                    d.is_downtime == true;

                bool isRunning = process?.process_status == 1;

                bool processFinished = process?.process_status == 2;

                // hitung progress
                int progress = 0;

                if (process != null &&
                    process.start_time != null &&
                    process.end_time != null)
                {
                    var totalDuration =
                        (process.end_time.Value -
                         process.start_time.Value).TotalMinutes;

                    var elapsed =
                        (DateTime.Now -
                         process.start_time.Value).TotalMinutes;

                    if (totalDuration > 0)
                    {
                        progress =
                            (int)Math.Min(
                                100,
                                Math.Max(
                                    0,
                                    elapsed / totalDuration * 100
                                )
                            );
                    }
                    if (processFinished)
                    {
                        progress = 100;
                    }
                }

                string operationalStatus;

                if (!deviceActive)
                {
                    operationalStatus = "DISABLED";
                }
                else if (!hasModule)
                {
                    operationalStatus = "NO MODULE";
                }
                else if (isDowntime)
                {
                    operationalStatus = "DOWNTIME";
                }
                else if (!moduleOnline)
                {
                    operationalStatus = "OFFLINE";
                }
                else if (isRunning)
                {
                    operationalStatus = "RUNNING";
                }
                else if (processFinished)
                {
                    operationalStatus = "FINISHED";
                }
                else
                {
                    operationalStatus = "READY";
                }
                string moduleStatus;

                if (!hasModule)
                {
                    moduleStatus = "NO MODULE";
                }
                else if (moduleOnline)
                {
                    moduleStatus = "ONLINE";
                }
                else
                {
                    moduleStatus = "OFFLINE";
                }
                string actionType;

                if (!hasModule)
                {
                    actionType = "SETUP_MODULE";
                }
                else if (!moduleOnline)
                {
                    actionType = "CHECK_MODULE";
                }
                else if (!deviceActive)
                {
                    actionType = "CHECK_DEVICE";
                }
                else if (isRunning || isDowntime)
                {
                    actionType = "MONITOR";
                }
                else
                {
                    actionType = "START_AGING";
                }

                var lastFinishedProcess = processList
    .Where(x =>
        x.id_device == d.id_device &&
        x.process_status == 2)
    .OrderByDescending(x => x.end_time)
    .FirstOrDefault();


                //// CAN START AGING
                //bool canStart =
                //    deviceActive &&
                //    hasModule &&
                //    moduleOnline &&
                //    !isDowntime &&
                //    !isRunning;

                return new DashboardViewModel
                {
                    id_device = d.id_device,

                    nama_device = d.device_name,
                    kode_device = d.device_code,

                    ModuleName = d.IoTModules?.module_name ?? "-",
                    MacAddress = d.IoTModules?.mac_address ?? "-",

                    last_heartbeat = d.IoTModules?.last_heartbeat,

                    StartTime = process?.start_time,
                    EndTime = process?.end_time,

                    HasModule = hasModule,

                    IsRunning = isRunning,

                    //CanStart = canStart,

                    OperationalStatus = operationalStatus,

                    ModuleStatus = moduleStatus,
                    ActionType = actionType,
                    ProgressPercent = progress,
                    LastFinishedTime = lastFinishedProcess?.end_time,

                    LastHeartbeat = d.IoTModules?.last_heartbeat
                };

            }).ToList();

            return View(dashboard);
        }

        private string GetRealtimeStatus(DateTime? heartbeat)
        {
            if (heartbeat == null)
                return "OFFLINE";

            var diff = (DateTime.Now - heartbeat.Value).TotalSeconds;

            if (diff <= 120)
                return "ONLINE";

            return "OFFLINE";
        }

        public IActionResult SetUpAging(int deviceId)
        {
            var device = _context.AgingDevices
                .FirstOrDefault(d => d.id_device == deviceId);

            if (device == null)
                return NotFound();

            // AMBIL LAMPU YANG SEDANG DIPAKAI
            var usedLampIds = _context.AgingProcess

                // scheduled / running
                .Where(p =>
                    p.process_status == 0 ||
                    p.process_status == 1)

                .Join(
                    _context.TestConfiguration,
                    process => process.id_config,
                    config => config.id_config,
                    (process, config) => config.id_lamp
                )

                .Distinct()
                .ToList();

            // HANYA LAMPU AVAILABLE
            var availableLampu = _context.DataLampu

                .Where(l =>
                    l.lamp_status &&
                    !usedLampIds.Contains(l.id_lamp))

                .ToList();

            // DROPDOWN
            var lampuList = availableLampu

                .Select(l => new SelectListItem
                {
                    Value = l.id_lamp.ToString(),
                    Text = $"{l.lamp_name} ({l.lamp_code})"
                })

                .ToList();

            if (!lampuList.Any())
            {
                TempData["Error"] = "Tidak ada lampu tersedia.";
            }

            // CONFIG
            var configList =
                _context.TestConfiguration.ToList();

            // DATA JS
            var lampuData = availableLampu

                .Select(l => new
                {
                    l.id_lamp,
                    l.lamp_name,
                    l.lamp_code,

                    parameter = configList

                        .Where(p => p.id_lamp == l.id_lamp)

                        .Select(p => new
                        {
                            p.nominal_current,
                            p.nominal_voltage,
                            p.current_min,
                            p.current_max,
                            p.voltage_min,
                            p.voltage_max
                        })

                        .FirstOrDefault()
                })

                .ToList();

            // DEVICE SPEC
            var deviceSpec = new
            {
                max_current = device.max_current,
                max_voltage = device.max_voltage
            };

            ViewBag.LampuData = lampuData;
            ViewBag.DeviceSpec = deviceSpec;

            // MODEL
            var model = new AgingSetupViewModel
            {
                DeviceId = device.id_device,
                NamaDevice = device.device_name,
                DeviceCode = device.device_code,

                LampuList = lampuList,

                LifetimeHours = 1,

                StartTime = DateTime.Now
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> StartAging(AgingSetupViewModel model)
        {
            try
            {
                var now = DateTime.Now;

                // VALIDASI DEVICE
                var deviceRunning = _context.AgingProcess
    .Any(x =>
        x.id_device == model.DeviceId &&
        (x.process_status == 0 || x.process_status == 1));

                if (deviceRunning)
                {
                    TempData["Error"] = "Device sedang running.";
                    return RedirectToAction("Dashboard");
                }

                // VALIDASI LAMPU
                if (model.LampuId == null)
                {
                    TempData["Error"] = "Lampu wajib dipilih.";
                    return RedirectToAction("SetUpAging", new { deviceId = model.DeviceId });
                }

                // VALIDASI PARAMETER
                var parameter = _context.TestConfiguration
                    .FirstOrDefault(x => x.id_lamp == model.LampuId);

                if (parameter == null)
                {
                    TempData["Error"] = "Parameter lampu belum diset.";
                    return RedirectToAction("SetUpAging", new { deviceId = model.DeviceId });
                }

                var lampBusy = _context.AgingProcess

    .Where(p =>
        p.process_status == 0 ||
        p.process_status == 1)

    .Join(
        _context.TestConfiguration,
        process => process.id_config,
        config => config.id_config,
        (process, config) => config
    )

    .Any(c => c.id_lamp == model.LampuId);

                if (lampBusy)
                {
                    TempData["Error"] =
                        "Lampu sedang digunakan process lain.";

                    return RedirectToAction(
                        "SetUpAging",
                        new { deviceId = model.DeviceId });
                }

                DateTime startTime;
                DateTime endTime;

                // MODE DURATION
                if (model.Mode == "duration")
                {
                    if (!model.LifetimeHours.HasValue || model.LifetimeHours <= 0)
                    {
                        TempData["Error"] = "Lifetime harus lebih dari 0.";
                        return RedirectToAction("SetUpAging", new { deviceId = model.DeviceId });
                    }

                    startTime = now;
                    endTime = now.AddHours(model.LifetimeHours.Value);
                }

                // MODE SCHEDULE
                else
                {
                    if (!model.StartTime.HasValue || !model.EndTime.HasValue)
                    {
                        TempData["Error"] = "Schedule belum lengkap.";
                        return RedirectToAction("SetUpAging", new { deviceId = model.DeviceId });
                    }

                    startTime = model.StartTime.Value;
                    endTime = model.EndTime.Value;

                    if (startTime >= endTime)
                    {
                        TempData["Error"] = "End time harus lebih besar.";

                        return RedirectToAction("SetUpAging",new { deviceId = model.DeviceId });
                    }

                    if (startTime < now)
                    {
                        TempData["Error"] = "Start time tidak boleh kurang dari waktu sekarang.";

                        return RedirectToAction("SetUpAging",new { deviceId = model.DeviceId });
                    }
                }

                // STATUS PROCESS
                int status =
                    now < startTime ? 0 :
                    now >= startTime && now < endTime ? 1 :
                    2;

                var lastTestNumber = await (
    from p in _context.AgingProcess
    join c in _context.TestConfiguration
        on p.id_config equals c.id_config
    where c.id_lamp == model.LampuId
    select (int?)p.field2
)
.MaxAsync();

                int currentTestNumber = (lastTestNumber ?? 0) + 1;

                var process = new AgingProcessModel
                {
                    id_device = model.DeviceId,
                    id_config = parameter.id_config,
                    id_user = 1,
                    start_time = startTime,
                    end_time = endTime,
                    process_status = status,
                    created_at = now,

                    field2 = currentTestNumber
                };

                _context.AgingProcess.Add(process);
                _context.SaveChanges();

                TempData["Success"] = "Aging berhasil dimulai.";

                return RedirectToAction("MonitoringAging",
                    new { deviceId = process.id_device });
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;

                return RedirectToAction("SetUpAging",
                    new { deviceId = model.DeviceId });
            }
        }

        public IActionResult MonitoringAging(int deviceId)
        {
            var now = DateTime.Now;

            var proses = (

                from p in _context.AgingProcess
                join d in _context.AgingDevices
                    on p.id_device equals d.id_device
                // 🔥 JOIN ESP32
                join iot in _context.IoTModules
                    on d.id_module equals iot.id_module
                join config in _context.TestConfiguration
                    on p.id_config equals config.id_config
                join lamp in _context.DataLampu
                    on config.id_lamp equals lamp.id_lamp
                where p.id_device == deviceId
                orderby p.created_at descending
                select new
                {
                    process = p,
                    device = d,
                    iotdevice = iot,
                    parameter = config,
                    lampu = lamp
                }
            ).FirstOrDefault();

            if (proses == null)
            {
                TempData["Error"] = "Tidak ada proses aging.";

                return RedirectToAction("Dashboard");
            }

            // 🔥 UPDATE STATUS
            if (proses.process.start_time != null &&
                proses.process.end_time != null)
            {
                if (now < proses.process.start_time)
                    proses.process.process_status = 0;
                else if (now >= proses.process.start_time &&
                         now <= proses.process.end_time)
                    proses.process.process_status = 1;
                else
                    proses.process.process_status = 2;
                _context.SaveChanges();
            }

            // 🔥 REALTIME LOG
            var realtime = _context.AgingLog
                .Where(x =>
                    x.id_process ==
                    proses.process.id_process)
                .OrderByDescending(x => x.log_time)
                .FirstOrDefault();

            var model = new MonitoringViewModel
            {
                IdProses = proses.process.id_process,
                DeviceName = proses.device.device_name,
                DeviceCode = proses.device.device_code,
                // 🔥 DATA MCU
                ModuleName = proses.iotdevice.module_name,
                MacAddress = proses.iotdevice.mac_address,
                IpAddress = proses.iotdevice.ip_address,
                LastHeartbeat = proses.iotdevice.last_heartbeat,
                // 🔥 LAMPU
                LampuName = proses.lampu.lamp_name,
                LampuCode = proses.lampu.lamp_code,
                // 🔥 PARAMETER
                NominalCurrent = proses.parameter.nominal_current,
                NominalVoltage = proses.parameter.nominal_voltage,
                CurrentMin = proses.parameter.current_min,
                CurrentMax = proses.parameter.current_max,
                VoltageMin = proses.parameter.voltage_min,
                VoltageMax = proses.parameter.voltage_max,
                // 🔥 REALTIME SENSOR
                CurrentValue = realtime?.current_value,
                VoltageValue = realtime?.voltage_value,
                StartTime = proses.process.start_time,
                EndTime = proses.process.end_time,
                Status = proses.process.process_status
            };

            return View(model);
        }

        [HttpGet]
        public IActionResult GetRealtimeLog(int prosesId)
        {
            var data = (
                from log in _context.AgingLog
                join process in _context.AgingProcess
                    on log.id_process equals process.id_process
                join device in _context.AgingDevices
                    on process.id_device equals device.id_device
                join module in _context.IoTModules
                    on device.id_module equals module.id_module

                where log.id_process == prosesId

                orderby log.log_time descending

                select new
                {
                    current = log.current_value,
                    voltage = log.voltage_value,
                    created_at = log.log_time,
                    lastHeartbeat = module.last_heartbeat
                }

            ).FirstOrDefault();

            if (data == null)
                return Json(null);

            return Json(data);
        }

        [HttpGet]
        public IActionResult GetChartLogs(int prosesId)
        {
            var logs = _context.AgingLog
                .Where(x => x.id_process == prosesId)
                .OrderBy(x => x.log_time)
                .Select(x => new
                {
                    current = x.current_value,
                    voltage = x.voltage_value,
                    log_time = x.log_time
                })
                .ToList();

            return Json(logs);
        }

        [HttpGet]
        public IActionResult GetMonitoringData(int prosesId)
        {
            var data = _context.AgingProcess
                .Where(p => p.id_process == prosesId)
                .Select(p => new {
                    start_time = p.start_time,
                    end_time = p.end_time,
                    status_proses = p.process_status
                })
                .FirstOrDefault();

            if (data == null)
                return Json(null);

            return Json(data);
        }

        // NOTIFICATION =====================================
        public IActionResult NotificationIndex()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var notifications = _context.Notifications
                .Where(x => x.id_user == userId)
                .OrderByDescending(x => x.created_at)
                .ToList();

            return View(notifications);
        }

        [HttpPost]
        public IActionResult NotificationMarkAsRead(int id)
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var notif = _context.Notifications
                .FirstOrDefault(x => x.id_notif == id && x.id_user == userId);

            if (notif == null) return NotFound();

            notif.is_read = true;
            _context.SaveChanges();

            return Ok();
        }

        [HttpPost]
        public IActionResult NotificationMarkAllAsRead()
        {
            int userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)!.Value);

            var notifications = _context.Notifications
                .Where(x => x.id_user == userId && !x.is_read)
                .ToList();

            notifications.ForEach(x => x.is_read = true);

            _context.SaveChanges();

            return RedirectToAction(nameof(NotificationIndex));
        }

        public async Task<IActionResult> HistoriPengujian(
    int? lampId,
    int? processId)
        {
            var vm = new HistoryPengujianVM();

            vm.SelectedLampId = lampId;
            if (lampId.HasValue)
            {
                var lamp = await _context.DataLampu
                    .FirstOrDefaultAsync(x => x.id_lamp == lampId);

                if (lamp != null)
                {
                    vm.SelectedLampName = lamp.lamp_name;
                    vm.SelectedLampCode = lamp.lamp_code;

                    vm.TotalProcess = await (
                        from p in _context.AgingProcess
                        join c in _context.TestConfiguration
                            on p.id_config equals c.id_config
                        where c.id_lamp == lampId
                        select p.id_process
                    ).CountAsync();
                }
            }

            vm.SelectedProcessId = processId;

            vm.Lamps = await (
    from lamp in _context.DataLampu
    join cfg in _context.TestConfiguration
        on lamp.id_lamp equals cfg.id_lamp
    join process in _context.AgingProcess
        on cfg.id_config equals process.id_config
    select new LampItemVM
    {
        IdLamp = lamp.id_lamp,
        LampName = lamp.lamp_name
    }
)
.Distinct()
.OrderBy(x => x.LampName)
.ToListAsync();

            if (lampId.HasValue)
            {
                vm.Processes = await (
    from process in _context.AgingProcess
    join config in _context.TestConfiguration
        on process.id_config equals config.id_config
    join device in _context.AgingDevices
        on process.id_device equals device.id_device
    where config.id_lamp == lampId.Value
    orderby process.field2 descending
    select new ProcessItemVM
    {
        IdProcess = process.id_process,
        TestNumber = process.field2 ?? 0,
        DeviceName = device.device_name,
        StartTime = process.start_time,
        EndTime = process.end_time,
        Status = process.process_status
    }
).ToListAsync();
            }

            if (processId.HasValue)
            {
                vm.Logs = await _context.AgingLog
                    .Where(x => x.id_process == processId.Value)
                    .OrderBy(x => x.log_time)
                    .Select(x => new LogItemVM
                    {
                        LogTime = x.log_time,
                        Current = x.current_value ?? 0,
                        Voltage = x.voltage_value ?? 0,
                        IsAbnormal = x.is_abnormal
                    })
                    .ToListAsync();
            }

            return View(vm);
        }
    }
}
