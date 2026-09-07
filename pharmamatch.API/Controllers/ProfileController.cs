using Microsoft.AspNetCore.Mvc;
using pharmamatch.Application.DTOs;
using pharmamatch.Application.Interfaces;

namespace pharmamatch.API.Controllers
{
    /// <summary>
    /// متحكم الملف الشخصي للصيدلاني (ProfileController) في الـ API.
    /// ملتزم بقواعد Clean Architecture:
    /// - يتعامل حصرياً مع واجهات طبقة Application (IPharmacistProfileService, IInventoryBatchRepository)
    /// - يوفر نقاط اتصال GET و PUT لتطبيق الفلاتر والويب للتزامن اللحظي لبيانات الصيدلي.
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class ProfileController : ControllerBase
    {
        private readonly IPharmacistProfileService _profileService;
        private readonly IInventoryBatchRepository _batchRepository;

        public ProfileController(
            IPharmacistProfileService profileService,
            IInventoryBatchRepository batchRepository)
        {
            _profileService = profileService;
            _batchRepository = batchRepository;
        }

        // GET: api/profile
        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var profile = _profileService.GetProfile();
            int expiredCount = await _batchRepository.GetExpiredCountAsync();

            return Ok(new
            {
                id = 1,
                name = profile.DoctorName,
                doctorName = profile.DoctorName,
                role = profile.Role,
                pharmacyName = profile.PharmacyName,
                pharmacy_name = profile.PharmacyName,
                licenseNumber = profile.LicenseNumber,
                license_number = profile.LicenseNumber,
                phone = profile.PhoneNumber,
                phoneNumber = profile.PhoneNumber,
                address = profile.Address,
                expiredDrugsCount = expiredCount,
                expired_drugs_count = expiredCount,
                totalAlternatives = 420,
                total_alternatives = 420
            });
        }

        // PUT: api/profile
        [HttpPut]
        [HttpPost]
        public IActionResult UpdateProfile([FromBody] PharmacistProfileDto updatedProfile)
        {
            if (updatedProfile == null)
            {
                return BadRequest(new { message = "بيانات الملف الشخصي غير صالحة" });
            }

            var current = _profileService.GetProfile();

            var toSave = new PharmacistProfileDto
            {
                DoctorName = !string.IsNullOrWhiteSpace(updatedProfile.DoctorName) ? updatedProfile.DoctorName.Trim() : current.DoctorName,
                Role = !string.IsNullOrWhiteSpace(updatedProfile.Role) ? updatedProfile.Role.Trim() : current.Role,
                PharmacyName = !string.IsNullOrWhiteSpace(updatedProfile.PharmacyName) ? updatedProfile.PharmacyName.Trim() : current.PharmacyName,
                LicenseNumber = !string.IsNullOrWhiteSpace(updatedProfile.LicenseNumber) ? updatedProfile.LicenseNumber.Trim() : current.LicenseNumber,
                PhoneNumber = !string.IsNullOrWhiteSpace(updatedProfile.PhoneNumber) ? updatedProfile.PhoneNumber.Trim() : current.PhoneNumber,
                Address = !string.IsNullOrWhiteSpace(updatedProfile.Address) ? updatedProfile.Address.Trim() : current.Address,
            };

            _profileService.SaveProfile(toSave);
            return Ok(toSave);
        }
    }
}
