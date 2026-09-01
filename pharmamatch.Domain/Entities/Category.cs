using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace pharmamatch.Domain.Entities
{
    public class Category
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required(ErrorMessage = "اسم الفئة مطلوب")]
        [StringLength(100, ErrorMessage = "اسم الفئة يجب ألا يتجاوز 100 حرف")]
        public string CategoryName { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "الوصف يجب ألا يتجاوز 500 حرف")]
        public string Description { get; set; } = string.Empty;

        // علاقة: الفئة تحتوي على عدة مواد فعالة
        public ICollection<ActiveIngredient> ActiveIngredients { get; set; } = new List<ActiveIngredient>();
    }
}
