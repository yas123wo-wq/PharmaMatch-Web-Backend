using Scalar.AspNetCore;
using pharmamatch.Application;
using pharmamatch.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// 1. تسجيل خدمات طبقة الـ Application وطبقة الـ Infrastructure وفق قواعد Clean Architecture
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

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
if (!app.Environment.IsDevelopment())
{
    app.UseHttpsRedirection();
}

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
