using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgingTest.Models
{
    [Table("tbl_otg_iot_users")]
    public class UserModel
    {
        [Key]
        public int id_user { get; set; }

        [Required]
        [StringLength(50)]
        public string user_badge { get; set; }

        [Required]
        [StringLength(30)]
        public string username { get; set; }

        [Required]
        [StringLength(250)]
        public string user_password { get; set; }

        [Required]
        [StringLength(100)]
        public string user_role { get; set; }

        public bool user_status { get; set; } = true;

        public DateTime? last_active { get; set; }

        public DateTime created_at { get; set; } = DateTime.Now;
        public DateTime updated_at { get; set; } = DateTime.Now;

        public string? remarks { get; set; }

        public string? field1 { get; set; }

        public int? field2 { get; set; }
        //public virtual ICollection<AgingProcessModel>? AgingProcesses { get; set; }
        //public virtual ICollection<NotificationModel>? Notifications { get; set; }
    }
}
