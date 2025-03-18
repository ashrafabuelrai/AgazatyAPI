using Agazaty.Data.Services.Interfaces;
using Agazaty.Models;
using Microsoft.EntityFrameworkCore;

namespace Agazaty.Data.Services
{
    public class LeaveValidationService: ILeaveValidationService
    {
        private readonly AppDbContext _context;

        public LeaveValidationService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<bool> IsLeaveOverlapping(string userId, DateTime startDate, DateTime endDate, string leaveType)
        {
            return (leaveType != "SickLeave" && await _context.SickLeaves.AnyAsync(l =>
                       l.UserID == userId &&
                       ((l.StartDate <= endDate && l.EndDate >= startDate) || (l.EndDate >= startDate && l.StartDate <= startDate))))
            //l.StartDate <= endDate &&
            //l.EndDate >= startDate))
            ||
                (leaveType != "NormalLeave" && await _context.NormalLeaves.AnyAsync(l =>
                       l.ResponseDone==true &&
                       l.UserID == userId &&
                       ((l.StartDate <= endDate && l.EndDate >= startDate) || (l.EndDate >= startDate && l.StartDate <= startDate))) )
            ||
                (leaveType != "CasualLeave" && await _context.CasualLeaves.AnyAsync(l =>
                       l.UserId == userId && 
                       ((l.StartDate <= endDate && l.EndDate >= startDate) || (l.EndDate >= startDate && l.StartDate <= startDate))));
        }
        public async Task<bool> IsSameLeaveOverlapping(string userId, DateTime startDate, DateTime endDate, string leaveType)
        {
            if(leaveType == "SickLeave")
            {
                return (leaveType == "SickLeave" && await _context.SickLeaves.AnyAsync(l =>
                      l.UserID == userId &&
                      ((l.StartDate <= endDate && l.EndDate >= startDate) || (l.EndDate >= startDate && l.StartDate <= startDate))));
            }
            if(leaveType == "NormalLeave")
            {
                return (leaveType == "NormalLeave" && await _context.NormalLeaves.AnyAsync(l =>
                       l.ResponseDone == true &&
                       l.UserID == userId &&
                       ((l.StartDate <= endDate && l.EndDate >= startDate) || (l.EndDate >= startDate && l.StartDate <= startDate))));
            }
            if(leaveType == "CasualLeave")
            {
                return (leaveType != "CasualLeave" && await _context.CasualLeaves.AnyAsync(l =>
                       l.UserId == userId &&
                       ((l.StartDate <= endDate && l.EndDate >= startDate) || (l.EndDate >= startDate && l.StartDate <= startDate))));
            }

            return false;
                
        }
    }
}
