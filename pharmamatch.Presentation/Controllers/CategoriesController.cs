using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pharmamatch.Data;
using pharmamatch.Domain.Entities;

namespace pharmamatch.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public CategoriesController(AppDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/Categories (GetAll)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Category>>> GetCategories()
        {
            var categories = await _context.Categories
                .Include(c => c.ActiveIngredients)
                .ToListAsync();
            return Ok(categories);
        }

        // 2. GET: api/Categories/5 (GetById)
        [HttpGet("{id:int}", Name = "GetCategoryById")]
        public async Task<ActionResult<Category>> GetCategory(int id)
        {
            var category = await _context.Categories
                .Include(c => c.ActiveIngredients)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return NotFound(new { message = $"الفئة ذات الرقم {id} غير موجودة" });
            }

            return Ok(category);
        }

        // 3. POST: api/Categories (Create)
        [HttpPost]
        public async Task<ActionResult<Category>> CreateCategory([FromBody] Category category)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return CreatedAtRoute("GetCategoryById", new { id = category.Id }, category);
        }

        // 4. PUT: api/Categories/5 (Update)
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] Category category)
        {
            if (id != category.Id)
            {
                return BadRequest(new { message = "معرف الفئة غير متطابق مع بيانات الطلب" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(category).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CategoryExists(id))
                {
                    return NotFound(new { message = $"الفئة ذات الرقم {id} غير موجودة" });
                }
                throw;
            }

            return Ok(category);
        }

        // 5. DELETE: api/Categories/5 (Delete)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound(new { message = $"الفئة ذات الرقم {id} غير موجودة" });
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم حذف الفئة بنجاح", id });
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.Id == id);
        }
    }
}
