using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SeguimientoApp.Infrastructure.Persistence.MySql.Models
{
    [Table("tbl_persona")]
    public class PersonaModel
    {
        [Key]
        [Column("IdPersona")]
        public long IdPersona { get; set; }

        [Column("IdCliente")]
        public int IdCliente { get; set; }

        [Column("IdTipoDocumento")]
        public int IdTipoDocumento { get; set; }

        [Column("NumeroDocumento")]
        public long NumeroDocumento { get; set; }

        [Column("EsLider")]
        public bool EsLider { get; set; }

        [Column("PrimerNombre")]
        public string PrimerNombre { get; set; }

        [Column("SegundoNombre")]
        public string SegundoNombre { get; set; }

        [Column("PrimerApellido")]
        public string PrimerApellido { get; set; }

        [Column("SegundoApellido")]
        public string SegundoApellido { get; set; }

        [Column("Celular")]
        public long Celular { get; set; }

        [Column("Correo")]
        public string Correo { get; set; }

        [Column("Direccion")]
        public string Direccion { get; set; }

        [Column("AceptaPoliticaDatos")]
        public bool AceptaPoliticaDatos { get; set; }

        [Column("LugarVotacion")]
        public string LugarVotacion { get; set; }

        [Column("Mesa")]
        public string Mesa { get; set; }

        [Column("Estado")]
        public bool Estado { get; set; }

        public CatalogoModel CatalogoTipoDocumento { get; set; }
        //public List<LiderModel> Lider { get; set; }
        //public List<PersonaLiderModel> PersonaLider { get; set; }
        //public List<PersonaLugarVotacionModel> PersonaLugarVota { get; set; }
    }
}
