﻿using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Agazaty.Domain.Entities
{
    public class PermitLeave
    {
        public int Id { get; set; }
        public double Hours { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }
        public PermitLeaveImage? PermitLeaveImage { get; set; }
        public ApplicationUser User { get; set; }
    }
}
