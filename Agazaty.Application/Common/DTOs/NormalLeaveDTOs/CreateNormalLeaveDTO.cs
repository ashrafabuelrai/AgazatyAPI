namespace Agazaty.Application.Common.DTOs.NormalLeaveDTOs
{
    public class CreateNormalLeaveDTO
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string? NotesFromEmployee { get; set; }
        public string UserID { get; set; }
        public string Coworker_ID { get; set; }
    }
}
