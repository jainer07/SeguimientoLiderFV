using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Domain.Enums;


namespace SeguimientoApp.Application.Ports.Persistence
{
    public interface IPersonaRepositoryPort
    {
        Task<List<PersonaDto>> GetAllAsync(CancellationToken ct = default);
        Task<PersonaEditDto?> GetByIdAsync(long idPersona, CancellationToken ct = default);
        Task<bool> ExistsByDocumentoAsync(int tipoDocumento, long numeroDocumento, CancellationToken ct = default);
        Task ToggleEstadoAsync(long idPersona, CancellationToken ct = default);
        Task CreateAsync(PersonaCreateDto dto, CancellationToken ct = default);
        Task UpdateAsync(PersonaEditDto dto, CancellationToken ct = default);
        Task<PersonaLookupResultDto> LookupByCedulaAsync(long idLider, long cedula, CancellationToken ct = default);
        Task<PersonaDetailsDto?> GetDetailsAsync(long idPersona, CancellationToken ct = default);
        Task<PersonaLiderAssignResult> AddPersonaToLiderAsync(long idLider, long idPersona, CancellationToken ct = default);
        Task RemovePersonaFromLiderAsync(long idLider, long idPersona, CancellationToken ct = default);

    }
}
