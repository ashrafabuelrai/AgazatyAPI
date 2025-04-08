using System.ComponentModel.DataAnnotations;

namespace Agazaty.Data.Email.DTOs
{
    public class VerifyOTPDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; }
        [Required, RegularExpression(@"^\d{6}$", ErrorMessage = ".يجب أن يكون الرقم السري مكونًا من 6 أرقام")]
        public string EnteredOtp { get; set; }
    }
}
