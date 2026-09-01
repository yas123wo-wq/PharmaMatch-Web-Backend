using MediatR;
using pharmamatch.Application.DTOs;
using pharmamatch.Application.Interfaces;

namespace pharmamatch.Application.Features.ActiveIngredients.Queries
{
    /// <summary>
    /// استعلام جلب مادة فعالة محددة بواسطة المعرف (CQRS Query)
    /// </summary>
    public record GetActiveIngredientByIdQuery(int Id) : IRequest<ActiveIngredientDto?>;

    public class GetActiveIngredientByIdQueryHandler : IRequestHandler<GetActiveIngredientByIdQuery, ActiveIngredientDto?>
    {
        private readonly IActiveIngredientRepository _repository;

        public GetActiveIngredientByIdQueryHandler(IActiveIngredientRepository repository)
        {
            _repository = repository;
        }

        public async Task<ActiveIngredientDto?> Handle(GetActiveIngredientByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            return entity?.ToDto();
        }
    }
}
