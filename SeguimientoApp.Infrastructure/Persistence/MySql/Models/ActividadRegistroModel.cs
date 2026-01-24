using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeguimientoApp.Infrastructure.Persistence.MySql.Models
{
    [Table("tbl_actividad_registro")]
    public class ActividadRegistroModel
    {
        [Key]
        public int IdActividadRegistro { get; set; }

        public int IdEventoActividad { get; set; }
        public int IdEventoParticipante { get; set; }

        public int IdEstadoRegistroCat { get; set; }

        [Column(TypeName = "json")]
        public string DatosJson { get; set; }

        public DateTime FechaHora { get; set; }

        public EventoActividadModel EventoActividad { get; set; }
        public EventoParticipanteModel EventoParticipante { get; set; }
    }

}
