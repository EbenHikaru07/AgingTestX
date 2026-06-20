using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgingTest_API.Models
{
    [Table("tbl_otg_iot_aging_process")]
    public class AgingProcessAPIModel
    {
        [Key]
        public int id_process { get; set; }

        public int id_device { get; set; }

        public int id_config { get; set; }

        public int id_user { get; set; }

        public DateTime? start_time { get; set; }
        public DateTime? end_time { get; set; }

        public int process_status { get; set; }

        public string? remarks { get; set; }
        public DateTime created_at { get; set; } = DateTime.Now;
        public string? field1 { get; set; }

        public int? field2 { get; set; }
        [ForeignKey("id_device")]
        public virtual AgingDeviceAPIModel? Device { get; set; }

        [ForeignKey("id_config")]
        public virtual TestConfigurationAPIModel? Config { get; set; }
    }
}
