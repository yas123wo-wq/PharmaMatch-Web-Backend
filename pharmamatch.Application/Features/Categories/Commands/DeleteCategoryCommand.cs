using MediatR;
using pharmamatch.Application.Interfaces;

namespace pharmamatch.Application.Features.Categories.Commands
{
    /// <summary>
    /// أمر حذف فئة دوائية (CQRS Command)
    /// </summary>
    public record DeleteCategoryCommand(int Id) : IRequest<bool>;

    public class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand, bool>
    {
        private readonly ICategoryRepository _repository;

        public DeleteCategoryCommandHandler(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
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
