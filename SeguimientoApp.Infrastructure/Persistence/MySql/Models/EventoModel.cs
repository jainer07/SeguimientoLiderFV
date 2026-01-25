using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeguimientoApp.Infrastructure.Persistence.MySql.Models
{
    [Table("tbl_evento")]
    public class EventoModel
    {
        [Key]
        public int IdEvento { get; set; }

        public string Nombre { get; set; }

        public int IdTipoEventoCat { get; set; }
        public int IdEstadoEventoCat { get; set; }

        public DateTime? FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }

        public string Lugar { get; set; }
        public string Observaciones { get; set; }

        public CatalogoModel TipoEvento { get; set; }
        public CatalogoModel EstadoEvento { get; set; }
    }

}
