using pharmamatch.Application.DTOs;

namespace pharmamatch.Application.Interfaces
{
    /// <summary>
    /// واجهة خدمة الملف الشخصي للصيدلاني في طبقة Application.
    /// تُعرّف العمليات المطلوبة دون أي ارتباط بتفاصيل التخزين (Clean Architecture).
    /// الـ Implementation الفعلي موجود في طبقة Infrastructure.
    /// </summary>
    public interface IPharmacistProfileService
    {
        /// <summary>جلب بيانات الملف الشخصي المحفوظة</summary>
        PharmacistProfileDto GetProfile();

        /// <summary>حفظ التعديلات على الملف الشخصي بشكل دائم</summary>
        void SaveProfile(PharmacistProfileDto profile);
    }
}
