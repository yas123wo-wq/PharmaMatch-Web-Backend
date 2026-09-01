using MediatR;
using Microsoft.AspNetCore.Mvc;
using pharmamatch.Application.DTOs;
using pharmamatch.Application.Features.Categories.Commands;
using pharmamatch.Application.Features.Categories.Queries;

namespace WebPharmamatch.Controllers
{
    // =========================================================================
    // 📁 متحكم الفئات (CategoriesController) في طبقة الـ UI
    // ملتزم تماماً بمعمارية Clean Architecture:
    // - عزل تام عن DbContext وعن Domain Entities.
    // - يتم التعامل حصرياً عبر IMediator و CategoryDto.
    // =========================================================================
    public class CategoriesController : Controller
    {
        private readonly IMediator _mediator;

        public CategoriesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            var categories = await _mediator.Send(new GetAllCategoriesQuery());
            return View(categories);
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryDto category)
        {
            ModelState.Remove("Id");

            if (!ModelState.IsValid)
            {
                return View(category);
            }

            category.Id = 0;

            try
            {
                await _mediator.Send(new CreateCategoryCommand(category));
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, $"فشل حفظ الفئة: {ex.Message}");
            }

            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _mediator.Send(new GetCategoryByIdQuery(id));
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _mediator.Send(new DeleteCategoryCommand(id));
            return RedirectToAction(nameof(Index));
        }
    }
}