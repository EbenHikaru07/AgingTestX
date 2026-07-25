using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgingTest.Models
{
    [Table("tbl_otg_iot_aging_process")]
    public class AgingProcessModel
    {
        [Key]
        public int id_process { get; set; }

        public int id_device { get; set; }

        public int id_config { get; set; }

        public int id_user { get; set; }

        public DateTime? start_time { get; set; }
        public DateTime? end_time { get; set; }

        public int process_status { get; set; }
        public int? test_number { get; set; }

        public string? remarks { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;

        // 🔥 Navigation
        [ForeignKey("id_device")]
        public virtual AgingDeviceModel? Device { get; set; }

        [ForeignKey("id_config")]
        public virtual TestConfigurationModel? Configuration { get; set; }
        
        [ForeignKey("id_user")]
        public virtual UserModel? User { get; set; }

        public virtual ICollection<AgingLogModel>? LogAgings { get; set; }
        public virtual ICollection<AgingAbnormalModel>? GangguanAgings { get; set; }
    }
}
