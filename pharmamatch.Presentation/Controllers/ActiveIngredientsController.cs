using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pharmamatch.Data;
using pharmamatch.Domain.Entities;

namespace pharmamatch.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActiveIngredientsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ActiveIngredientsController(AppDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/ActiveIngredients (GetAll)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActiveIngredient>>> GetActiveIngredients()
        {
            var ingredients = await _context.ActiveIngredients
                .Include(a => a.Category)
                .Include(a => a.Medicines)
                .ToListAsync();
            return Ok(ingredients);
        }

        // 2. GET: api/ActiveIngredients/5 (GetById)
        [HttpGet("{id:int}", Name = "GetActiveIngredientById")]
        public async Task<ActionResult<ActiveIngredient>> GetActiveIngredient(int id)
        {
            var ingredient = await _context.ActiveIngredients
                .Include(a => a.Category)
                .Include(a => a.Medicines)
                .FirstOrDefaultAsync(a => a.Id == id);

            if (ingredient == null)
            {
                return NotFound(new { message = $"المادة الفعالة ذات الرقم {id} غير موجودة" });
            }

            return Ok(ingredient);
        }

        // 3. POST: api/ActiveIngredients (Create)
        [HttpPost]
        public async Task<ActionResult<ActiveIngredient>> CreateActiveIngredient([FromBody] ActiveIngredient ingredient)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.ActiveIngredients.Add(ingredient);
            await _context.SaveChangesAsync();

            return CreatedAtRoute("GetActiveIngredientById", new { id = ingredient.Id }, ingredient);
        }

        // 4. PUT: api/ActiveIngredients/5 (Update)
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateActiveIngredient(int id, [FromBody] ActiveIngredient ingredient)
        {
            if (id != ingredient.Id)
            {
                return BadRequest(new { message = "معرف المادة الفعالة غير متطابق" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(ingredient).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ActiveIngredientExists(id))
                {
                    return NotFound(new { message = $"المادة الفعالة ذات الرقم {id} غير موجودة" });
                }
                throw;
            }

            return Ok(ingredient);
        }

        // 5. DELETE: api/ActiveIngredients/5 (Delete)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteActiveIngredient(int id)
        {
            var ingredient = await _context.ActiveIngredients.FindAsync(id);
            if (ingredient == null)
            {
                return NotFound(new { message = $"المادة الفعالة ذات الرقم {id} غير موجودة" });
            }

            _context.ActiveIngredients.Remove(ingredient);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم حذف المادة الفعالة بنجاح", id });
        }

        private bool ActiveIngredientExists(int id)
        {
            return _context.ActiveIngredients.Any(e => e.Id == id);
        }
    }
}
