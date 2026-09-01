using MediatR;
using pharmamatch.Application.DTOs;
using pharmamatch.Application.Interfaces;
using pharmamatch.Domain.Entities;

namespace pharmamatch.Application.Features.ProductMedicines.Queries
{
    public class SearchMedicinesResultDto
    {
        public IEnumerable<CategoryDto> Categories { get; set; } = new List<CategoryDto>();
        public IEnumerable<ProductMedicineDto> Results { get; set; } = new List<ProductMedicineDto>();
        public IEnumerable<ProductMedicineDto> Alternatives { get; set; } = new List<ProductMedicineDto>();
    }

    public record SearchMedicinesQuery(string? Query, int? CategoryId, string? StatusFilter) : IRequest<SearchMedicinesResultDto>;

    public class SearchMedicinesQueryHandler : IRequestHandler<SearchMedicinesQuery, SearchMedicinesResultDto>
    {
        private readonly IProductMedicineRepository _medicineRepository;
        private readonly ICategoryRepository _categoryRepository;

        public SearchMedicinesQueryHandler(
            IProductMedicineRepository medicineRepository,
            ICategoryRepository categoryRepository)
        {
            _medicineRepository = medicineRepository;
            _categoryRepository = categoryRepository;
        }

        public async Task<SearchMedicinesResultDto> Handle(SearchMedicinesQuery request, CancellationToken cancellationToken)
        {
            var categories = await _categoryRepository.GetAllAsync();
            var resultsEntities = (await _medicineRepository.SearchMedicinesAsync(request.Query, request.CategoryId, request.StatusFilter)).ToList();

            List<ProductMedicine> alternativesEntities = new List<ProductMedicine>();
            if (resultsEntities.Any())
            {
                var searchedIngredientIds = resultsEntities.Select(m => m.IngredientId).Distinct().ToList();
                var searchedMedicineIds = resultsEntities.Select(m => m.Id).ToList();

                alternativesEntities = (await _medicineRepository.GetAlternativesForIngredientsAsync(searchedIngredientIds, searchedMedicineIds)).ToList();
            }

            return new SearchMedicinesResultDto
            {
                Categories = categories.Select(c => c.ToDto()!),
                Results = resultsEntities.Select(r => r.ToDto()!),
                Alternatives = alternativesEntities.Select(a => a.ToDto()!)
            };
        }
    }
}
