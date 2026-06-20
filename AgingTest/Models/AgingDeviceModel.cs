using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgingTest.Models
{
    [Table("tbl_otg_iot_aging_devices")]
    public class AgingDeviceModel
    {
        [Key]
        public int id_device { get; set; }

        [StringLength(150)]
        public string? device_name { get; set; }

        [StringLength(50)]
        public string? device_code { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? max_current { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? max_voltage { get; set; }

        //public string? firmware_version { get; set; }

        public bool device_status { get; set; } = true;
        public bool? is_downtime { get; set; }
        public DateTime? downtime_start { get; set; }
        public DateTime? downtime_end { get; set; }

        public string? remarks { get; set; }

        public DateTime created_at { get; set; } = DateTime.Now;
        public DateTime? updated_at { get; set; }
        public string? field1 { get; set; }

        public int? field2 { get; set; }

        public DateTime? field3 { get; set; }

        public decimal? field4 { get; set; }

        public decimal? field5 { get; set; }

        // 🔥 Navigation
        public int? id_module { get; set; }

        [ForeignKey("id_module")]
        public virtual IoTModuleModel? IoTModules { get; set; }

        public virtual ICollection<AgingProcessModel>? ProsesAgings { get; set; }
    }
}
