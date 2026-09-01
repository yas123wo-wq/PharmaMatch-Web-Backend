using pharmamatch.Domain.Entities;

namespace pharmamatch.Application.Interfaces
{
    public interface IActiveIngredientRepository
    {
        Task<IEnumerable<ActiveIngredient>> GetAllAsync();
        Task<ActiveIngredient?> GetByIdAsync(int id);
        Task<ActiveIngredient> CreateAsync(ActiveIngredient ingredient);
        Task<ActiveIngredient> UpdateAsync(ActiveIngredient ingredient);
        Task DeleteAsync(int id);
        bool Exists(int id);
    }
}
