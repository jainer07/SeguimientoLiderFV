using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeguimientoApp.Infrastructure.Persistence.MySql.Models
{
    [Table("tbl_catalogo")]
    public class CatalogoModel
    {
        [Key]
        [Column("IdCatalogo")]
        public int IdCatalogo { get; set; }

        [Column("IdTipoCatalogo")]
        public int IdTipoCatalogo { get; set; }

        [Column("Nombre")]
        public string Nombre { get; set; }

        [Column("Valor")]
        public string Valor { get; set; }

        [Column("Codigo")]
        public string Codigo { get; set; }

        [Column("Estado")]
        public bool Estado { get; set; }

        public TipoCatalogoModel TipoCatalogo { get; set; }
        public List<PersonaModel> Personas { get; set; }
    }
}
