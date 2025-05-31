﻿using System.ComponentModel.DataAnnotations;

namespace Agazaty.Application.Common.DTOs.CasualLeaveDTOs
{
    public class CreateCasualLeaveDTO
    {
        [Required]
        public DateTime StartDate { get; set; }
        [Required]
        public DateTime EndDate { get; set; }
        [Required]
        public string UserId { get; set; }
        public string? Notes { get; set; }
    }
}
