using System.ComponentModel.DataAnnotations;

namespace Agazaty.Shared.Contracts.Email.DTOs
{
    public class SendOTPDTO
    {
        [Required, EmailAddress]
        public string Email { get; set; }

        //[Required]
        //public string UserName { get; set; }
    }
}
