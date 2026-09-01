using MediatR;
using pharmamatch.Application.Interfaces;
using pharmamatch.Domain.Entities;

namespace pharmamatch.Application.Features.ProductMedicines.Commands
{
    public record AddMedicineWithBatchCommand(
        string TradeName,
        decimal Price,
        int CategoryId,
        int IngredientId,
        int InitialQuantity,
        DateTime ExpiryDate,
        string? BatchNumber) : IRequest<bool>;

    public class AddMedicineWithBatchCommandHandler : IRequestHandler<AddMedicineWithBatchCommand, bool>
    {
        private readonly IProductMedicineRepository _medicineRepository;

        public AddMedicineWithBatchCommandHandler(IProductMedicineRepository medicineRepository)
        {
            _medicineRepository = medicineRepository;
        }

        public async Task<bool> Handle(AddMedicineWithBatchCommand request, CancellationToken cancellationToken)
        {
            var medicine = new ProductMedicine
            {
                TradeName = request.TradeName.Trim(),
                Price = request.Price,
                IngredientId = request.IngredientId,
                IsWatchlist = true,
                LastUpdated = DateTime.Now
            };

            var batch = new InventoryBatch
            {
                BatchNumber = string.IsNullOrWhiteSpace(request.BatchNumber) ? $"BN-NEW-{Guid.NewGuid().ToString().Substring(0, 5)}" : request.BatchNumber.Trim(),
                Quantity = request.InitialQuantity,
                ExpiryDate = request.ExpiryDate
            };

            await _medicineRepository.AddWithBatchAsync(medicine, batch);
            return true;
        }
    }
}
