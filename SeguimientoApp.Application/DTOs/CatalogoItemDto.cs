namespace SeguimientoApp.Application.DTOs
{
    public class CatalogoItemDto
    {
        public int IdCatalogo { get; set; }
        public string Nombre { get; set; } = "";
        public string Codigo { get; set; } = "";
        public string Valor { get; set; } = "";
        public bool Estado { get; set; }
    }
}
