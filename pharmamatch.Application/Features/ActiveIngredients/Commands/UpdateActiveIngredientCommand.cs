using MediatR;
using pharmamatch.Application.DTOs;
using pharmamatch.Application.Interfaces;

namespace pharmamatch.Application.Features.ActiveIngredients.Commands
{
    /// <summary>
    /// أمر تعديل بيانات مادة فعالة (CQRS Command)
    /// </summary>
    public record UpdateActiveIngredientCommand(int Id, ActiveIngredientDto Ingredient) : IRequest<ActiveIngredientDto>;

    public class UpdateActiveIngredientCommandHandler : IRequestHandler<UpdateActiveIngredientCommand, ActiveIngredientDto>
    {
        private readonly IActiveIngredientRepository _repository;

        public UpdateActiveIngredientCommandHandler(IActiveIngredientRepository repository)
        {
            _repository = repository;
        }

        public async Task<ActiveIngredientDto> Handle(UpdateActiveIngredientCommand request, CancellationToken cancellationToken)
        {
            var entity = request.Ingredient.ToEntity();
            entity.Id = request.Id;
            var updatedEntity = await _repository.UpdateAsync(entity);
            return updatedEntity.ToDto()!;
        }
    }
}
