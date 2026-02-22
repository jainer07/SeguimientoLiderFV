using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeguimientoApp.Infrastructure.Persistence.MySql.Models
{
    [Table("tbl_sms_outbox")]
    public class SmsOutboxModel
    {
        [Key]
        [Column("Id")]
        public long Id { get; set; }

        [Column("IdJob")]
        public long IdJob { get; set; }

        [Column("Phone")]
        public string Phone { get; set; } = ""; // guardamos ya formateado: 57 + 10 dígitos

        [Column("Estado")]
        public string Estado { get; set; } = "PENDING"; // PENDING, SENDING, SENT, FAILED, RETRY

        [Column("Intentos")]
        public int Intentos { get; set; }

        [Column("NextAttemptAt")]
        public DateTime NextAttemptAt { get; set; } = DateTime.UtcNow;

        [Column("LastError")]
        public string? LastError { get; set; }

        [Column("ProviderMessageId")]
        public string? ProviderMessageId { get; set; }

        [Column("CreatedAt")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("UpdatedAt")]
        public DateTime? UpdatedAt { get; set; }

        public SmsJobModel Job { get; set; } = null!;
    }
}
