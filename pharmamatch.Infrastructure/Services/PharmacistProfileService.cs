using System.Text.Json;
using Microsoft.Extensions.Configuration;
using pharmamatch.Application.DTOs;
using pharmamatch.Application.Interfaces;

namespace pharmamatch.Infrastructure.Services
{
    /// <summary>
    /// تنفيذ خدمة الملف الشخصي للصيدلاني في طبقة Infrastructure.
    /// يحفظ البيانات في ملف JSON دائم بجانب ملف قاعدة البيانات،
    /// بحيث تبقى محفوظة بعد إعادة تشغيل التطبيق.
    ///
    /// ✅ Clean Architecture:
    ///   - طبقة Application تعرف فقط Interface (IPharmacistProfileService)
    ///   - هذا الكلاس في Infrastructure هو الوحيد الذي يعرف تفاصيل التخزين
    /// </summary>
    public class PharmacistProfileService : IPharmacistProfileService
    {
        private readonly string _profileFilePath;

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions
        {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        };

        public PharmacistProfileService(IConfiguration configuration)
        {
            // حفظ الملف في مجلد مشترك للنظام (AppData) لتتشاركه كل من Webpharmamatch11 و pharmamatch.API
            var localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            var pharmaDir = Path.Combine(localAppData, "PharmaMatch");
            if (!Directory.Exists(pharmaDir))
            {
                Directory.CreateDirectory(pharmaDir);
            }
            _profileFilePath = Path.Combine(pharmaDir, "pharmacist_profile.json");

            // في حال لم يكن موجوداً في AppData وكان موجوداً في مجلد التشغيل الحالي، نقوم بنسخه
            if (!File.Exists(_profileFilePath))
            {
                var localBinFile = Path.Combine(AppContext.BaseDirectory, "pharmacist_profile.json");
                if (File.Exists(localBinFile))
                {
                    try { File.Copy(localBinFile, _profileFilePath, true); } catch { }
                }
            }
        }

        public PharmacistProfileDto GetProfile()
        {
            if (!File.Exists(_profileFilePath))
                return new PharmacistProfileDto(); // القيم الافتراضية

            try
            {
                var json = File.ReadAllText(_profileFilePath);
                return JsonSerializer.Deserialize<PharmacistProfileDto>(json, _jsonOptions)
                       ?? new PharmacistProfileDto();
            }
            catch
            {
                return new PharmacistProfileDto();
            }
        }

        public void SaveProfile(PharmacistProfileDto profile)
        {
            try
            {
                var json = JsonSerializer.Serialize(profile, _jsonOptions);
                File.WriteAllText(_profileFilePath, json);
            }
            catch
            {
                // في حال فشل الحفظ نتجاهل الخطأ (لا نكسر التطبيق)
            }
        }
    }
}
