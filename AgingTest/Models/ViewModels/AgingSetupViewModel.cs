using Microsoft.AspNetCore.Mvc.Rendering;

namespace AgingTest.Models
{
    public class AgingSetupViewModel
    {
        public int DeviceId { get; set; }
        public string NamaDevice { get; set; }
        public string DeviceCode { get; set; }

        public int? LampuId { get; set; }
        public List<SelectListItem> LampuList { get; set; }

        // 🔥 PARAMETER INPUT (RUNTIME)
        public decimal? ArusNominal { get; set; }
        public decimal? TeganganNominal { get; set; }

        public decimal? ArusMin { get; set; }
        public decimal? ArusMax { get; set; }

        public decimal? TeganganMin { get; set; }
        public decimal? TeganganMax { get; set; }

        public int? LifetimeHours { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }

        public string Mode { get; set; }
    }
}
