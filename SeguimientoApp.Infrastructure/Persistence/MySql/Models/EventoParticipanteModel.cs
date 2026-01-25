using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeguimientoApp.Infrastructure.Persistence.MySql.Models
{
    [Table("tbl_evento_participante")]
    public class EventoParticipanteModel
    {
        [Key]
        public int IdEventoParticipante { get; set; }

        public int IdEvento { get; set; }
        public long IdPersona { get; set; }

        public int IdRolCat { get; set; }

        public EventoModel Evento { get; set; }
        public PersonaModel Persona { get; set; }
    }

}
