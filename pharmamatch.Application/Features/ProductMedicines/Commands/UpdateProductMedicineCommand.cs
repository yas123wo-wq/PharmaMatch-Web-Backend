using MediatR;
using pharmamatch.Application.DTOs;
using pharmamatch.Application.Interfaces;

namespace pharmamatch.Application.Features.ProductMedicines.Commands
{
    /// <summary>
    /// أمر تعديل بيانات دواء قائمة (CQRS Command)
    /// </summary>
    public record UpdateProductMedicineCommand(int Id, ProductMedicineDto Medicine) : IRequest<ProductMedicineDto>;

    public class UpdateProductMedicineCommandHandler : IRequestHandler<UpdateProductMedicineCommand, ProductMedicineDto>
    {
        private readonly IProductMedicineRepository _repository;

        public UpdateProductMedicineCommandHandler(IProductMedicineRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProductMedicineDto> Handle(UpdateProductMedicineCommand request, CancellationToken cancellationToken)
        {
            var entity = request.Medicine.ToEntity();
            entity.Id = request.Id;
            var updatedEntity = await _repository.UpdateAsync(entity);
            return updatedEntity.ToDto()!;
        }
    }
}
