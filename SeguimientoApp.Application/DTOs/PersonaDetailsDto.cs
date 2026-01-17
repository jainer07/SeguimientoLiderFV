namespace SeguimientoApp.Application.DTOs
{
    public class PersonaDetailsDto
    {
        public PersonaEditDto Persona { get; set; } = new();
        public List<PersonaMiniDto> PersonasACargo { get; set; } = new();
        public List<PersonaMiniDto> Candidatos { get; set; } = new();
    }
}
