using MediatR;
using Microsoft.AspNetCore.Mvc;
using pharmamatch.Application.DTOs;
using pharmamatch.Application.Features.Categories.Commands;
using pharmamatch.Application.Features.Categories.Queries;

namespace pharmamatch.API.Controllers
{
    /// <summary>
    /// متحكم الفئات الدوائية (CategoriesController) في طبقة الـ API.
    /// ملتزم تماماً بمعمارية Clean Architecture ونمط CQRS:
    /// - عزل كامل عن الـ Domain Entities و Database.
    /// - يحول طلبات HTTP إلى MediatR Commands و Queries مع التعامل بالـ DTOs.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class CategoriesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // 1. GET: api/Categories (GetAll)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetCategories()
        {
            var categories = await _mediator.Send(new GetAllCategoriesQuery());
            return Ok(categories);
        }

        // 2. GET: api/Categories/5 (GetById)
        [HttpGet("{id:int}", Name = "GetCategoryById")]
        public async Task<ActionResult<CategoryDto>> GetCategory(int id)
        {
            var category = await _mediator.Send(new GetCategoryByIdQuery(id));
            if (category == null)
            {
                return NotFound(new { message = $"الفئة ذات الرقم {id} غير موجودة" });
            }

            return Ok(category);
        }

        // 3. POST: api/Categories (Create)
        [HttpPost]
        public async Task<ActionResult<CategoryDto>> CreateCategory([FromBody] CategoryDto categoryDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdCategory = await _mediator.Send(new CreateCategoryCommand(categoryDto));
            return CreatedAtRoute("GetCategoryById", new { id = createdCategory.Id }, createdCategory);
        }

        // 4. PUT: api/Categories/5 (Update)
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateCategory(int id, [FromBody] CategoryDto categoryDto)
        {
            if (id != categoryDto.Id)
            {
                return BadRequest(new { message = "معرف الفئة غير متطابق مع بيانات الطلب" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedCategory = await _mediator.Send(new UpdateCategoryCommand(id, categoryDto));
            return Ok(updatedCategory);
        }

        // 5. DELETE: api/Categories/5 (Delete)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var result = await _mediator.Send(new DeleteCategoryCommand(id));
            if (!result)
            {
                return NotFound(new { message = $"الفئة ذات الرقم {id} غير موجودة" });
            }

            return Ok(new { message = "تم حذف الفئة بنجاح", id });
        }
    }
}
