using Scalar.AspNetCore;
using pharmamatch.Data;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// 1. ربط قاعدة البيانات وقراءة ConnectionString من appsettings.json
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
}

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));

// 2. تفعيل CORS للسماح بالطلبات من أي مصدر أثناء التطوير
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod());
});

// 3. إضافة الـ Controllers وتجنب المراجع الدائرية عند الـ Serialization
builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.ReferenceHandler = System.Text.Json.Serialization.ReferenceHandler.IgnoreCycles;
    });

// 4. إعداد OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

// 5. الترتيب الصحيح للـ Middleware:
app.UseHttpsRedirection();

// تفعيل CORS قبل OpenAPI و Scalar لتفادي مشاكل Network Error أثناء اختبار الـ Endpoints
app.UseCors();

app.UseAuthorization();

// 6. تشغيل OpenAPI و Scalar API Reference وتحديد مسار openapi/v1.json
app.MapOpenApi();
app.MapScalarApiReference(options =>
{
    options.WithTitle("PharmaMatch API Documentation")
           .WithTheme(ScalarTheme.Purple)
           .WithOpenApiRoutePattern("/openapi/v1.json");
});

// توجيه تلقائي من الصفحة الرئيسية (/) إلى واجهة Scalar (/scalar/v1)
app.MapGet("/", () => Results.Redirect("/scalar/v1"));

app.MapControllers();

app.Run();
