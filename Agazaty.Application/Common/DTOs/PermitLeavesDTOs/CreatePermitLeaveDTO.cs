
using System.ComponentModel.DataAnnotations;

namespace Agazaty.Application.Common.DTOs.PermitLeavesDTOs
{
    public class CreatePermitLeaveDTO
    {
        [Required]
        public double Hours { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public string UserId { get; set; }
    }
}
