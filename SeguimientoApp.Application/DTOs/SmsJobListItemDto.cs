namespace SeguimientoApp.Application.DTOs
{
    public class SmsJobListItemDto
    {
        public long JobId { get; set; }
        public string Message { get; set; }
        public int Total { get; set; }
        public int Sent { get; set; }
        public int Failed { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
