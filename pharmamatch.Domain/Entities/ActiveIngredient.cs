using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pharmamatch.Domain.Entities
{
    public class ActiveIngredient
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "الاسم العلمي مطلوب")]
        [StringLength(200, ErrorMessage = "الاسم العلمي يجب ألا يتجاوز 200 حرف")]
        public string ScientificName { get; set; } = string.Empty;

        [Required]
        [ForeignKey(nameof(Category))]
        public int CategoryId { get; set; }

        public Category? Category { get; set; }

        public ICollection<ProductMedicine> Medicines { get; set; } = new List<ProductMedicine>();
    }
}
