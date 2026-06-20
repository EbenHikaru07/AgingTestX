using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgingTest.Models
{
    [Table("tbl_otg_iot_module")]
    public class IoTModuleModel
    {
        [Key]
        public int id_module { get; set; }

        [Required]
        [StringLength(150)]
        public string module_name { get; set; }

        [StringLength(100)]
        public string? mac_address { get; set; }

        [StringLength(100)]
        public string? ip_address { get; set; }

        //public string? firmware_version { get; set; }

        public bool module_status { get; set; } = true;

        public DateTime? last_heartbeat { get; set; }

        public string? remarks { get; set; }

        public DateTime created_at { get; set; } = DateTime.Now;
        public DateTime? updated_at { get; set; }
        public string? field1 { get; set; }
        public int? field2 { get; set; }

        public virtual ICollection<AgingDeviceModel>? AgingDevices { get; set; }
    }
}
