using SeguimientoApp.Application.DTOs;

namespace SeguimientoApp.Application.Ports.Persistence
{
    public interface ICatalogoRepositoryPort
    {
        Task<List<CatalogoItemDto>> GetLsCatalogoAsync(int tipoCatalogo, CancellationToken ct = default);
    }
}
