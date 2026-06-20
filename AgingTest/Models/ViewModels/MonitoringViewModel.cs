namespace AgingTest.Models.ViewModels
{
    public class MonitoringViewModel
    {
        public int IdProses { get; set; }

        // 🔵 DEVICE
        public string DeviceName { get; set; }
        public string DeviceCode { get; set; }

        // Module
        public string ModuleName { get; set; }

        // 💡 LAMPU
        public string LampuName { get; set; }
        public string LampuCode { get; set; }

        public decimal? NominalCurrent { get; set; }
        public decimal? NominalVoltage { get; set; }

        // ⏱ TIME
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public int? Duration { get; set; } // jam
        public string? MacAddress { get; set; }

        public string? IpAddress { get; set; }

        public DateTime? LastHeartbeat { get; set; }
        // 🔥 STATUS (0=STOP,1=RUNNING,2=DONE,3=ERROR,4=SCHEDULED)
        public int Status { get; set; }

        // 🔥 STATUS TEXT
        public string StatusText => Status switch
        {
            0 => "STOP",
            1 => "RUNNING",
            2 => "DONE",
            3 => "ERROR",
            4 => "SCHEDULED",
            _ => "UNKNOWN"
        };

        // 🔥 PROGRESS %
        public double ProgressPercent
        {
            get
            {
                if (StartTime == null || Duration == null)
                    return 0;

                var now = DateTime.Now;

                // 🔥 belum mulai (schedule)
                if (now < StartTime.Value)
                    return 0;

                var total = TimeSpan.FromHours(Duration.Value);
                var elapsed = now - StartTime.Value;

                return Math.Min(100, elapsed.TotalSeconds / total.TotalSeconds * 100);
            }
        }

        // 🔥 TIMER TEXT (langsung bisa dipakai di View kalau mau)
        public string TimerText
        {
            get
            {
                if (StartTime == null || Duration == null)
                    return "-";

                var now = DateTime.Now;

                // 🔥 BELUM MULAI
                if (now < StartTime.Value)
                {
                    var diff = StartTime.Value - now;
                    return $"Start in {FormatTime(diff)}";
                }

                // 🔥 SEDANG JALAN
                var end = EndTime.Value;

                if (now <= end)
                {
                    var diff = end - now;
                    return $"{FormatTime(diff)} left";
                }

                // 🔥 SUDAH SELESAI
                return "DONE";
            }
        }

        // 🔥 FORMAT TIME HELPER
        private string FormatTime(TimeSpan ts)
        {
            return $"{(int)ts.TotalHours}h {ts.Minutes}m {ts.Seconds}s";
        }

        // ⚡ REALTIME VALUE (dari sensor / DB nanti)
        public decimal? CurrentValue { get; set; }
        public decimal? VoltageValue { get; set; }
        public decimal? CurrentMin { get; set; }
        public decimal? CurrentMax { get; set; }
        public decimal? VoltageMin { get; set; }
        public decimal? VoltageMax { get; set; }

        // 🔥 STATUS POWER (helper UI)
        public string PowerStatus
        {
            get
            {
                if (CurrentValue == null || VoltageValue == null || NominalCurrent == null || NominalVoltage == null)
                    return "UNKNOWN";

                var currentPercent = (double)(CurrentValue.Value / NominalCurrent.Value) * 100;
                var voltagePercent = (double)(VoltageValue.Value / NominalVoltage.Value) * 100;

                var max = Math.Max(currentPercent, voltagePercent);

                if (max > 100) return "OVERLOAD";
                if (max > 80) return "WARNING";

                return "NORMAL";
            }
        }
    }
}