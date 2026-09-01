using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Diagnostics;
using pharmamatch.Application.DTOs;
using pharmamatch.Application.Features.ActiveIngredients.Queries;
using pharmamatch.Application.Features.Categories.Queries;
using pharmamatch.Application.Features.InventoryBatches.Queries;
using pharmamatch.Application.Features.ProductMedicines.Commands;
using pharmamatch.Application.Features.ProductMedicines.Queries;
using Webpharmamatch11.Models;

namespace Webpharmamatch11.Controllers
{
    // =========================================================================
    // 🏥 المتحكم الرئيسي للتطبيق (HomeController)
    // يدير الشاشات الرئيسية: لوحة التحكم، البحث الذكي، قائمة المتابعة، والملف الشخصي
    // ملتزم تماماً بمعمارية Clean Architecture بنسبة 100%:
    // - عزل تام عن DbContext و EF Core و Domain Entities.
    // - يتعامل حصرياً مع طبقة التطبيق عبر MediatR ومجسمات DTOs.
    // =========================================================================
    public class HomeController : Controller
    {
        private readonly IMediator _mediator;

        // بيانات الملف الشخصي للصيدلاني (مخزنة في التطبيق وتتحدث فوراً وحياً في كافة صفحات الموقع)
        public static string DoctorName = "د. أحمد عبد الرحمن";
        public static string Role = "صيدلي مرخص";
        public static string PharmacyName = "صيدلية الشفاء الحديثة";
        public static string LicenseNumber = "LIC-2024-9981-AR";
        public static string PhoneNumber = "+966 50 123 4567";
        public static string Address = "حي المروج، الرياض، المملكة العربية السعودية";

        // حقن وسيط طبقة التطبيق (IMediator) حصرياً بدون أي اتصال مع قواعد البيانات
        public HomeController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // =========================================================================
        // ⚡ فلتر تنفيذي تلقائي يُنفّذ قبل كل الأكشنز لمزامنة اسم الصيدلاني ورتبته في الهيدر العلوي ديناميكياً
        // =========================================================================
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            ViewBag.DoctorNameCurrent = DoctorName;
            ViewBag.RoleCurrent = Role;
            base.OnActionExecuting(filterContext);
        }

        // =========================================================================
        // 1. الشاشة الرئيسية / لوحة التحكم (Dashboard - Index)
        // تقرأ المؤشرات الحية (KPIs) وجدول المخزون والتقرير المنتهي تلقائياً عبر MediatR CQRS
        // =========================================================================
        public async Task<IActionResult> Index()
        {
            var data = await _mediator.Send(new GetDashboardDataQuery());

            ViewBag.TotalStock = data.TotalStock;
            ViewBag.CriticalItemsCount = data.CriticalItemsCount;
            ViewBag.ExpiredBatches = data.ExpiredBatches;

            return View(data.Medicines);
        }

        // =========================================================================
        // 2. واجهة تتبع الأدوية والمفضلات (Watchlist View)
        // تعرض الأدوية الموجودة في قائمة المتابعة مع المؤشرات والإحصائيات عبر DTOs
        // =========================================================================
        public async Task<IActionResult> Watchlist(string search)
        {
            var data = await _mediator.Send(new GetWatchlistDataQuery(search));

            ViewBag.FavoriteCount = data.FavoriteCount;
            ViewBag.SevereShortageCount = data.SevereShortageCount;
            ViewBag.StableStockCount = data.StableStockCount;
            ViewBag.SearchQuery = search;

            return View(data.FilteredMedicines);
        }

        // =========================================================================
        // 3. تبديل حالة المتابعة / التفضيل (Toggle Watchlist Action)
        // =========================================================================
        [HttpPost]
        public async Task<IActionResult> ToggleWatchlist(int id, string returnUrl)
        {
            await _mediator.Send(new ToggleWatchlistCommand(id));

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Watchlist");
        }

        // =========================================================================
        // 4. 🔍 واجهة البحث المتقدم والفلترة مع ميزة البحث الذكي عن البدائل (Smart Alternatives Search)
        // =========================================================================
        public async Task<IActionResult> Search(string query, int? categoryId, string statusFilter)
        {
            var data = await _mediator.Send(new SearchMedicinesQuery(query, categoryId, statusFilter));

            ViewBag.Categories = data.Categories;
            ViewBag.SearchQuery = query;
            ViewBag.SelectedCategory = categoryId;
            ViewBag.SelectedStatus = statusFilter;
            ViewBag.Alternatives = data.Alternatives;

            return View(data.Results);
        }

        // =========================================================================
        // 5. واجهة الملف الشخصي للصيدلي (Profile View)
        // =========================================================================
        public async Task<IActionResult> Profile(bool showEdit = false)
        {
            var batches = await _mediator.Send(new GetAllInventoryBatchesQuery());
            int expiredCount = batches.Count(b => b.ExpiryDate < DateTime.Now);

            ViewBag.DoctorName = DoctorName;
            ViewBag.Role = Role;
            ViewBag.PharmacyName = PharmacyName;
            ViewBag.LicenseNumber = LicenseNumber;
            ViewBag.PhoneNumber = PhoneNumber;
            ViewBag.Address = Address;
            ViewBag.ExpiredCount = expiredCount;
            ViewBag.TotalAlternatives = 420;
            ViewBag.ShowEditForm = showEdit;

            return View();
        }

        // =========================================================================
        // 6. حفظ تعديلات الملف الشخصي للصيدلاني (EditProfile POST Action)
        // =========================================================================
        [HttpPost]
        public IActionResult EditProfile(string doctorName, string role, string pharmacyName, string licenseNumber, string phoneNumber, string address)
        {
            if (!string.IsNullOrWhiteSpace(doctorName)) DoctorName = doctorName;
            if (!string.IsNullOrWhiteSpace(role)) Role = role;
            if (!string.IsNullOrWhiteSpace(pharmacyName)) PharmacyName = pharmacyName;
            if (!string.IsNullOrWhiteSpace(licenseNumber)) LicenseNumber = licenseNumber;
            if (!string.IsNullOrWhiteSpace(phoneNumber)) PhoneNumber = phoneNumber;
            if (!string.IsNullOrWhiteSpace(address)) Address = address;

            TempData["SuccessMessage"] = "تم تحديث بيانات الملف الشخصي بنجاح وتحديث اسم الصيدلاني في الهيدر العلوي تلقائياً!";
            return RedirectToAction("Profile", new { showEdit = false });
        }

        // =========================================================================
        // 7. 💊 إضافة دواء جديد للمخزون (Create Actions)
        // =========================================================================
        public async Task<IActionResult> Create()
        {
            ViewBag.Categories = await _mediator.Send(new GetAllCategoriesQuery());
            ViewBag.Ingredients = await _mediator.Send(new GetAllActiveIngredientsQuery());
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(string tradeName, decimal price, int categoryId, int ingredientId, int initialQuantity, DateTime expiryDate, string batchNumber)
        {
            if (!string.IsNullOrWhiteSpace(tradeName) && ingredientId > 0 && expiryDate > DateTime.MinValue)
            {
                await _mediator.Send(new AddMedicineWithBatchCommand(tradeName, price, categoryId, ingredientId, initialQuantity, expiryDate, batchNumber));
                return RedirectToAction("Index");
            }

            ViewBag.Categories = await _mediator.Send(new GetAllCategoriesQuery());
            ViewBag.Ingredients = await _mediator.Send(new GetAllActiveIngredientsQuery());
            return View();
        }

        // =========================================================================
        // 8. حذف دواء من قاعدة البيانات (Delete Action)
        // =========================================================================
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            await _mediator.Send(new DeleteProductMedicineCommand(id));
            return RedirectToAction("Index");
        }

        // =========================================================================
        // 9. ✏️ جلب بيانات الدواء الشاملة لتعديلها (Edit GET Action)
        // =========================================================================
        public async Task<IActionResult> Edit(int id)
        {
            var medicine = await _mediator.Send(new GetProductMedicineByIdQuery(id));
            if (medicine == null)
            {
                return NotFound();
            }

            var latestBatch = medicine.InventoryBatches.OrderBy(b => b.ExpiryDate).FirstOrDefault();

            ViewBag.Categories = await _mediator.Send(new GetAllCategoriesQuery());
            ViewBag.Ingredients = await _mediator.Send(new GetAllActiveIngredientsQuery());
            ViewBag.SelectedCategoryId = medicine.CategoryId ?? 0;
            ViewBag.CurrentQuantity = medicine.TotalQuantity;
            ViewBag.BatchNumber = latestBatch?.BatchNumber ?? $"BN-{medicine.Id}";
            ViewBag.ExpiryDate = latestBatch != null ? latestBatch.ExpiryDate.ToString("yyyy-MM-dd") : DateTime.Now.AddYears(1).ToString("yyyy-MM-dd");

            return View(medicine);
        }

        // =========================================================================
        // 10. 💾 حفظ تعديلات الدواء الشاملة (Edit POST Action)
        // =========================================================================
        [HttpPost]
        public async Task<IActionResult> Edit(int id, string tradeName, decimal price, int categoryId, int ingredientId, int totalQuantity, string batchNumber, DateTime expiryDate)
        {
            if (!string.IsNullOrWhiteSpace(tradeName))
            {
                await _mediator.Send(new UpdateMedicineWithBatchCommand(id, tradeName, price, categoryId, ingredientId, totalQuantity, batchNumber, expiryDate));
                return RedirectToAction("Index");
            }

            var existingMed = await _mediator.Send(new GetProductMedicineByIdQuery(id));
            ViewBag.Categories = await _mediator.Send(new GetAllCategoriesQuery());
            ViewBag.Ingredients = await _mediator.Send(new GetAllActiveIngredientsQuery());
            ViewBag.SelectedCategoryId = categoryId;
            ViewBag.CurrentQuantity = totalQuantity;
            ViewBag.BatchNumber = batchNumber;
            ViewBag.ExpiryDate = expiryDate.ToString("yyyy-MM-dd");
            return View(existingMed);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
