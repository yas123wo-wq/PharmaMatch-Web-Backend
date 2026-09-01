using MediatR;
using pharmamatch.Application.DTOs;
using pharmamatch.Application.Interfaces;

namespace pharmamatch.Application.Features.ProductMedicines.Queries
{
    /// <summary>
    /// استعلام البحث الذكي عن الأدوية البديلة التي تشترك في نفس المادة الفعالة (CQRS Query)
    /// </summary>
    public record GetMedicineAlternativesQuery(int MedicineId) : IRequest<IEnumerable<ProductMedicineDto>>;

    public class GetMedicineAlternativesQueryHandler : IRequestHandler<GetMedicineAlternativesQuery, IEnumerable<ProductMedicineDto>>
    {
        private readonly IProductMedicineRepository _repository;

        public GetMedicineAlternativesQueryHandler(IProductMedicineRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ProductMedicineDto>> Handle(GetMedicineAlternativesQuery request, CancellationToken cancellationToken)
        {
            var entities = await _repository.GetAlternativesAsync(request.MedicineId);
            return entities.Select(e => e.ToDto()!);
        }
    }
}
