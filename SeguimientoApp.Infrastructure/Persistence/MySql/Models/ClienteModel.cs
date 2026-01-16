using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeguimientoApp.Infrastructure.Persistence.MySql.Models
{
    [Table("tbl_cliente")]
    public class ClienteModel
    {
        [Key]
        [Column("IdCliente")]
        public int IdCliente { get; set; }

        [Column("IdTipoDocumento")]
        public int IdTipoDocumento { get; set; }

        [Column("NumeroDocumento")]
        public long NumeroDocumento { get; set; }

        [Column("Nombre")]
        public string Nombre { get; set; }

        [Column("Estado")]
        public bool Estado { get; set; }
    }
}
