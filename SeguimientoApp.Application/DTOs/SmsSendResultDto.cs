namespace SeguimientoApp.Application.DTOs
{
    public class SmsSendResultDto
    {
        public bool Ok { get; set; }
        public string? ProviderMessageId { get; set; }
        public string? ProviderState { get; set; }
        public string? ErrorMessage { get; set; }
    }
}
