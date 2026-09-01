using MediatR;
using pharmamatch.Application.DTOs;
using pharmamatch.Application.Interfaces;

namespace pharmamatch.Application.Features.Categories.Commands
{
    /// <summary>
    /// أمر إضافة فئة دوائية جديدة (CQRS Command)
    /// </summary>
    public record CreateCategoryCommand(CategoryDto Category) : IRequest<CategoryDto>;

    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
    {
        private readonly ICategoryRepository _repository;

        public CreateCategoryCommandHandler(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
        {
            var entity = request.Category.ToEntity();
            var createdEntity = await _repository.CreateAsync(entity);
            return createdEntity.ToDto()!;
        }
    }
}
