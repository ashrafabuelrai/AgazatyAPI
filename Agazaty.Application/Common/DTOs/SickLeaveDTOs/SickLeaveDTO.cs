
namespace Agazaty.Application.Common.DTOs.SickLeaveDTOs
{
    public class SickLeaveDTO
    {
        public int Id { get; set; }
        public string Disease { get; set; }
        public DateTime RequestDate { get; set; }
        public string? MedicalCommitteAddress { get; set; }
        public bool RespononseDoneForMedicalCommitte { get; set; }
        public bool ResponseDoneFinal { get; set; }
        public int Year { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? Street { get; set; }
        public string? governorate { get; set; }
        public string? State { get; set; }
        public int? Days { get; set; }
        public bool Chronic { get; set; }
        public bool Certified { get; set; }
        public string UserName { get; set; }
        public string UserID { get; set; }
    }
}
