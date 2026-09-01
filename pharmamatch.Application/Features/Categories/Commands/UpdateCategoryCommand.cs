using MediatR;
using pharmamatch.Application.DTOs;
using pharmamatch.Application.Interfaces;

namespace pharmamatch.Application.Features.Categories.Commands
{
    /// <summary>
    /// أمر تعديل بيانات فئة دوائية (CQRS Command)
    /// </summary>
    public record UpdateCategoryCommand(int Id, CategoryDto Category) : IRequest<CategoryDto>;

    public class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand, CategoryDto>
    {
        private readonly ICategoryRepository _repository;

        public UpdateCategoryCommandHandler(ICategoryRepository repository)
        {
            _repository = repository;
        }

        public async Task<CategoryDto> Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
        {
            var entity = request.Category.ToEntity();
            entity.Id = request.Id;
            var updatedEntity = await _repository.UpdateAsync(entity);
            return updatedEntity.ToDto()!;
        }
    }
}
