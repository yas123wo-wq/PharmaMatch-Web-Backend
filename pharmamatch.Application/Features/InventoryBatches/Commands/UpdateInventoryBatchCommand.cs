using MediatR;
using pharmamatch.Application.DTOs;
using pharmamatch.Application.Interfaces;

namespace pharmamatch.Application.Features.InventoryBatches.Commands
{
    /// <summary>
    /// أمر تعديل بيانات دفعة مخزون (CQRS Command)
    /// </summary>
    public record UpdateInventoryBatchCommand(int Id, InventoryBatchDto Batch) : IRequest<InventoryBatchDto>;

    public class UpdateInventoryBatchCommandHandler : IRequestHandler<UpdateInventoryBatchCommand, InventoryBatchDto>
    {
        private readonly IInventoryBatchRepository _repository;

        public UpdateInventoryBatchCommandHandler(IInventoryBatchRepository repository)
        {
            _repository = repository;
        }

        public async Task<InventoryBatchDto> Handle(UpdateInventoryBatchCommand request, CancellationToken cancellationToken)
        {
            var entity = request.Batch.ToEntity();
            entity.Id = request.Id;
            var updatedEntity = await _repository.UpdateAsync(entity);
            return updatedEntity.ToDto()!;
        }
    }
}
