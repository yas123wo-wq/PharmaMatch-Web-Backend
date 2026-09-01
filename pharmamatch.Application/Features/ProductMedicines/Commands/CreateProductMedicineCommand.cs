using MediatR;
using pharmamatch.Application.DTOs;
using pharmamatch.Application.Interfaces;

namespace pharmamatch.Application.Features.ProductMedicines.Commands
{
    /// <summary>
    /// أمر إنشاء دواء جديد (CQRS Command)
    /// </summary>
    public record CreateProductMedicineCommand(ProductMedicineDto Medicine) : IRequest<ProductMedicineDto>;

    public class CreateProductMedicineCommandHandler : IRequestHandler<CreateProductMedicineCommand, ProductMedicineDto>
    {
        private readonly IProductMedicineRepository _repository;

        public CreateProductMedicineCommandHandler(IProductMedicineRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProductMedicineDto> Handle(CreateProductMedicineCommand request, CancellationToken cancellationToken)
        {
            var entity = request.Medicine.ToEntity();
            var createdEntity = await _repository.CreateAsync(entity);
            return createdEntity.ToDto()!;
        }
    }
}
