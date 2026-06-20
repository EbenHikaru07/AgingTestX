using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgingTest.Models
{
    [Table("tbl_otg_iot_aging_lamps")]
    public class DataLampu
    {
        [Key]
        public int id_lamp { get; set; }

        [Required]
        [StringLength(150)]
        public string lamp_name { get; set; }

        [Required]
        [StringLength(50)]
        public string lamp_code { get; set; }

        public bool lamp_status { get; set; } = true;

        public string? remarks { get; set; }

        public DateTime created_at { get; set; } = DateTime.Now;
        //public DateTime updated_at { get; set; } = DateTime.Now;

        public string? field1 { get; set; }

        public int? field2 { get; set; }

        public DateTime? field3 { get; set; }

        // 🔥 Navigation
        //public virtual TestConfigurationModel? ParameterUji { get; set; }
    }
}
