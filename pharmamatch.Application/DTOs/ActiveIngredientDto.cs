namespace pharmamatch.Application.DTOs
{
    public class ActiveIngredientDto
    {
        public int Id { get; set; }
        public string ScientificName { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
    }
}
