using System.ComponentModel.DataAnnotations;

namespace Agazaty.Data.DTOs.AccountDTOs
{
    public class ChangePasswordDTO
    {
        [Required]
        public string UseId {  get; set; }
        [Required]
        public string CurrentPassword { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8, ErrorMessage = ".يجب أن تكون كلمة المرور على الأقل 8 أحرف")]
        public string NewPassword { get; set; }

        [Required]
        [Compare("NewPassword", ErrorMessage = ".كلمة المرور غير متطابقة")]
        public string ConfirmNewPassword { get; set; }
    }
}