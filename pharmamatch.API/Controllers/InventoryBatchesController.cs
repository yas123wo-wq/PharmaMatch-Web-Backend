using MediatR;
using Microsoft.AspNetCore.Mvc;
using pharmamatch.Application.DTOs;
using pharmamatch.Application.Features.InventoryBatches.Commands;
using pharmamatch.Application.Features.InventoryBatches.Queries;

namespace pharmamatch.API.Controllers
{
    /// <summary>
    /// متحكم دفعات المخزون (InventoryBatchesController) في طبقة الـ API.
    /// ملتزم تماماً بمعمارية Clean Architecture ونمط CQRS:
    /// - عزل كامل عن قواعد البيانات والـ Entities.
    /// - استدعاء الـ Handlers حكراً باستخدام MediatR والـ DTOs.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class InventoryBatchesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public InventoryBatchesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // 1. GET: api/InventoryBatches (GetAll)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<InventoryBatchDto>>> GetInventoryBatches()
        {
            var batches = await _mediator.Send(new GetAllInventoryBatchesQuery());
            return Ok(batches);
        }

        // 2. GET: api/InventoryBatches/5 (GetById)
        [HttpGet("{id:int}", Name = "GetInventoryBatchById")]
        public async Task<ActionResult<InventoryBatchDto>> GetInventoryBatch(int id)
        {
            var batch = await _mediator.Send(new GetInventoryBatchByIdQuery(id));
            if (batch == null)
            {
                return NotFound(new { message = $"دفعة المخزون ذات الرقم {id} غير موجودة" });
            }

            return Ok(batch);
        }

        // 3. POST: api/InventoryBatches (Create)
        [HttpPost]
        public async Task<ActionResult<InventoryBatchDto>> CreateInventoryBatch([FromBody] InventoryBatchDto batchDto)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var createdBatch = await _mediator.Send(new CreateInventoryBatchCommand(batchDto));
            return CreatedAtRoute("GetInventoryBatchById", new { id = createdBatch.Id }, createdBatch);
        }

        // 4. PUT: api/InventoryBatches/5 (Update)
        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateInventoryBatch(int id, [FromBody] InventoryBatchDto batchDto)
        {
            if (id != batchDto.Id)
            {
                return BadRequest(new { message = "معرف دفعة المخزون غير متطابق" });
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var updatedBatch = await _mediator.Send(new UpdateInventoryBatchCommand(id, batchDto));
            return Ok(updatedBatch);
        }

        // 5. DELETE: api/InventoryBatches/5 (Delete)
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteInventoryBatch(int id)
        {
            var result = await _mediator.Send(new DeleteInventoryBatchCommand(id));
            if (!result)
            {
                return NotFound(new { message = $"دفعة المخزون ذات الرقم {id} غير موجودة" });
            }

            return Ok(new { message = "تم حذف دفعة المخزون بنجاح", id });
        }
    }
}
