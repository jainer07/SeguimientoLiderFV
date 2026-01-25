using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeguimientoApp.Infrastructure.Persistence.MySql.Models
{
    [Table("tbl_actividad_plantilla")]
    public class ActividadPlantillaModel
    {
        [Key]
        public int IdActividadPlantilla { get; set; }

        public string Nombre { get; set; }
        public int IdTipoRegistroCat { get; set; }
        public bool Estado { get; set; }

        public CatalogoModel TipoRegistro { get; set; }
    }

}
