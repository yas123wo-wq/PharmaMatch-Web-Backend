using MediatR;
using pharmamatch.Application.Interfaces;
using pharmamatch.Domain.Entities;

namespace pharmamatch.Application.Features.ProductMedicines.Commands
{
    public record UpdateMedicineWithBatchCommand(
        int Id,
        string TradeName,
        decimal Price,
        int CategoryId,
        int IngredientId,
        int TotalQuantity,
        string? BatchNumber,
        DateTime ExpiryDate) : IRequest<bool>;

    public class UpdateMedicineWithBatchCommandHandler : IRequestHandler<UpdateMedicineWithBatchCommand, bool>
    {
        private readonly IProductMedicineRepository _medicineRepository;

        public UpdateMedicineWithBatchCommandHandler(IProductMedicineRepository medicineRepository)
        {
            _medicineRepository = medicineRepository;
        }

        public async Task<bool> Handle(UpdateMedicineWithBatchCommand request, CancellationToken cancellationToken)
        {
            var medicine = new ProductMedicine
            {
                Id = request.Id,
                TradeName = request.TradeName.Trim(),
                Price = request.Price,
                IngredientId = request.IngredientId
            };

            await _medicineRepository.UpdateWithBatchAsync(medicine, request.TotalQuantity, request.BatchNumber, request.ExpiryDate);
            return true;
        }
    }
}
