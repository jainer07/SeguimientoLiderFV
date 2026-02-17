namespace SeguimientoApp.Infrastructure.Notificacion.Onurix.Models
{
    public class OnurixSendSmsResponse
    {
        public string? id {get;set; }
        public int status {get;set; }
        public OnurixData? data {get;set; }
        public int? error {get;set; }
        public string? msg { get; set; }
    }
}
