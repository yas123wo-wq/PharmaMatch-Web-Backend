using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pharmamatch.Domain.Entities
{
    public class ProductMedicine
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "الاسم التجاري مطلوب")]
        [StringLength(200, ErrorMessage = "الاسم التجاري يجب ألا يتجاوز 200 حرف")]
        public string TradeName { get; set; } = string.Empty;

        [Required(ErrorMessage = "السعر مطلوب")]
        [Column(TypeName = "decimal(18,2)")]
        [Range(0, 999999.99, ErrorMessage = "السعر يجب أن يكون أكبر من أو يساوي 0")]
        public decimal Price { get; set; }

        [StringLength(500, ErrorMessage = "مسار الصورة يجب ألا يتجاوز 500 حرف")]
        public string ImagePath { get; set; } = string.Empty;

        [Required]
        [ForeignKey(nameof(ActiveIngredient))]
        public int IngredientId { get; set; }

        public ActiveIngredient? ActiveIngredient { get; set; }

        public bool IsWatchlist { get; set; } = false;

        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public ICollection<InventoryBatch> InventoryBatches { get; set; } = new List<InventoryBatch>();
    }
}
