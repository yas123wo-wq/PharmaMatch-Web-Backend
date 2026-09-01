using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pharmamatch.Domain.Entities
{
    public class InventoryBatch
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "رقم التشغيلة/الدفعة مطلوب")]
        [StringLength(100, ErrorMessage = "رقم التشغيلة يجب ألا يتجاوز 100 حرف")]
        public string BatchNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "الكمية مطلوبة")]
        [Range(0, int.MaxValue, ErrorMessage = "الكمية يجب أن تكون أكبر من أو تساوي 0")]
        public int Quantity { get; set; }

        [Required(ErrorMessage = "تاريخ الانتهاء مطلوب")]
        [DataType(DataType.Date)]
        public DateTime ExpiryDate { get; set; }

        [Required]
        [ForeignKey(nameof(ProductMedicine))]
        public int MedicineId { get; set; }

        public ProductMedicine? ProductMedicine { get; set; }
    }
}
