namespace pharmamatch.Application.DTOs
{
    /// <summary>
    /// مجسّم بيانات الملف الشخصي للصيدلاني (Data Transfer Object)
    /// يُستخدم لنقل البيانات بين طبقة Application وطبقة Presentation
    /// دون أي تعامل مباشر مع قاعدة البيانات أو Infrastructure
    /// </summary>
    public class PharmacistProfileDto
    {
        public string DoctorName    { get; set; } = "د. أحمد عبد الرحمن";
        public string Role          { get; set; } = "صيدلي مرخص";
        public string PharmacyName  { get; set; } = "صيدلية الشفاء الحديثة";
        public string LicenseNumber { get; set; } = "LIC-2024-9981-AR";
        public string PhoneNumber   { get; set; } = "+966 50 123 4567";
        public string Address       { get; set; } = "حي المروج، الرياض، المملكة العربية السعودية";
    }
}
