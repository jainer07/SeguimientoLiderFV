namespace SeguimientoApp.Application.DTOs
{
    public class SmsBulkResultDto
    {
        public int Total { get; set; }
        public int Ok { get; set; }
        public int Fail { get; set; }
        public List<string> Errores { get; set; } = new();
    }
}
