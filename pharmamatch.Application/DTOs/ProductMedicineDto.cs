namespace pharmamatch.Application.DTOs
{
    public class ProductMedicineDto
    {
        public int Id { get; set; }
        public string TradeName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string ImagePath { get; set; } = string.Empty;
        public int IngredientId { get; set; }
        public string? ScientificName { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public bool IsWatchlist { get; set; }
        public DateTime LastUpdated { get; set; }

        public List<InventoryBatchDto> InventoryBatches { get; set; } = new();
        public int TotalQuantity => InventoryBatches.Sum(b => b.Quantity);
        public DateTime? NearestExpiryDate => InventoryBatches.OrderBy(b => b.ExpiryDate).FirstOrDefault()?.ExpiryDate;
        public string? NearestBatchNumber => InventoryBatches.OrderBy(b => b.ExpiryDate).FirstOrDefault()?.BatchNumber;
    }
}

