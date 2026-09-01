namespace WebPharmamatch.Models
{
    public class ActiveIngredient
    {
        public int Id { get; set; }
        public string ScientificName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
    }
}
