using SeguimientoApp.Application.DTOs;
using SeguimientoApp.Application.Ports.Persistence;

namespace SeguimientoApp.Application.UseCases.Catalogos
{
    public class GetLsCatalogo(ICatalogoRepositoryPort repo)
    {
        private readonly ICatalogoRepositoryPort _repo = repo;

        public Task<List<CatalogoItemDto>> ExecuteAsync(int tipoCatalogo, CancellationToken ct = default)
            => _repo.GetLsCatalogoAsync(tipoCatalogo, ct);
    }
}
