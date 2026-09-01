using pharmamatch.Domain.Entities;

namespace pharmamatch.Application.DTOs
{
    public static class MappingExtensions
    {
        // --- ActiveIngredient ---
        public static ActiveIngredientDto? ToDto(this ActiveIngredient? entity)
        {
            if (entity == null) return null;
            return new ActiveIngredientDto
            {
                Id = entity.Id,
                ScientificName = entity.ScientificName,
                CategoryId = entity.CategoryId,
                CategoryName = entity.Category?.CategoryName
            };
        }

        public static ActiveIngredient ToEntity(this ActiveIngredientDto dto)
        {
            return new ActiveIngredient
            {
                Id = dto.Id,
                ScientificName = dto.ScientificName,
                CategoryId = dto.CategoryId
            };
        }

        // --- Category ---
        public static CategoryDto? ToDto(this Category? entity)
        {
            if (entity == null) return null;
            return new CategoryDto
            {
                Id = entity.Id,
                CategoryName = entity.CategoryName,
                Description = entity.Description
            };
        }

        public static Category ToEntity(this CategoryDto dto)
        {
            return new Category
            {
                Id = dto.Id,
                CategoryName = dto.CategoryName,
                Description = dto.Description
            };
        }

        // --- ProductMedicine ---
        public static ProductMedicineDto? ToDto(this ProductMedicine? entity)
        {
            if (entity == null) return null;
            return new ProductMedicineDto
            {
                Id = entity.Id,
                TradeName = entity.TradeName,
                Price = entity.Price,
                ImagePath = entity.ImagePath,
                IngredientId = entity.IngredientId,
                ScientificName = entity.ActiveIngredient?.ScientificName,
                CategoryId = entity.ActiveIngredient?.CategoryId,
                CategoryName = entity.ActiveIngredient?.Category?.CategoryName,
                IsWatchlist = entity.IsWatchlist,
                LastUpdated = entity.LastUpdated,
                InventoryBatches = entity.InventoryBatches?.Select(b => b.ToDto()!).Where(b => b != null).ToList() ?? new List<InventoryBatchDto>()
            };
        }

        public static ProductMedicine ToEntity(this ProductMedicineDto dto)
        {
            return new ProductMedicine
            {
                Id = dto.Id,
                TradeName = dto.TradeName,
                Price = dto.Price,
                ImagePath = dto.ImagePath,
                IngredientId = dto.IngredientId,
                IsWatchlist = dto.IsWatchlist,
                LastUpdated = dto.LastUpdated
            };
        }

        // --- InventoryBatch ---
        public static InventoryBatchDto? ToDto(this InventoryBatch? entity)
        {
            if (entity == null) return null;
            return new InventoryBatchDto
            {
                Id = entity.Id,
                BatchNumber = entity.BatchNumber,
                Quantity = entity.Quantity,
                ExpiryDate = entity.ExpiryDate,
                MedicineId = entity.MedicineId,
                TradeName = entity.ProductMedicine?.TradeName
            };
        }

        public static InventoryBatch ToEntity(this InventoryBatchDto dto)
        {
            return new InventoryBatch
            {
                Id = dto.Id,
                BatchNumber = dto.BatchNumber,
                Quantity = dto.Quantity,
                ExpiryDate = dto.ExpiryDate,
                MedicineId = dto.MedicineId
            };
        }
    }
}
