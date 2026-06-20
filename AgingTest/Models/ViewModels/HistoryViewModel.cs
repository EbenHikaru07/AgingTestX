namespace AgingTest.Models.ViewModels
{
    public class HistoryPengujianVM
    {
        public int? SelectedLampId { get; set; }
        public int? SelectedProcessId { get; set; }

        public string? SelectedLampName { get; set; }
        public string? SelectedLampCode { get; set; }

        public int TotalProcess { get; set; }

        public List<LampItemVM> Lamps { get; set; } = new();
        public List<ProcessItemVM> Processes { get; set; } = new();
        public List<LogItemVM> Logs { get; set; } = new();
    }

    public class LampItemVM
    {
        public int IdLamp { get; set; }
        public string LampName { get; set; }
    }

    public class ProcessItemVM
    {
        public int IdProcess { get; set; }

        public int TestNumber { get; set; }

        public string DeviceName { get; set; }

        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }

        public int Status { get; set; }

        public string StatusText =>
            Status == 1 ? "Running" : "Finished";
    }

    public class LogItemVM
    {
        public DateTime LogTime { get; set; }
        public decimal Current { get; set; }
        public decimal Voltage { get; set; }
        public bool IsAbnormal { get; set; }
    }
}
