using System.ComponentModel.DataAnnotations.Schema;

namespace SeguimientoApp.Infrastructure.Persistence.MySql.Models
{
    [Table("tbl_persona_lider")]
    public class PersonaLiderModel
    {
        [Column("IdLider")]
        public long IdLider { get; set; }

        [Column("IdPersona")]
        public long IdPersona { get; set; }

        public PersonaModel Lider { get; set; } = default!;
        public PersonaModel Persona { get; set; } = default!;
    }
}
