using MediatR;
using pharmamatch.Application.DTOs;
using pharmamatch.Application.Interfaces;

namespace pharmamatch.Application.Features.ActiveIngredients.Commands
{
    /// <summary>
    /// أمر إضافة مادة فعالة جديدة (CQRS Command)
    /// </summary>
    public record CreateActiveIngredientCommand(ActiveIngredientDto Ingredient) : IRequest<ActiveIngredientDto>;

    public class CreateActiveIngredientCommandHandler : IRequestHandler<CreateActiveIngredientCommand, ActiveIngredientDto>
    {
        private readonly IActiveIngredientRepository _repository;

        public CreateActiveIngredientCommandHandler(IActiveIngredientRepository repository)
        {
            _repository = repository;
        }

        public async Task<ActiveIngredientDto> Handle(CreateActiveIngredientCommand request, CancellationToken cancellationToken)
        {
            var entity = request.Ingredient.ToEntity();
            var createdEntity = await _repository.CreateAsync(entity);
            return createdEntity.ToDto()!;
        }
    }
}
