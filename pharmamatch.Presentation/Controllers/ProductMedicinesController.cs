using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using pharmamatch.Data;
using pharmamatch.Domain.Entities;

namespace pharmamatch.Presentation.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductMedicinesController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProductMedicinesController(AppDbContext context)
        {
            _context = context;
        }

        // 1. GET: api/ProductMedicines (GetAll)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductMedicine>>> GetProductMedicines()
        {
            var medicines = await _context.Medicines
                .Include(m => m.ActiveIngredient)
                .Include(m => m.InventoryBatches)
                .ToListAsync();
            return Ok(medicines);
        }

        // 2. GET: api/ProductMedicines/5 (GetById)
        [HttpGet("{id:int}", Name = "GetProductMedicineById")]
        public async Task<ActionResult<ProductMedicine>> GetProductMedicine(int id)
        {
            var medicine = await _context.Medicines
                .Include(m => m.ActiveIngredient)
                .Include(m => m.InventoryBatches)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (medicine == null)
            {
                return NotFound(new { message = $"الدواء ذات الرقم {id} غير موجود" });
            }

            return Ok(medicine);
        }

        // 3. POST: api/ProductMedicines (Create)
        [HttpPost]
        public async Task<ActionResult<ProductMedicine>> CreateProductMedicine([FromBody] ProductMedicine medicine)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Medicines.Add(medicine);
            await _context.SaveChangesAsync();

            return CreatedAtRoute("GetProductMedicineById", new { id = medicine.Id }, medicine);
        }

        // 4. PUT: api/ProductMedicines/5 (Update)
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProductMedicine(int id, [FromBody] ProductMedicine medicine)
        {
            if (id != medicine.Id)
            {
                return BadRequest(new { message = "معرف الدواء غير متطابق" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.Entry(medicine).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ProductMedicineExists(id))
                {
                    return NotFound(new { message = $"الدواء ذات الرقم {id} غير موجود" });
                }
                throw;
            }

            return Ok(medicine);
        }

        // 5. DELETE: api/ProductMedicines/5 (Delete)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProductMedicine(int id)
        {
            var medicine = await _context.Medicines.FindAsync(id);
            if (medicine == null)
            {
                return NotFound(new { message = $"الدواء ذات الرقم {id} غير موجود" });
            }

            _context.Medicines.Remove(medicine);
            await _context.SaveChangesAsync();

            return Ok(new { message = "تم حذف الدواء بنجاح", id });
        }

        private bool ProductMedicineExists(int id)
        {
            return _context.Medicines.Any(e => e.Id == id);
        }
    }
}
