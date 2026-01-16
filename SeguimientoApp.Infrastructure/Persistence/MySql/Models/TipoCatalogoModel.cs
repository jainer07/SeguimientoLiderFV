using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeguimientoApp.Infrastructure.Persistence.MySql.Models
{
    [Table("tbl_tipocatalogo")]
    public class TipoCatalogoModel
    {
        [Key]
        [Column("IdTipoCatalogo")]
        public int IdTipoCatalogo { get; set; }

        [Column("Nombre")]
        public string Nombre { get; set; }

        public List<CatalogoModel> Catalogos { get; set; }
    }
}
