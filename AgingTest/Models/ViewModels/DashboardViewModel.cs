namespace AgingTest.Models.ViewModels
{
    public class DashboardViewModel
    {
        public int id_device { get; set; }
        public string nama_device { get; set; }
        public string kode_device { get; set; }
        public string ip_address { get; set; }
        public string lokasi { get; set; }
        public DateTime? last_heartbeat { get; set; }
        public string? MacAddress { get; set; }

        public string? IpAddress { get; set; }

        //public string StatusRealtime { get; set; }
        public string HeartbeatIndicator { get; set; }
        public string ActionType { get; set; }

        public DateTime? LastHeartbeat { get; set; }
        public DateTime? LastFinishedTime { get; set; }
        // 🔥 tambahan
        public bool IsRunning { get; set; }
        public string StatusProses { get; set; }
        public DateTime? StartTime { get; set; }
        public string? ModuleName { get; set; }
        public DateTime? EndTime { get; set; }
        public bool HasModule { get; set; }
        public string LampName { get; set; }
        public string LampCode { get; set; }
        public string FinalStatus { get; set; }
        public string DeviceStatus { get; set; }
        public string ModuleStatus { get; set; }
        public string ProcessStatus { get; set; }
        public int ProgressPercent { get; set; }
        public bool CanStart { get; set; }

        public string OperationalStatus { get; set; }
    }
}
