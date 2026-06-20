using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgingTest_API.Models
{
    [Table("tbl_otg_iot_test_configurations")]
    public class TestConfigurationAPIModel
    {
        [Key]
        public int id_config { get; set; }

        public int id_lamp { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? nominal_current { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? nominal_voltage { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? current_min { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? current_max { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? voltage_min { get; set; }

        [Column(TypeName = "decimal(10,2)")]
        public decimal? voltage_max { get; set; }

        //public int? lifetime_hours { get; set; }

        public string? remarks { get; set; }

        public DateTime created_at { get; set; } = DateTime.Now;

    }
}
