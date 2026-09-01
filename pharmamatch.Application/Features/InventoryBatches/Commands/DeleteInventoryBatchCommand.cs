using MediatR;
using pharmamatch.Application.Interfaces;

namespace pharmamatch.Application.Features.InventoryBatches.Commands
{
    /// <summary>
    /// أمر حذف دفعة مخزون (CQRS Command)
    /// </summary>
    public record DeleteInventoryBatchCommand(int Id) : IRequest<bool>;

    public class DeleteInventoryBatchCommandHandler : IRequestHandler<DeleteInventoryBatchCommand, bool>
    {
        private readonly IInventoryBatchRepository _repository;

        public DeleteInventoryBatchCommandHandler(IInventoryBatchRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteInventoryBatchCommand request, CancellationToken cancellationToken)
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
