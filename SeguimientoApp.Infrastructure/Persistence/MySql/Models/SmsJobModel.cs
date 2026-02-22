using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeguimientoApp.Infrastructure.Persistence.MySql.Models
{
    [Table("tbl_sms_job")]
    public class SmsJobModel
    {
        [Key]
        [Column("IdJob")]
        public long IdJob { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("Message")]
        public string Message { get; set; } = "";

        [Column("Target")]
        public string Target { get; set; } = "";

        [Column("Total")]
        public int Total { get; set; }

        [Column("Sent")]
        public int Sent { get; set; }

        [Column("Failed")]
        public int Failed { get; set; }

        [Column("Status")]
        public string Status { get; set; } = "QUEUED"; // QUEUED, RUNNING, DONE, CANCELED

        [Column("LastError")]
        public string? LastError { get; set; }

        public List<SmsOutboxModel> Outbox { get; set; } = new();
    }
}
