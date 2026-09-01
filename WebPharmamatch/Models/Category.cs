namespace WebPharmamatch.Models
{
    public class Category
    {
        public int Id { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public List<ActiveIngredient> ActiveIngredients { get; set; } = new();
    }
}
