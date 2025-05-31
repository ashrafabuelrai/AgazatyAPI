﻿




using static Agazaty.Shared.Utility.SD;

namespace Agazaty.Application.Common.DTOs.NormalLeaveDTOs
{
    public class NormalLeaveDTO
    {
        public int ID { get; set; }
        public string UserName { get; set; }
        public string CoworkerName { get; set; }
        public string DirectManagerName { get; set; }
        public string GeneralManagerName { get; set; }
        public string Coworker_ID { get; set; }
        public string UserID { get; set; }
        public DateTime RequestDate { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int Days { get; set; }
        public int Year { get; set; }
        public string? NotesFromEmployee { get; set; }
        public bool Accepted { get; set; } = false;
        public bool ResponseDone { get; set; } = false;
        public bool DirectManager_Decision { get; set; } = false;
        public bool GeneralManager_Decision { get; set; } = false;
        public bool CoWorker_Decision { get; set; } = false;
        public string? DisapproveReasonOfGeneral_Manager { get; set; }
        public string? DisapproveReasonOfDirect_Manager { get; set; }   
        public string PhoneNumber { get; set; }
        public string DepartmentName { get; set; }
        public LeaveStatus LeaveStatus { get; set; }
        public Holder Holder { get; set; }
        public RejectedBy RejectedBy { get; set; }
    }
}
