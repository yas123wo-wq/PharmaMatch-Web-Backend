using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pharmamatch.Data;
using pharmamatch.Domain.Entities;

namespace pharmamatch.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryBatchesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public InventoryBatchesController(AppDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/InventoryBatches (GetAll)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryBatch>>> GetInventoryBatches()
        {
            var batches = await _context.InventoryBatches
                .Include(b => b.ProductMedicine)
                .ToListAsync();
            return Ok(batches);
        }

        // 2. GET: api/InventoryBatches/5 (GetById)
        [HttpGet("{id:int}", Name = "GetInventoryBatchById")]
        public async Task<ActionResult<InventoryBatch>> GetInventoryBatch(int id)
        {
            var batch = await _context.InventoryBatches
                .Include(b => b.ProductMedicine)
                .FirstOrDefaultAsync(b => b.Id == id);

            if (batch == null)
            {
                return NotFound(new { message = $"دفعة المخزون ذات الرقم {id} غير موجودة" });
            }

            return Ok(batch);
        }

        // 3. POST: api/InventoryBatches (Create)
        [HttpPost]
        public async Task<ActionResult<InventoryBatch>> CreateInventoryBatch([FromBody] InventoryBatch batch)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.InventoryBatches.Add(batch);
            await _context.SaveChangesAsync();

            return CreatedAtRoute("GetInventoryBatchById", new { id = batch.Id }, batch);
        }

        // 4. PUT: api/InventoryBatches/5 (Update)
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateInventoryBatch(int id, [FromBody] InventoryBatch batch)
        {
            if (id != batch.Id)
            {
                return BadRequest(new { message = "معرف دفعة المخزون غير متطابق" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(batch).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!InventoryBatchExists(id))
                {
                    return NotFound(new { message = $"دفعة المخزون ذات الرقم {id} غير موجودة" });
                }
                throw;
            }

            return Ok(batch);
        }

        // 5. DELETE: api/InventoryBatches/5 (Delete)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteInventoryBatch(int id)
        {
            var batch = await _context.InventoryBatches.FindAsync(id);
            if (batch == null)
            {
                return NotFound(new { message = $"دفعة المخزون ذات الرقم {id} غير موجودة" });
            }

            _context.InventoryBatches.Remove(batch);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم حذف دفعة المخزون بنجاح", id });
        }

        private bool InventoryBatchExists(int id)
        {
            return _context.InventoryBatches.Any(e => e.Id == id);
        }
    }
}
