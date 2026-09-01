using MediatR;
using pharmamatch.Application.Interfaces;

namespace pharmamatch.Application.Features.ActiveIngredients.Commands
{
    /// <summary>
    /// أمر حذف مادة فعالة (CQRS Command)
    /// </summary>
    public record DeleteActiveIngredientCommand(int Id) : IRequest<bool>;

    public class DeleteActiveIngredientCommandHandler : IRequestHandler<DeleteActiveIngredientCommand, bool>
    {
        private readonly IActiveIngredientRepository _repository;

        public DeleteActiveIngredientCommandHandler(IActiveIngredientRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> Handle(DeleteActiveIngredientCommand request, CancellationToken cancellationToken)
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
