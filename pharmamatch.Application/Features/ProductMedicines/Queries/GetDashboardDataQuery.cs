using MediatR;
using pharmamatch.Application.DTOs;
using pharmamatch.Application.Interfaces;

namespace pharmamatch.Application.Features.ProductMedicines.Queries
{
    public class DashboardDataDto
    {
        public int TotalStock { get; set; }
        public int CriticalItemsCount { get; set; }
        public IEnumerable<ProductMedicineDto> Medicines { get; set; } = new List<ProductMedicineDto>();
        public IEnumerable<InventoryBatchDto> ExpiredBatches { get; set; } = new List<InventoryBatchDto>();
    }

    public record GetDashboardDataQuery() : IRequest<DashboardDataDto>;

    public class GetDashboardDataQueryHandler : IRequestHandler<GetDashboardDataQuery, DashboardDataDto>
    {
        private readonly IProductMedicineRepository _medicineRepository;
        private readonly IInventoryBatchRepository _batchRepository;

        public GetDashboardDataQueryHandler(
            IProductMedicineRepository medicineRepository,
            IInventoryBatchRepository batchRepository)
        {
            _medicineRepository = medicineRepository;
            _batchRepository = batchRepository;
        }

        public async Task<DashboardDataDto> Handle(GetDashboardDataQuery request, CancellationToken cancellationToken)
        {
            int totalStock = await _batchRepository.GetTotalStockAsync();
            int criticalItemsCount = await _medicineRepository.GetCriticalItemsCountAsync();
            var medicinesEntities = await _medicineRepository.GetAllAsync();
            var expiredBatchesEntities = await _batchRepository.GetExpiredBatchesAsync();

            return new DashboardDataDto
            {
                TotalStock = totalStock,
                CriticalItemsCount = criticalItemsCount,
                Medicines = medicinesEntities.Select(m => m.ToDto()!),
                ExpiredBatches = expiredBatchesEntities.Select(b => b.ToDto()!)
            };
        }
    }
}
