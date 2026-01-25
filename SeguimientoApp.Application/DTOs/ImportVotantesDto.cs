using SeguimientoApp.Domain.Enums;

namespace SeguimientoApp.Application.DTOs
{
    public record ImportVotanteRowResult(
        long? Documento,
        bool PersonaCreada,
        bool RelacionCreada,
        ImportRowStatus Status,
        string Motivo,
        long? LiderActualDocumento = null,
        string? LiderActualNombre = null
    );

    public record ImportVotantesResult(
        bool Ok,
        string? Error,
        long DocumentoLider,
        string? NombreLider,
        int PersonasCreadas,
        int RelacionesCreadas,
        int Omitidos,
        List<ImportVotanteRowResult> Rows
    );
}
