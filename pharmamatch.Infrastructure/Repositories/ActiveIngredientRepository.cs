using Microsoft.EntityFrameworkCore;
using pharmamatch.Application.Interfaces;
using pharmamatch.Domain.Entities;

namespace pharmamatch.Infrastructure.Repositories
{
    public class ActiveIngredientRepository : IActiveIngredientRepository
    {
        private readonly AppDbContext _context;

        public ActiveIngredientRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<ActiveIngredient>> GetAllAsync()
        {
            return await _context.ActiveIngredients
                .Include(a => a.Category)
                .Include(a => a.Medicines)
                .ToListAsync();
        }

        public async Task<ActiveIngredient?> GetByIdAsync(int id)
        {
            return await _context.ActiveIngredients
                .Include(a => a.Category)
                .Include(a => a.Medicines)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<ActiveIngredient> CreateAsync(ActiveIngredient ingredient)
        {
            _context.ActiveIngredients.Add(ingredient);
            await _context.SaveChangesAsync();
            return ingredient;
        }

        public async Task<ActiveIngredient> UpdateAsync(ActiveIngredient ingredient)
        {
            _context.Entry(ingredient).State = EntityState.Modified;
            await _context.SaveChangesAsync();
            return ingredient;
        }

        public async Task DeleteAsync(int id)
        {
            var ingredient = await _context.ActiveIngredients.FindAsync(id);
            if (ingredient != null)
            {
                _context.ActiveIngredients.Remove(ingredient);
                await _context.SaveChangesAsync();
            }
        }

        public bool Exists(int id)
        {
            return _context.ActiveIngredients.Any(e => e.Id == id);
        }
    }
}
