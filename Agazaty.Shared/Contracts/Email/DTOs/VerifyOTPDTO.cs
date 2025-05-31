using System.ComponentModel.DataAnnotations;

namespace Agazaty.Shared.Contracts.Email.DTOs
{
    public class VerifyOTPDTO
    {
       
        [Required(ErrorMessage = "البريد الإلكتروني مطلوب.")]
        [EmailAddress(ErrorMessage = "صيغة البريد الإلكتروني غير صالحة.")]
        public string Email { get; set; }
        [Required(ErrorMessage = "الرمز السري مطلوب.")]
        [RegularExpression(@"^\d{6}$", ErrorMessage = "يجب أن يكون الرقم السري مكونًا من 6 أرقام.")]
        public string EnteredOtp { get; set; }
    }
}
