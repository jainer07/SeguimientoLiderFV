namespace SeguimientoApp.Application.DTOs
{
    public class OnurixOptions
    {
        public string BaseUrl { get; set; } = "https://www.onurix.com";
        public long Client { get; set; }
        public string Key { get; set; } = "";
    }
}
