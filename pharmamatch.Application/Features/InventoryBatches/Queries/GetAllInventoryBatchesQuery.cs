using MediatR;
using pharmamatch.Application.DTOs;
using pharmamatch.Application.Interfaces;

namespace pharmamatch.Application.Features.InventoryBatches.Queries
{
    /// <summary>
    /// استعلام جلب كافة دفعات المخزون (CQRS Query)
    /// </summary>
    public record GetAllInventoryBatchesQuery() : IRequest<IEnumerable<InventoryBatchDto>>;

    public class GetAllInventoryBatchesQueryHandler : IRequestHandler<GetAllInventoryBatchesQuery, IEnumerable<InventoryBatchDto>>
    {
        private readonly IInventoryBatchRepository _repository;

        public GetAllInventoryBatchesQueryHandler(IInventoryBatchRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<InventoryBatchDto>> Handle(GetAllInventoryBatchesQuery request, CancellationToken cancellationToken)
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToDto()!);
        }
    }
}
