// =========================================================================
// ⚙️ Composition Root - نقطة التجميع الوحيدة (Clean Architecture)
// =========================================================================
// هذا الملف هو المكان الوحيد في طبقة Presentation المسموح له بمعرفة
// طبقة Infrastructure، وذلك حصرياً لتسجيل التبعيات في DI Container.
//
// ✅ الترتيب الصحيح للطبقات:
//    Domain ← Application ← Infrastructure
//                         ← Presentation (هنا فقط - Composition Root)
//
// جميع Controllers تتعامل فقط مع IMediator من طبقة Application.
// =========================================================================
using pharmamatch.Application;
using pharmamatch.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddHttpClient();

// 1. تسجيل خدمات طبقة الـ Application وطبقة الـ Infrastructure وفق قواعد Clean Architecture
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseRouting();

app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

// =========================================================================
// التعبئة والتوليد التلقائي لقاعدة البيانات عبر طبقة الـ Infrastructure عند بدء التشغيل
// =========================================================================
try
{
    pharmamatch.Infrastructure.DependencyInjection.SeedDatabase(app.Services);
}
catch (Exception ex)
{
    Console.WriteLine($"خطأ أثناء تهيئة قاعدة البيانات: {ex.Message}");
}

app.Run();
