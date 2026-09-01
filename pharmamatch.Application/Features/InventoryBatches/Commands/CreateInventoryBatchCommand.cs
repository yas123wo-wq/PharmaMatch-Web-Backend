using MediatR;
using pharmamatch.Application.DTOs;
using pharmamatch.Application.Interfaces;

namespace pharmamatch.Application.Features.InventoryBatches.Commands
{
    /// <summary>
    /// أمر إضافة دفعة مخزون جديدة (CQRS Command)
    /// </summary>
    public record CreateInventoryBatchCommand(InventoryBatchDto Batch) : IRequest<InventoryBatchDto>;

    public class CreateInventoryBatchCommandHandler : IRequestHandler<CreateInventoryBatchCommand, InventoryBatchDto>
    {
        private readonly IInventoryBatchRepository _repository;

        public CreateInventoryBatchCommandHandler(IInventoryBatchRepository repository)
        {
            _repository = repository;
        }

        public async Task<InventoryBatchDto> Handle(CreateInventoryBatchCommand request, CancellationToken cancellationToken)
        {
            var entity = request.Batch.ToEntity();
            var createdEntity = await _repository.CreateAsync(entity);
            return createdEntity.ToDto()!;
        }
    }
}
