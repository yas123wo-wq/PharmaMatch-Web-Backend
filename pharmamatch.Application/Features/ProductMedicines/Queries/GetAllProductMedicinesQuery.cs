using MediatR;
using pharmamatch.Application.DTOs;
using pharmamatch.Application.Interfaces;

namespace pharmamatch.Application.Features.ProductMedicines.Queries
{
    /// <summary>
    /// استعلام جلب كافة الأدوية المتوفرة (CQRS Query)
    /// </summary>
    public record GetAllProductMedicinesQuery() : IRequest<IEnumerable<ProductMedicineDto>>;

    public class GetAllProductMedicinesQueryHandler : IRequestHandler<GetAllProductMedicinesQuery, IEnumerable<ProductMedicineDto>>
    {
        private readonly IProductMedicineRepository _repository;

        public GetAllProductMedicinesQueryHandler(IProductMedicineRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ProductMedicineDto>> Handle(GetAllProductMedicinesQuery request, CancellationToken cancellationToken)
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToDto()!);
        }
    }
}
