using MediatR;
using pharmamatch.Application.DTOs;
using pharmamatch.Application.Interfaces;

namespace pharmamatch.Application.Features.InventoryBatches.Queries
{
    /// <summary>
    /// استعلام جلب دفعة مخزون محددة بواسطة المعرف (CQRS Query)
    /// </summary>
    public record GetInventoryBatchByIdQuery(int Id) : IRequest<InventoryBatchDto?>;

    public class GetInventoryBatchByIdQueryHandler : IRequestHandler<GetInventoryBatchByIdQuery, InventoryBatchDto?>
    {
        private readonly IInventoryBatchRepository _repository;

        public GetInventoryBatchByIdQueryHandler(IInventoryBatchRepository repository)
        {
            _repository = repository;
        }

        public async Task<InventoryBatchDto?> Handle(GetInventoryBatchByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            return entity?.ToDto();
        }
    }
}
