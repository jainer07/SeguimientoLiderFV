using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeguimientoApp.Infrastructure.Persistence.MySql.Models
{
    [Table("tbl_evento_actividad")]
    public class EventoActividadModel
    {
        [Key]
        public int IdEventoActividad { get; set; }

        public int IdEvento { get; set; }
        public int IdActividadPlantilla { get; set; }

        public int Orden { get; set; }
        public int IdAplicaACat { get; set; }

        public bool EsObligatoria { get; set; }
        public bool ReglaCierre { get; set; }

        [Column(TypeName = "json")]
        public string ParametrosJson { get; set; }

        public EventoModel Evento { get; set; }
        public ActividadPlantillaModel ActividadPlantilla { get; set; }
    }

}
