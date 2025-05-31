namespace Agazaty.Application.Common.DTOs.SickLeaveDTOs
{
    public class CreateSickLeaveDTO
    {
        public string Disease { get; set; }
        public string? Street { get; set; }
        public string? governorate { get; set; }
        public string? State { get; set; }
        public string UserID { get; set; }
    }
}
