using MediatR;
using pharmamatch.Application.DTOs;
using pharmamatch.Application.Interfaces;

namespace pharmamatch.Application.Features.ProductMedicines.Queries
{
    /// <summary>
    /// استعلام جلب دواء محدد بواسطة المعرف (CQRS Query)
    /// </summary>
    public record GetProductMedicineByIdQuery(int Id) : IRequest<ProductMedicineDto?>;

    public class GetProductMedicineByIdQueryHandler : IRequestHandler<GetProductMedicineByIdQuery, ProductMedicineDto?>
    {
        private readonly IProductMedicineRepository _repository;

        public GetProductMedicineByIdQueryHandler(IProductMedicineRepository repository)
        {
            _repository = repository;
        }

        public async Task<ProductMedicineDto?> Handle(GetProductMedicineByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            return entity?.ToDto();
        }
    }
}
