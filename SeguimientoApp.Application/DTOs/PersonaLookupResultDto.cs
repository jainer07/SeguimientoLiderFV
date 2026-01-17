namespace SeguimientoApp.Application.DTOs
{
    public class PersonaLookupResultDto
    {
        // AVAILABLE | ASSIGNED | NOT_FOUND | SELF | INACTIVE
        public string Code { get; set; } = "NOT_FOUND";

        public PersonaMiniDto? Persona { get; set; }
        public PersonaMiniDto? LiderActual { get; set; }
    }
}
