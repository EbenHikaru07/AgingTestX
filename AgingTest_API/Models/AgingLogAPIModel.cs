using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgingTest_API.Models
{
    [Table("tbl_otg_iot_aging_logs")]
    public class AgingLogAPIModel
    {
        [Key]
        public int id_log { get; set; }

        public int id_process { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? current_value { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? voltage_value { get; set; }
        public int? runtime_seconds { get; set; }

        public DateTime log_time { get; set; } = DateTime.Now;

        public bool is_abnormal { get; set; } = false;

        public string? remarks { get; set; }

        // 🔥 Navigation
        [ForeignKey("id_process")]
        public virtual AgingProcessAPIModel? Proses { get; set; }
    }
}