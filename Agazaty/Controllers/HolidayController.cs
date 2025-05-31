﻿
using Agazaty.Application.Common.DTOs.HolidayDTOs;

using Agazaty.Application.Services.Interfaces;
using Agazaty.Domain.Entities;
using Agazaty.Domain.Repositories;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Data;

namespace Agazaty.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class HolidayController : ControllerBase
    {
        private readonly IEntityBaseRepository<Holiday> _base;
        private readonly IEntityBaseRepository<NormalLeave> _normalLeavebase;
        private readonly IAccountService _accountService;
        private readonly IMapper _mapper;
        public HolidayController(IMapper mapper, IEntityBaseRepository<Holiday> Ebase, IAccountService accountService, IEntityBaseRepository<NormalLeave> normalLeavebase)
        {
            _mapper = mapper;
            _base = Ebase;
            _accountService = accountService;
            _normalLeavebase = normalLeavebase;
        }
        //[Authorize(Roles = "مدير الموارد البشرية")]
        [HttpGet("GetAllHolidays")]
        public async Task<IActionResult> GetAllHolidays()
        {
            try
            {
                var holidays = await _base.GetAll();
                if (!holidays.Any())
                {
                    return NotFound(new { Message = "No holidays found." });
                }
                return Ok(holidays);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        //[Authorize(Roles = "مدير الموارد البشرية")]
        [HttpGet("GetHolidayById/{holidayID:int}")]
        public async Task<IActionResult> GetHolidayById(int holidayID)
        {
            if (holidayID <= 0)
            {
                return BadRequest(new { Message = "Invalid department Id" });
            }
            try
            {
                var holiday = await _base.Get(h => h.Id == holidayID);
                if (holiday == null)
                {
                    return NotFound(new { Message = "No department found." });
                }
                return Ok(holiday);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        //[Authorize(Roles = "مدير الموارد البشرية")]
        [HttpPost("CreateHoliday")]
        public async Task<IActionResult> CreateHoliday([FromBody] CreateHolidayDTO model)
        {
            try
            {
                if (model == null)
                {
                    return BadRequest(new { Message = "Invalid holiday data." });
                }
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var sameholiday = await _base.Get(h => h.Date.Date == model.Date.Date);
                if (sameholiday != null)
                {
                    return BadRequest(new { Message = "There is already holiday with this date." });
                }

                var holiday = _mapper.Map<Holiday>(model);
                await _base.Add(holiday);
                //اخر تلت شهور
                //var AllNormalLeaves = await _normalLeavebase.GetAll(l => l.StartDate.Year == holiday.Date.Year);
                var threeMonthsAgo = holiday.Date.AddMonths(-3);
                var AllNormalLeaves = await _normalLeavebase.GetAll(l => l.StartDate >= threeMonthsAgo);
                foreach (var leave in AllNormalLeaves)
                {
                    if (leave.StartDate.Date <= model.Date.Date && leave.EndDate.Date >= model.Date.Date)
                    {
                        var user = await _accountService.FindById(leave.UserID);
                        user.NormalLeavesCount++;
                        leave.Days--;
                        await _normalLeavebase.Update(leave);
                        await _accountService.Update(user);
                    }
                }

                return CreatedAtAction(nameof(GetHolidayById), new { holidayID = holiday.Id }, holiday);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        //[Authorize(Roles = "مدير الموارد البشرية")]
        [HttpPut("UpdateHoliday/{holidayID:int}")]
        public async Task<IActionResult> UpdateHoliday([FromRoute] int holidayID, [FromBody] UpdateHolidayDTO model)
        {
            if (holidayID <= 0)
            {           
                return BadRequest(new { Message = "Invalid holiday data." });
            }
            var OldHoliday = await _base.Get(h => h.Id == holidayID);
            if (OldHoliday.Date.Date == model.Date.Date)
            {
                _mapper.Map(model, OldHoliday);
                await _base.Update(OldHoliday);
                return Ok(new { Message = $"Holiday has been successfully updated.", Holiday = OldHoliday });
            }
            var sameholiday = await _base.Get(h => h.Date.Date == model.Date.Date);
            if (sameholiday != null)
            {
                return BadRequest(new { Message = "There is already holiday with this date." });
            }

            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }
                var holiday = await _base.Get(d => d.Id == holidayID);
                if (holiday == null)
                {
                    return NotFound(new { Message = "Holiday is not found." });
                }
                else // old date wrong
                {
                    //اخر تلت شهور
                    //var AllNormalLeaves1 = await _normalLeavebase.GetAll(l => l.StartDate.Year == holiday.Date.Year);
                    var threeMonthsAgo2 = holiday.Date.AddMonths(-3);
                    var AllNormalLeaves = await _normalLeavebase.GetAll(l => l.StartDate >= threeMonthsAgo2);
                    foreach (var leave in AllNormalLeaves)
                    {
                        if (leave.StartDate.Date <= holiday.Date.Date && leave.EndDate.Date >= holiday.Date.Date)
                        {
                            var user = await _accountService.FindById(leave.UserID);
                            user.NormalLeavesCount--;
                            leave.Days++;
                            await _normalLeavebase.Update(leave);
                            await _accountService.Update(user);
                        }
                    }
                }
                _mapper.Map(model, holiday);
                await _base.Update(holiday);

                //أخر تلت شهور
                //var AllNormalLeaves = await _normalLeavebase.GetAll(l => l.StartDate.Year == holiday.Date.Year);  // new date
                var threeMonthsAgo3 = model.Date.AddMonths(-3);
                var AllNormalLeaves1 = await _normalLeavebase.GetAll(l => l.StartDate >= threeMonthsAgo3);
                foreach (var leave in AllNormalLeaves1)
                {
                    if (leave.StartDate.Date <= model.Date.Date && leave.EndDate.Date >= model.Date.Date)
                    {
                        var user = await _accountService.FindById(leave.UserID);
                        user.NormalLeavesCount++;
                        leave.Days--;
                        await _normalLeavebase.Update(leave);
                        await _accountService.Update(user);
                    }
                }

                return Ok(new { Message = $"Holiday has been successfully updated.", Holiday = holiday });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
        //[Authorize(Roles = "مدير الموارد البشرية")]
        [HttpDelete("DeleteHoliday/{holidayID:int}")]
        public async Task<IActionResult> DeleteHoliday(int holidayID)
        {
            if (holidayID <= 0)
            {
                return BadRequest(new { Message = "Invalid holiday Id." });
            }

            try
            {
                var holiday = await _base.Get(d => d.Id == holidayID);

                if (holiday == null)
                {
                    return NotFound(new { Message = "No holidays found." });
                }
                await _base.Remove(holiday);

                //var AllNormalLeaves = await _normalLeavebase.GetAll(l => l.StartDate.Year == holiday.Date.Year);
                var threeMonthsAgo3 = holiday.Date.AddMonths(-3);
                var AllNormalLeaves = await _normalLeavebase.GetAll(l => l.StartDate >= threeMonthsAgo3);
                foreach (var leave in AllNormalLeaves)
                {
                    if (leave.StartDate.Date <= holiday.Date.Date && leave.EndDate.Date >= holiday.Date.Date)
                    {
                        var user = await _accountService.FindById(leave.UserID);
                        user.NormalLeavesCount--;
                    }
                }
                return Ok(new { Message = $"Holiday has been successfully deleted."});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "An error occurred while processing your request.", error = ex.Message });
            }
        }
    }
}