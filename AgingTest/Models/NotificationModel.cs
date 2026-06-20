//using DocumentFormat.OpenXml.Spreadsheet;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace AgingTest.Models
{
    [Table("tbl_otg_iot_aging_notifications")]
    public class NotificationModel
    {
        [Key]
        public int id_notif { get; set; }

        [Required]
        [StringLength(250)]
        public string title { get; set; }

        [Required]
        public string message { get; set; }

        public int? id_user { get; set; }

        public bool is_read { get; set; } = false;

        public DateTime created_at { get; set; } = DateTime.Now;

        public string? field1 { get; set; }

        public int? field2 { get; set; }
        // Navigation property (jika kamu punya model User)
        [ForeignKey("id_user")]
        public virtual UserModel? User { get; set; }
    }
}