using MediatR;
using Microsoft.AspNetCore.Mvc;
using pharmamatch.Application.DTOs;
using pharmamatch.Application.Features.ActiveIngredients.Commands;
using pharmamatch.Application.Features.ActiveIngredients.Queries;

namespace pharmamatch.API.Controllers
{
    /// <summary>
    /// متحكم المواد الفعالة (ActiveIngredientsController) في طبقة الـ API.
    /// ملتزم تماماً بمعمارية Clean Architecture ونمط CQRS:
    /// - عزل كامل عن الـ Domain Entities وعن الـ Database.
    /// - يتم توجيه العمليات حصرياً عبر MediatR مع التعامل مع DTOs.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ActiveIngredientsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ActiveIngredientsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // 1. GET: api/ActiveIngredients (GetAll or Filter)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ActiveIngredientDto>>> GetActiveIngredients([FromQuery] string? q, [FromQuery] string? query)
        {
            var ingredients = await _mediator.Send(new GetAllActiveIngredientsQuery());
            var searchTerm = !string.IsNullOrWhiteSpace(q) ? q : query;
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                ingredients = ingredients.Where(i => i.ScientificName.Contains(searchTerm, StringComparison.OrdinalIgnoreCase));
            }
            return Ok(ingredients);
        }

        // 2. GET: api/ActiveIngredients/5 (GetById)
        [HttpGet("{id:int}", Name = "GetActiveIngredientById")]
        public async Task<ActionResult<ActiveIngredientDto>> GetActiveIngredient(int id)
        {
            var ingredient = await _mediator.Send(new GetActiveIngredientByIdQuery(id));
            if (ingredient == null)
            {
                return NotFound(new { message = $"المادة الفعالة ذات الرقم {id} غير موجودة" });
            }

            return Ok(ingredient);
        }

        // 3. POST: api/ActiveIngredients (Create)
        [HttpPost]
        public async Task<ActionResult<ActiveIngredientDto>> CreateActiveIngredient([FromBody] ActiveIngredientDto ingredientDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdIngredient = await _mediator.Send(new CreateActiveIngredientCommand(ingredientDto));
            return CreatedAtRoute("GetActiveIngredientById", new { id = createdIngredient.Id }, createdIngredient);
        }

        // 4. PUT: api/ActiveIngredients/5 (Update)
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateActiveIngredient(int id, [FromBody] ActiveIngredientDto ingredientDto)
        {
            if (id != ingredientDto.Id)
            {
                return BadRequest(new { message = "معرف المادة الفعالة غير متطابق" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedIngredient = await _mediator.Send(new UpdateActiveIngredientCommand(id, ingredientDto));
            return Ok(updatedIngredient);
        }

        // 5. DELETE: api/ActiveIngredients/5 (Delete)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteActiveIngredient(int id)
        {
            var result = await _mediator.Send(new DeleteActiveIngredientCommand(id));
            if (!result)
            {
                return NotFound(new { message = $"المادة الفعالة ذات الرقم {id} غير موجودة" });
            }

            return Ok(new { message = "تم حذف المادة الفعالة بنجاح", id });
        }
    }
}
