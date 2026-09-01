namespace pharmamatch.Application.DTOs
{
    public class InventoryBatchDto
    {
        public int Id { get; set; }
        public string BatchNumber { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public DateTime ExpiryDate { get; set; }
        public int MedicineId { get; set; }
        public string? TradeName { get; set; }
    }
}
