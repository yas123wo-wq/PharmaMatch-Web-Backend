using MediatR;
using pharmamatch.Application.DTOs;
using pharmamatch.Application.Interfaces;

namespace pharmamatch.Application.Features.ProductMedicines.Queries
{
    public class WatchlistDataDto
    {
        public IEnumerable<ProductMedicineDto> FilteredMedicines { get; set; } = new List<ProductMedicineDto>();
        public int FavoriteCount { get; set; }
        public int SevereShortageCount { get; set; }
        public int StableStockCount { get; set; }
    }

    public record GetWatchlistDataQuery(string? Search) : IRequest<WatchlistDataDto>;

    public class GetWatchlistDataQueryHandler : IRequestHandler<GetWatchlistDataQuery, WatchlistDataDto>
    {
        private readonly IProductMedicineRepository _medicineRepository;

        public GetWatchlistDataQueryHandler(IProductMedicineRepository medicineRepository)
        {
            _medicineRepository = medicineRepository;
        }

        public async Task<WatchlistDataDto> Handle(GetWatchlistDataQuery request, CancellationToken cancellationToken)
        {
            var filteredEntities = (await _medicineRepository.GetWatchlistAsync(request.Search)).ToList();
            var allWatchlist = (await _medicineRepository.GetWatchlistAsync(null)).ToList();

            int favoriteCount = allWatchlist.Count;
            int severeShortageCount = allWatchlist.Count(m => m.InventoryBatches.Sum(b => b.Quantity) <= 50);
            int stableStockCount = allWatchlist.Count(m => m.InventoryBatches.Sum(b => b.Quantity) > 100);

            return new WatchlistDataDto
            {
                FilteredMedicines = filteredEntities.Select(m => m.ToDto()!),
                FavoriteCount = favoriteCount,
                SevereShortageCount = severeShortageCount,
                StableStockCount = stableStockCount
            };
        }
    }
}
