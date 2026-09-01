using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;
using WebPharmamatch.Models;

namespace WebPharmamatch.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly HttpClient _httpClient;

        public CategoriesController(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // GET: Categories
        public async Task<IActionResult> Index()
        {
            try
            {
                var categories = await _httpClient.GetFromJsonAsync<List<Category>>("https://localhost:7001/api/Categories") ?? new List<Category>();
                return View(categories);
            }
            catch
            {
                return View(new List<Category>());
            }
        }

        // GET: Categories/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Categories/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (!ModelState.IsValid)
            {
                return View(category);
            }

            category.Id = 0;

            try
            {
                using var response = await _httpClient.PostAsJsonAsync("https://localhost:7001/api/Categories", category);
                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }
            catch
            {
                ModelState.AddModelError(string.Empty, "تعذر الاتصال بـ API الخدمة");
            }

            return View(category);
        }

        // GET: Categories/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                using var response = await _httpClient.GetAsync($"https://localhost:7001/api/Categories/{id}");
                if (!response.IsSuccessStatusCode)
                {
                    return NotFound();
                }

                var category = await response.Content.ReadFromJsonAsync<Category>();
                return View(category);
            }
            catch
            {
                return NotFound();
            }
        }

        // POST: Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                using var response = await _httpClient.DeleteAsync($"https://localhost:7001/api/Categories/{id}");
            }
            catch
            {
                // Fallback redirect
            }

            return RedirectToAction(nameof(Index));
        }
    }
}