namespace SeguimientoApp.Application.DTOs
{
    public class SmsOutboxItemDto
    {
        public long Id { get; set; }
        public long JobId { get; set; }
        public string Phone { get; set; }
        public int Intentos { get; set; }
        public string Message { get; set; }
    }
}
