using MediatR;
using Microsoft.AspNetCore.Mvc;
using pharmamatch.Application.DTOs;
using pharmamatch.Application.Features.ActiveIngredients.Commands;
using pharmamatch.Application.Features.ActiveIngredients.Queries;
using pharmamatch.Application.Features.Categories.Queries;
using pharmamatch.Application.Features.ProductMedicines.Commands;
using pharmamatch.Application.Features.ProductMedicines.Queries;

namespace pharmamatch.API.Controllers
{
    /// <summary>
    /// متحكم الأدوية (ProductMedicinesController) في طبقة الـ API.
    /// ملتزم تماماً بمعمارية Clean Architecture ونمط CQRS:
    /// - يستقبل طلبات HTTP ويقوم بتمريرها عبر MediatR إلى الـ Handlers باستخدام الـ DTOs.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProductMedicinesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ProductMedicinesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // 1. GET: api/ProductMedicines (GetAll or Search)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductMedicineDto>>> GetProductMedicines([FromQuery] string? q, [FromQuery] string? query)
        {
            var searchTerm = !string.IsNullOrWhiteSpace(q) ? q : query;
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchResult = await _mediator.Send(new SearchMedicinesQuery(searchTerm, null, null));
                return Ok(searchResult.Results);
            }

            var medicines = await _mediator.Send(new GetAllProductMedicinesQuery());
            return Ok(medicines);
        }

        // 2. GET: api/ProductMedicines/alternatives (Search Alternatives by query string)
        [HttpGet("alternatives")]
        public async Task<ActionResult<IEnumerable<ProductMedicineDto>>> SearchAlternatives([FromQuery] string? q, [FromQuery] string? query)
        {
            var searchTerm = !string.IsNullOrWhiteSpace(q) ? q : query;
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var searchResult = await _mediator.Send(new SearchMedicinesQuery(searchTerm, null, null));
                var combined = searchResult.Results.Concat(searchResult.Alternatives)
                    .GroupBy(m => m.Id)
                    .Select(g => g.First())
                    .ToList();
                return Ok(combined);
            }

            var medicines = await _mediator.Send(new GetAllProductMedicinesQuery());
            return Ok(medicines);
        }

        // 3. GET: api/ProductMedicines/5 (GetById)
        [HttpGet("{id:int}", Name = "GetProductMedicineById")]
        public async Task<ActionResult<ProductMedicineDto>> GetProductMedicine(int id)
        {
            var medicine = await _mediator.Send(new GetProductMedicineByIdQuery(id));
            if (medicine == null)
            {
                return NotFound(new { message = $"الدواء ذات الرقم {id} غير موجود" });
            }

            return Ok(medicine);
        }

        // 4. GET: api/ProductMedicines/5/alternatives (GetMedicineAlternativesQuery - CQRS)
        [HttpGet("{id:int}/alternatives")]
        public async Task<ActionResult<IEnumerable<ProductMedicineDto>>> GetMedicineAlternatives(int id)
        {
            var alternatives = await _mediator.Send(new GetMedicineAlternativesQuery(id));
            return Ok(alternatives);
        }

        // 5. POST: api/ProductMedicines (Create with Batch & Database Persistence)
        [HttpPost]
        public async Task<ActionResult<ProductMedicineDto>> CreateProductMedicine([FromBody] CreateMedicineApiRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "بيانات الدواء غير صالحة" });
            }

            var tradeName = (request.TradeName ?? request.Name ?? "").Trim();
            if (string.IsNullOrWhiteSpace(tradeName))
            {
                return BadRequest(new { message = "اسم الدواء مطلوب" });
            }

            // تحديد المادة الفعالة
            int ingredientId = request.IngredientId;
            var scientificName = (request.ScientificName ?? request.ActiveIngredient ?? "").Trim();

            var ingredients = await _mediator.Send(new GetAllActiveIngredientsQuery());
            if (ingredientId <= 0 && !string.IsNullOrWhiteSpace(scientificName))
            {
                var existingIng = ingredients.FirstOrDefault(i => i.ScientificName.Equals(scientificName, StringComparison.OrdinalIgnoreCase));
                if (existingIng != null)
                {
                    ingredientId = existingIng.Id;
                }
                else
                {
                    // جلب أول فئة متاحة إن لم تكن ممررة
                    int categoryId = request.CategoryId;
                    if (categoryId <= 0)
                    {
                        var categories = await _mediator.Send(new GetAllCategoriesQuery());
                        var categoryName = (request.CategoryName ?? request.Category ?? "").Trim();
                        var existingCat = categories.FirstOrDefault(c => c.CategoryName.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
                        categoryId = existingCat?.Id ?? categories.FirstOrDefault()?.Id ?? 1;
                    }

                    var newIng = await _mediator.Send(new CreateActiveIngredientCommand(new ActiveIngredientDto
                    {
                        ScientificName = scientificName,
                        CategoryId = categoryId
                    }));
                    ingredientId = newIng.Id;
                }
            }

            if (ingredientId <= 0)
            {
                ingredientId = ingredients.FirstOrDefault()?.Id ?? 1;
            }

            int categoryIdForMed = request.CategoryId > 0 ? request.CategoryId : 1;
            int quantity = request.InitialQuantity > 0 ? request.InitialQuantity : (request.Stock > 0 ? request.Stock : 10);
            DateTime expiryDate = request.ExpiryDate ?? DateTime.Now.AddYears(2);
            string batchNumber = string.IsNullOrWhiteSpace(request.BatchNumber)
                ? $"BN-{Guid.NewGuid().ToString().Substring(0, 5).ToUpper()}"
                : request.BatchNumber.Trim();

            // تنفيذ أمر إضافة الدواء مع الشحنة للحفظ المباشر في قاعدة البيانات
            await _mediator.Send(new AddMedicineWithBatchCommand(
                tradeName,
                request.Price,
                categoryIdForMed,
                ingredientId,
                quantity,
                expiryDate,
                batchNumber
            ));

            // جلب الدواء المضاف حديثاً لترجيعه في الاستجابة
            var allMeds = await _mediator.Send(new GetAllProductMedicinesQuery());
            var createdMed = allMeds.FirstOrDefault(m => m.TradeName == tradeName) ?? allMeds.LastOrDefault();

            return Ok(createdMed);
        }

        // 6. PUT: api/ProductMedicines/5 (Update with Batch & Full Details)
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateProductMedicine(int id, [FromBody] CreateMedicineApiRequest request)
        {
            if (request == null)
            {
                return BadRequest(new { message = "بيانات التعديل غير صالحة" });
            }

            var tradeName = (request.TradeName ?? request.Name ?? "").Trim();
            if (string.IsNullOrWhiteSpace(tradeName))
            {
                return BadRequest(new { message = "اسم الدواء مطلوب" });
            }

            int ingredientId = request.IngredientId;
            var scientificName = (request.ScientificName ?? request.ActiveIngredient ?? "").Trim();

            var ingredients = await _mediator.Send(new GetAllActiveIngredientsQuery());
            if (ingredientId <= 0 && !string.IsNullOrWhiteSpace(scientificName))
            {
                var existingIng = ingredients.FirstOrDefault(i => i.ScientificName.Equals(scientificName, StringComparison.OrdinalIgnoreCase));
                if (existingIng != null)
                {
                    ingredientId = existingIng.Id;
                }
                else
                {
                    int categoryId = request.CategoryId;
                    if (categoryId <= 0)
                    {
                        var categories = await _mediator.Send(new GetAllCategoriesQuery());
                        var categoryName = (request.CategoryName ?? request.Category ?? "").Trim();
                        var existingCat = categories.FirstOrDefault(c => c.CategoryName.Equals(categoryName, StringComparison.OrdinalIgnoreCase));
                        categoryId = existingCat?.Id ?? categories.FirstOrDefault()?.Id ?? 1;
                    }

                    var newIng = await _mediator.Send(new CreateActiveIngredientCommand(new ActiveIngredientDto
                    {
                        ScientificName = scientificName,
                        CategoryId = categoryId
                    }));
                    ingredientId = newIng.Id;
                }
            }

            if (ingredientId <= 0)
            {
                ingredientId = ingredients.FirstOrDefault()?.Id ?? 1;
            }

            int categoryIdForMed = request.CategoryId > 0 ? request.CategoryId : 1;
            int quantity = request.InitialQuantity > 0 ? request.InitialQuantity : (request.Stock > 0 ? request.Stock : 10);
            DateTime expiryDate = request.ExpiryDate ?? DateTime.Now.AddYears(2);
            string batchNumber = string.IsNullOrWhiteSpace(request.BatchNumber)
                ? $"BN-{id}"
                : request.BatchNumber.Trim();

            await _mediator.Send(new UpdateMedicineWithBatchCommand(
                id,
                tradeName,
                request.Price,
                categoryIdForMed,
                ingredientId,
                quantity,
                batchNumber,
                expiryDate
            ));

            var updatedMed = await _mediator.Send(new GetProductMedicineByIdQuery(id));
            return Ok(updatedMed);
        }

        // 7. DELETE: api/ProductMedicines/5 (Delete)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteProductMedicine(int id)
        {
            var result = await _mediator.Send(new DeleteProductMedicineCommand(id));
            if (!result)
            {
                return NotFound(new { message = $"الدواء ذات الرقم {id} غير موجود" });
            }

            return Ok(new { message = "تم حذف الدواء بنجاح", id });
        }
    }

    /// <summary>
    /// نموذج الطلب الهجين لاستقبال طلبات إضافة الأدوية من تطبيق الفلاتر أو الـ API
    /// </summary>
    public class CreateMedicineApiRequest
    {
        public string? TradeName { get; set; }
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int IngredientId { get; set; }
        public string? ScientificName { get; set; }
        public string? ActiveIngredient { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public string? Category { get; set; }
        public int InitialQuantity { get; set; }
        public int Stock { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public string? BatchNumber { get; set; }
    }
}
