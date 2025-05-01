using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agazaty.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public bool DepartmentType { get; set; }
        public DateTime CreateDate { get; set; }
        public IEnumerable<ApplicationUser> Members { get; set; }
        public string ManagerId { get; set; }
    }
}
