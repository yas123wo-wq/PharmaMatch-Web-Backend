using MediatR;
using pharmamatch.Application.DTOs;
using pharmamatch.Application.Interfaces;

namespace pharmamatch.Application.Features.Categories.Queries
{
    /// <summary>
    /// استعلام جلب فئة دوائية بواسطة المعرف (CQRS Query)
    /// </summary>
    public record GetCategoryByIdQuery(int Id) : IRequest<CategoryDto?>;

    public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
    {
        private readonly ICategoryRepository _repository;

        public GetCategoryByIdQueryHandler(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<CategoryDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
        {
            var entity = await _repository.GetByIdAsync(request.Id);
            return entity?.ToDto();
        }
    }
}
