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
using pharmamatch.Application.Interfaces;
using Webpharmamatch11.Models;

namespace Webpharmamatch11.Controllers
{
    // =========================================================================
    // 🏥 المتحكم الرئيسي للتطبيق (HomeController)
    // يدير الشاشات الرئيسية: لوحة التحكم، البحث الذكي، قائمة المتابعة، والملف الشخصي
    // ملتزم تماماً بمعمارية Clean Architecture بنسبة 100%:
    // - عزل تام عن DbContext و EF Core و Domain Entities.
    // - يتعامل حصرياً مع طبقة التطبيق عبر MediatR ومجسمات DTOs.
    // - بيانات الملف الشخصي تُحفظ دائماً عبر IPharmacistProfileService (Infrastructure).
    // =========================================================================
    public class HomeController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IPharmacistProfileService _profileService;

        public HomeController(IMediator mediator, IPharmacistProfileService profileService)
        {
            _mediator = mediator;
            _profileService = profileService;
        }

        // =========================================================================
        // ⚡ فلتر تنفيذي تلقائي يُنفّذ قبل كل الأكشنز لمزامنة اسم الصيدلاني ورتبته في الهيدر العلوي ديناميكياً
        // =========================================================================
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var profile = _profileService.GetProfile();
            ViewBag.DoctorNameCurrent = profile.DoctorName;
            ViewBag.RoleCurrent = profile.Role;
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
        [HttpGet]
        public async Task<IActionResult> ToggleWatchlist(int id, string? returnUrl = null)
        {
            await _mediator.Send(new ToggleWatchlistCommand(id));

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer) && Uri.TryCreate(referer, UriKind.RelativeOrAbsolute, out var uri))
            {
                var localPath = uri.IsAbsoluteUri ? uri.PathAndQuery : referer;
                if (Url.IsLocalUrl(localPath))
                {
                    return Redirect(localPath);
                }
            }

            return RedirectToAction("Watchlist");
        }

        // =========================================================================
        // 3.1 إصدار وعرض التقرير المنسق للطباعة (Printable Report View)
        // يقوم بجلب البيانات المنسقة للمخزون وعرضها في واجهة طباعة احترافية
        // =========================================================================
        public async Task<IActionResult> Report(string type = "all")
        {
            var profile = _profileService.GetProfile();
            var allMedicines = (await _mediator.Send(new GetAllProductMedicinesQuery())).ToList();

            IEnumerable<ProductMedicineDto> reportData = allMedicines;
            string reportTitle = "تقرير المخزون الشامل للأدوية";

            if (type.Equals("expired", StringComparison.OrdinalIgnoreCase))
            {
                reportTitle = "تقرير الأدوية منتهية الصلاحية";
                reportData = allMedicines.Where(m => m.InventoryBatches != null && m.InventoryBatches.Any(b => b.ExpiryDate < DateTime.Now)).ToList();
            }
            else if (type.Equals("critical", StringComparison.OrdinalIgnoreCase) || type.Equals("shortage", StringComparison.OrdinalIgnoreCase))
            {
                reportTitle = "تقرير النواقص والحالات الحرجة";
                reportData = allMedicines.Where(m => m.TotalQuantity <= 50 || (m.InventoryBatches != null && m.InventoryBatches.Any(b => b.ExpiryDate < DateTime.Now))).ToList();
            }
            else if (type.Equals("watchlist", StringComparison.OrdinalIgnoreCase))
            {
                reportTitle = "تقرير الأدوية في قائمة المتابعة والمفضلة";
                reportData = allMedicines.Where(m => m.IsWatchlist).ToList();
            }

            ViewBag.ReportType = type;
            ViewBag.ReportTitle = reportTitle;
            ViewBag.ReportDate = DateTime.Now.ToString("yyyy/MM/dd HH:mm");
            ViewBag.DoctorName = profile.DoctorName;
            ViewBag.PharmacyName = profile.PharmacyName;
            ViewBag.LicenseNumber = profile.LicenseNumber;
            ViewBag.TotalItems = reportData.Count();
            ViewBag.TotalUnits = reportData.Sum(m => m.TotalQuantity);

            return View(reportData);
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

            var profile = _profileService.GetProfile();
            ViewBag.DoctorName    = profile.DoctorName;
            ViewBag.Role          = profile.Role;
            ViewBag.PharmacyName  = profile.PharmacyName;
            ViewBag.LicenseNumber = profile.LicenseNumber;
            ViewBag.PhoneNumber   = profile.PhoneNumber;
            ViewBag.Address       = profile.Address;
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
            var current = _profileService.GetProfile();

            var updated = new PharmacistProfileDto
            {
                DoctorName    = !string.IsNullOrWhiteSpace(doctorName)    ? doctorName.Trim()    : current.DoctorName,
                Role          = !string.IsNullOrWhiteSpace(role)          ? role.Trim()          : current.Role,
                PharmacyName  = !string.IsNullOrWhiteSpace(pharmacyName)  ? pharmacyName.Trim()  : current.PharmacyName,
                LicenseNumber = !string.IsNullOrWhiteSpace(licenseNumber) ? licenseNumber.Trim() : current.LicenseNumber,
                PhoneNumber   = !string.IsNullOrWhiteSpace(phoneNumber)   ? phoneNumber.Trim()   : current.PhoneNumber,
                Address       = !string.IsNullOrWhiteSpace(address)       ? address.Trim()       : current.Address,
            };

            _profileService.SaveProfile(updated);

            TempData["SuccessMessage"] = "تم تحديث بيانات الملف الشخصي بنجاح وحفظها بشكل دائم!";
            return RedirectToAction("Profile", new { showEdit = false });
        }

        // =========================================================================
        // 7. 🔄 اختيار دواء بديل (SelectAlternative Action)
        // يُعيد توجيه المستخدم إلى صفحة البحث بالدواء البديل كنتيجة رئيسية
        // يستخدم MediatR وفق Clean Architecture - لا تعامل مباشر مع قاعدة البيانات
        // =========================================================================
        public async Task<IActionResult> SelectAlternative(int id)
        {
            var medicine = await _mediator.Send(new GetProductMedicineByIdQuery(id));
            if (medicine == null)
                return RedirectToAction("Search");

            // إعادة توجيه البحث باسم الدواء البديل ليظهر كنتيجة رئيسية
            return RedirectToAction("Search", new { query = medicine.TradeName });
        }

        // =========================================================================
        // 8. 💊 إضافة دواء جديد للمخزون (Create Actions)
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
        public async Task<IActionResult> Delete(int id, string? returnUrl = null)
        {
            await _mediator.Send(new DeleteProductMedicineCommand(id));

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            var referer = Request.Headers["Referer"].ToString();
            if (!string.IsNullOrEmpty(referer) && Uri.TryCreate(referer, UriKind.RelativeOrAbsolute, out var uri))
            {
                var localPath = uri.IsAbsoluteUri ? uri.PathAndQuery : referer;
                if (Url.IsLocalUrl(localPath))
                {
                    return Redirect(localPath);
                }
            }

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
