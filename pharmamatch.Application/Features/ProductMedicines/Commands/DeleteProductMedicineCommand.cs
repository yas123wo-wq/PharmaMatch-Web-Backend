using MediatR;
using pharmamatch.Application.Interfaces;

namespace pharmamatch.Application.Features.ProductMedicines.Commands
{
    /// <summary>
    /// أمر حذف دواء من المنظومة (CQRS Command)
    /// </summary>
    public record DeleteProductMedicineCommand(int Id) : IRequest<bool>;

    public class DeleteProductMedicineCommandHandler : IRequestHandler<DeleteProductMedicineCommand, bool>
    {
        private readonly IProductMedicineRepository _repository;

        public DeleteProductMedicineCommandHandler(IProductMedicineRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteProductMedicineCommand request, CancellationToken cancellationToken)
        {
            if (!_repository.Exists(request.Id))
            {
                return false;
            }

            await _repository.DeleteAsync(request.Id);
            return true;
        }
    }
}
