using MediatR;
using pharmamatch.Application.DTOs;
using pharmamatch.Application.Interfaces;

namespace pharmamatch.Application.Features.ActiveIngredients.Queries
{
    /// <summary>
    /// استعلام جلب كافة المواد الفعالة (CQRS Query)
    /// </summary>
    public record GetAllActiveIngredientsQuery() : IRequest<IEnumerable<ActiveIngredientDto>>;

    public class GetAllActiveIngredientsQueryHandler : IRequestHandler<GetAllActiveIngredientsQuery, IEnumerable<ActiveIngredientDto>>
    {
        private readonly IActiveIngredientRepository _repository;

        public GetAllActiveIngredientsQueryHandler(IActiveIngredientRepository repository)
        {
            _repository = repository;
        }

        public async Task<IEnumerable<ActiveIngredientDto>> Handle(GetAllActiveIngredientsQuery request, CancellationToken cancellationToken)
        {
            var entities = await _repository.GetAllAsync();
            return entities.Select(e => e.ToDto()!);
        }
    }
}
