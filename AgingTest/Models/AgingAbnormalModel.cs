using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgingTest.Models
{
    [Table("tbl_otg_iot_aging_abnormal")]
    public class AgingAbnormalModel
    {
        [Key]
        public int id_abnormal { get; set; }

        public int id_log { get; set; }

        [StringLength(250)]
        public string? description { get; set; }

        public string? status { get; set; }

        public DateTime abnormal_time { get; set; } = DateTime.Now;

        public string? field1 { get; set; }

        public int? field2 { get; set; }
        // 🔥 Navigation
        [ForeignKey("id_log")]
        public virtual AgingLogModel? Log { get; set; }

    }
}
