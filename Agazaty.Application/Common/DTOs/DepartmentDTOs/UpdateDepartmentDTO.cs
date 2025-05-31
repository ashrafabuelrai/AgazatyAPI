using System.ComponentModel.DataAnnotations;

namespace Agazaty.Application.Common.DTOs.DepartmentDTOs
{
    public class UpdateDepartmentDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Code { get; set; }
        [Required]
        public DateTime CreateDate { get; set; }
        [Required]
        public string ManagerId { get; set; }
        public bool DepartmentType { get; set; }

    }
}
