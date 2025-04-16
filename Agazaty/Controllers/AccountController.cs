using Agazaty.Data.Base;
using Agazaty.Data.DTOs.AccountDTOs;
using Agazaty.Data.Email.DTOs;
using Agazaty.Data.Enums;
using Agazaty.Data.Services.Interfaces;
using Agazaty.Models;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using OfficeOpenXml;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Security.Claims;
using System.Security.Cryptography.X509Certificates;
using System.Text.RegularExpressions;

namespace Agazaty.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;
        private readonly IRoleService _roleService;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IEntityBaseRepository<Department> _deptBase;
        private readonly IMapper _mapper;
        public AccountController(IMapper mapper, IAccountService accountService, SignInManager<ApplicationUser> signInManager, IRoleService roleService, IEntityBaseRepository<Department> deptBase, UserManager<ApplicationUser> userManager)
        {
            _accountService = accountService;
            _signInManager = signInManager;
            _roleService = roleService;
            _deptBase = deptBase;
            _mapper = mapper;
            _userManager = userManager;
        }
        
        //[Authorize]
        [HttpPut("Reset-Password")]
        public async Task<IActionResult> ResetPassword(ResetPasswordDTO DTO)
        {
            var result = await _accountService.ResetPassword(DTO);
            if (!result.IsAuthenticated) return BadRequest(result);
            return Ok(result);
        }
        //[Authorize]
        [HttpPost("Forget-Password")]
        public async Task<IActionResult> ForgetPassword(SendOTPDTO DTO)
        {
            var result = await _accountService.ForgetPassword(DTO);
            if (!result.IsAuthenticated) return BadRequest(result);
            return Ok(result);
        }
        //[Authorize]
        [HttpPost("Send-OTP")]
        public async Task<IActionResult> SendOtp(SendOTPDTO DTO)
        {
            var result = await _accountService.SendOTP(DTO.Email);
            if (!result.IsAuthenticated) return BadRequest(result);
            return Ok(result);
        }
        [HttpPost("Verify-OTP")]
        public async Task<IActionResult> VerifyOtp(VerifyOTPDTO verifyOtpDTO)
        {
            var result = await _accountService.VerifyOtpAsync(verifyOtpDTO.Email, verifyOtpDTO.EnteredOtp);
            if (!result.IsAuthenticated) return BadRequest(result);
            return Ok(result);
        }
        //[Authorize]
        [HttpPost("Change-Password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO model)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _accountService.FindById(model.UseId);
            if (user == null)
                return NotFound("لم يتم العثور على المستخدم.");

            var result = await _accountService.ChangePassword(user, model);
            if (!result.Succeeded)
            {
                var errors = result.Errors.Select(e => e.Description).ToList();
                return BadRequest(new { Errors = errors });
            }
            return Ok("تم تغيير كلمة المرور بنجاح.");
        }
        //[Authorize]
        [HttpGet("GetUserById/{userID}")]
        public async Task<IActionResult> GetUserById([FromRoute]string userID)
        {
            if (string.IsNullOrWhiteSpace(userID))
                return BadRequest(new { message = "معرف المستخدم غير صالح." });
            try
            {
                var user = await _accountService.FindById(userID);
                if (user == null)
                {
                    return NotFound(new { Message = "لم يتم العثور على المستخدم." });
                }

                var ReturnedUser = _mapper.Map<UserDTO>(user);

                ReturnedUser.FullName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                ReturnedUser.RoleName = await _accountService.GetFirstRole(user);
                if (user.Departement_ID != null)
                {
                    var dept = await _deptBase.Get(d => d.Id == user.Departement_ID);
                    ReturnedUser.DepartmentName = dept.Name;
                }
                var DepartmentManager = await _deptBase.Get(d => d.ManagerId == ReturnedUser.Id);
                if (DepartmentManager != null) ReturnedUser.IsDirectManager = true;
                else ReturnedUser.IsDirectManager = false;
                return Ok(ReturnedUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء معالجة طلبك.", error = ex.Message });
            }
        }
        //[Authorize]
        [HttpGet("GetRoleOfUser/{userID}")]
        public async Task<IActionResult> GetRoleOfUser([FromRoute] string userID)
        {
            if (string.IsNullOrWhiteSpace(userID))
                return BadRequest(new { message = "معرف المستخدم غير صالح." });
            try
            {
                var user = await _accountService.FindById(userID);
                if (user == null)
                {
                    return NotFound(new { Message = "لم يتم العثور على المستخدم." });
                }
                var role =await _accountService.GetFirstRole(user);
                return Ok(new{role});
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء معالجة طلبك.", error = ex.Message });
            }
        }
        //[Authorize(Roles = "عميد الكلية,أمين الكلية,مدير الموارد البشرية")]
        [HttpGet("GetUserByNationalId/{NationalId}")]
        public async Task<IActionResult> GetUserByNationalId(string NationalId)
        {
            if (string.IsNullOrWhiteSpace(NationalId))
                return BadRequest(new { message = "معرف المستخدم غير صالح." });
            try
            {
                var user = await _accountService.FindByNationalId(NationalId);
                if (user == null) return NotFound(new { Message = "لم يتم العثور على المستخدم." });

                var ReturnedUser = _mapper.Map<UserDTO>(user);
                ReturnedUser.FullName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                ReturnedUser.RoleName = await _accountService.GetFirstRole(user);
                if (user.Departement_ID != null)
                {
                    var dpt = await _deptBase.Get(d => d.Id == user.Departement_ID);
                    ReturnedUser.DepartmentName = dpt.Name;
                }
                var DepartmentManager = await _deptBase.Get(d => d.ManagerId == ReturnedUser.Id);
                if (DepartmentManager != null) ReturnedUser.IsDirectManager = true;
                else ReturnedUser.IsDirectManager = false;
                return Ok(ReturnedUser);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء معالجة طلبك.", error = ex.Message });
            }
        }
        //[Authorize(Roles = "عميد الكلية,أمين الكلية,مدير الموارد البشرية")]
        [HttpGet("GetAllActiveUsers")]
        public async Task<IActionResult> GetAllActiveUsers()
        {
            try
            {
                var users = await _accountService.GetAllActiveUsers();
                if (!users.Any()) { return NotFound(new { Message = "لم يتم العثور على أي مستخدمين." }); }

                var ReturnedUsers = _mapper.Map<IEnumerable<UserDTO>>(users);
                foreach(var ReturnedUser in ReturnedUsers)
                {
                    var user = await _accountService.FindById(ReturnedUser.Id);
                    ReturnedUser.FullName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                    ReturnedUser.RoleName = await _accountService.GetFirstRole(user);
                    if (user.Departement_ID != null)
                    {
                        var dpt = await _deptBase.Get(d => d.Id == user.Departement_ID);
                        ReturnedUser.DepartmentName = dpt.Name;
                    }
                    var DepartmentManager = await _deptBase.Get(d => d.ManagerId == ReturnedUser.Id);
                    if (DepartmentManager != null) ReturnedUser.IsDirectManager = true;
                    else ReturnedUser.IsDirectManager = false;
                }
                return Ok(ReturnedUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء معالجة طلبك.", error = ex.Message });
            }
        }
        //[Authorize(Roles = "عميد الكلية,أمين الكلية,مدير الموارد البشرية")]
        [HttpGet("GetAllNonActiveUsers")]
        public async Task<IActionResult> GetAllNonActiveUsers()
        {
            try
            {
                var users = await _accountService.GetAllNonActiveUsers();
                if (!users.Any()) { return NotFound(new { Message = "لم يتم العثور على أي مستخدمين." }); }

                var ReturnedUsers = _mapper.Map<IEnumerable<UserDTO>>(users);
                foreach (var ReturnedUser in ReturnedUsers)
                {
                    var user = await _accountService.FindById(ReturnedUser.Id);
                    ReturnedUser.FullName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                    ReturnedUser.RoleName = await _accountService.GetFirstRole(user);
                    if (user.Departement_ID != null)
                    {
                        var dpt = await _deptBase.Get(d => d.Id == user.Departement_ID);
                        ReturnedUser.DepartmentName = dpt.Name;
                    }
                    var DepartmentManager = await _deptBase.Get(d => d.ManagerId == ReturnedUser.Id);
                    if (DepartmentManager != null) ReturnedUser.IsDirectManager = true;
                    else ReturnedUser.IsDirectManager = false;
                }
                return Ok(ReturnedUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء معالجة طلبك.", error = ex.Message });
            }
        }
        //[Authorize]
        [HttpGet("GetAllAvailabelCoworkers/{userId}")]
        public async Task<IActionResult> GetAllAvailableCoworkers(string userId)
        {
            try
            {
                var u = await _accountService.FindById(userId);
                if (u == null) return NotFound(new { Message = "لم يتم العثور على المستخدم." });

                var users = await _accountService.GetAllActiveAvailableCoworkers(u);
                if (!users.Any()) { return NotFound(new { Message = "لم يتم العثور على أي مستخدمين." }); }

                var ReturnedUsers = _mapper.Map<IEnumerable<CoworkerDTO>>(users);
                foreach (var ReturnedUser in ReturnedUsers)
                {
                    var user = await _accountService.FindById(ReturnedUser.Id);
                    ReturnedUser.FullName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                    ReturnedUser.RoleName = await _accountService.GetFirstRole(user);
                    if (user.Departement_ID != null)
                    {
                        var dpt = await _deptBase.Get(d => d.Id == user.Departement_ID);
                        ReturnedUser.DepartmentName = dpt.Name;
                    }
                }
                return Ok(ReturnedUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء معالجة طلبك.", error = ex.Message });
            }
        }
        //[Authorize(Roles = "عميد الكلية,أمين الكلية,مدير الموارد البشرية")]
        [HttpGet("GetAllUsersByDepartmentId/{DepartmentId}")]
        public async Task<IActionResult> GetAllUsersByDepartmentId(int DepartmentId)
        {
            if (DepartmentId <= 0)
                return BadRequest(new { Message = "معرف القسم غير صالح." });
            try
            {
                var dept = await _deptBase.Get(d=>d.Id==DepartmentId);
                if (dept == null) return NotFound(new { Message = "لم يتم العثور على قسم بهذا المعرف." });

                var users = await _accountService.GetAllUsersByDepartmentId(DepartmentId);
                if (!users.Any()) return NotFound(new { Message = "لم يتم العثور على مستخدمين في هذا القسم." });

                var ReturnedUsers = _mapper.Map<IEnumerable<UserDTO>>(users);
                foreach (var ReturnedUser in ReturnedUsers)
                {
                    var user = await _accountService.FindById(ReturnedUser.Id);
                    ReturnedUser.FullName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                    ReturnedUser.RoleName = await _accountService.GetFirstRole(user);
                    if (user.Departement_ID != null)
                    {
                        var dpt = await _deptBase.Get(d => d.Id == user.Departement_ID);
                        ReturnedUser.DepartmentName = dpt.Name;
                    }
                    var DepartmentManager = await _deptBase.Get(d => d.ManagerId == ReturnedUser.Id);
                    if (DepartmentManager != null) ReturnedUser.IsDirectManager = true;
                    else ReturnedUser.IsDirectManager = false;
                }
                return Ok(ReturnedUsers);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء معالجة طلبك.", error = ex.Message });
            }
        }
        //[HttpPost("upload-users-excel")]
        //public async Task<IActionResult> UploadUsersExcel(IFormFile file)
        //{
        //    // 1. Validate File
        //    if (file == null || file.Length == 0)
        //        return BadRequest("No file uploaded.");

        //    if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
        //        return BadRequest("Only .xlsx files are allowed.");

        //    var results = new List<string>();
        //    var validationErrors = new List<string>();

        //    using var stream = new MemoryStream();
        //    await file.CopyToAsync(stream);

        //    // 2. Set EPPlus LicenseContext (critical for EPPlus 5+)
        //    ExcelPackage.LicenseContext = LicenseContext.NonCommercial; // or Commercial

        //    using var package = new ExcelPackage(stream);
        //    ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
        //    int rowCount = worksheet.Dimension?.Rows ?? 0;

        //    if (rowCount < 2)
        //        return BadRequest("Excel file has no data rows.");

        //    // 3. Process Each Row
        //    for (int row = 2; row <= rowCount; row++)
        //    {
        //        try
        //        {
        //            // 4. Parse and Validate Data
        //            if (!DateTime.TryParse(worksheet.Cells[row, 9].Text, out var dateOfBirth))
        //            {
        //                results.Add($"Row {row}: تاريخ الميلاد غير صالح.");
        //                continue;
        //            }

        //            if (!DateTime.TryParse(worksheet.Cells[row, 11].Text, out var hireDate))
        //            {
        //                results.Add($"Row {row}: تاريخ التعيين غير صالح.");
        //                continue;
        //            }

        //            var dto = new CreateUserDTO
        //            {
        //                UserName = worksheet.Cells[row, 1].Text?.Trim(),
        //                PhoneNumber = worksheet.Cells[row, 2].Text?.Trim(),
        //                Email = worksheet.Cells[row, 3].Text?.Trim(),
        //                Password = worksheet.Cells[row, 4].Text?.Trim(),
        //                FirstName = worksheet.Cells[row, 5].Text?.Trim(),
        //                SecondName = worksheet.Cells[row, 6].Text?.Trim(),
        //                ThirdName = worksheet.Cells[row, 7].Text?.Trim(),
        //                ForthName = worksheet.Cells[row, 8].Text?.Trim(),
        //                DateOfBirth = dateOfBirth,
        //                Gender = worksheet.Cells[row, 10].Text?.Trim(),
        //                HireDate = hireDate,
        //                NationalID = worksheet.Cells[row, 12].Text?.Trim(),
        //                position = int.TryParse(worksheet.Cells[row, 13].Text, out var pos) ? pos : 0,
        //                Departement_ID = string.IsNullOrWhiteSpace(worksheet.Cells[row, 14].Text)
        //                    ? null
        //                    : int.TryParse(worksheet.Cells[row, 14].Text, out var dept) ? dept : null,
        //                Disability = worksheet.Cells[row, 15].Text?.Trim() == "نعم",
        //                Street = worksheet.Cells[row, 16].Text?.Trim(),
        //                governorate = worksheet.Cells[row, 17].Text?.Trim(),
        //                State = worksheet.Cells[row, 18].Text?.Trim(),
        //                NormalLeavesCount = int.TryParse(worksheet.Cells[row, 19].Text, out var normal) ? normal : 0,
        //                CasualLeavesCount = int.TryParse(worksheet.Cells[row, 20].Text, out var casual) ? casual : 0,
        //                NonChronicSickLeavesCount = int.TryParse(worksheet.Cells[row, 21].Text, out var sick) ? sick : 0,
        //                NormalLeavesCount_47 = int.TryParse(worksheet.Cells[row, 22].Text, out var n47) ? n47 : 0,
        //                NormalLeavesCount_81Before3Years = int.TryParse(worksheet.Cells[row, 23].Text, out var n81_3) ? n81_3 : 0,
        //                NormalLeavesCount_81Before2Years = int.TryParse(worksheet.Cells[row, 24].Text, out var n81_2) ? n81_2 : 0,
        //                NormalLeavesCount_81Before1Years = int.TryParse(worksheet.Cells[row, 25].Text, out var n81_1) ? n81_1 : 0,
        //                HowManyDaysFrom81And47 = int.TryParse(worksheet.Cells[row, 26].Text, out var totalDays) ? totalDays : 0
        //            };

        //            // 5. Validate DTO (using DataAnnotations)
        //            var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(dto);
        //            var validationResults = new List<ValidationResult>();
        //            bool isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);

        //            if (!isValid)
        //            {
        //                var errorMessages = validationResults.Select(v => v.ErrorMessage);
        //                results.Add($"Row {row}: خطأ في التحقق - {string.Join(" | ", errorMessages)}");
        //                continue;
        //            }
        //            // 6. Map DTO to ApplicationUser (manual mapping)
        //            var user = new ApplicationUser
        //            {
        //                UserName = dto.UserName,
        //                Email = dto.Email,
        //                PhoneNumber = dto.PhoneNumber,
        //                FirstName = dto.FirstName,
        //                SecondName = dto.SecondName,
        //                ThirdName = dto.ThirdName,
        //                ForthName = dto.ForthName,
        //                DateOfBirth = dto.DateOfBirth,
        //                Gender = dto.Gender,
        //                HireDate = dto.HireDate,
        //                NationalID = dto.NationalID,
        //                position = dto.position,
        //                Departement_ID = dto.Departement_ID,
        //                Disability = dto.Disability,
        //                Street = dto.Street,
        //                Governorate = dto.governorate,
        //                State = dto.State,
        //                NormalLeavesCount = dto.NormalLeavesCount,
        //                CasualLeavesCount = dto.CasualLeavesCount,
        //                NonChronicSickLeavesCount = dto.NonChronicSickLeavesCount,
        //                NormalLeavesCount_47 = dto.NormalLeavesCount_47,
        //                NormalLeavesCount_81Before3Years = dto.NormalLeavesCount_81Before3Years,
        //                NormalLeavesCount_81Before2Years = dto.NormalLeavesCount_81Before2Years,
        //                NormalLeavesCount_81Before1Years = dto.NormalLeavesCount_81Before1Years,
        //                HowManyDaysFrom81And47 = dto.HowManyDaysFrom81And47,
        //            };

        //            // 7. Create User
        //            var createResult = await _userManager.CreateAsync(user, dto.Password);

        //            if (!createResult.Succeeded)
        //            {
        //                var errors = createResult.Errors.Select(e => e.Description);
        //                results.Add($"Row {row}: فشل إنشاء المستخدم - {string.Join(" | ", errors)}");
        //                continue;
        //            }

        //            results.Add($"Row {row}: تم إنشاء المستخدم بنجاح.");
        //        }
        //        catch (Exception ex)
        //        {
        //            results.Add($"Row {row}: خطأ غير متوقع - {ex.Message}");
        //        }
        //    }

        //    // 8. Return Structured Response
        //    var successCount = results.Count(r => r.Contains("نجاح"));
        //    var errorCount = results.Count - successCount;

        //    return Ok(new
        //    {
        //        TotalRowsProcessed = rowCount - 1,
        //        SuccessCount = successCount,
        //        ErrorCount = errorCount,
        //        Results = results
        //    });
        //}
        [HttpPost("upload-users-excel")]
        public async Task<IActionResult> UploadUsersExcel(IFormFile file)
        {
            // 1. Validate File
            if (file == null || file.Length == 0)
                return BadRequest("No file uploaded.");

            if (!Path.GetExtension(file.FileName).Equals(".xlsx", StringComparison.OrdinalIgnoreCase))
                return BadRequest("Only .xlsx files are allowed.");

            var results = new List<string>();
            var validationErrors = new List<string>();

            using var stream = new MemoryStream();
            await file.CopyToAsync(stream);

            // 2. Set EPPlus LicenseContext
            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using var package = new ExcelPackage(stream);
            ExcelWorksheet worksheet = package.Workbook.Worksheets[0];
            int rowCount = worksheet.Dimension?.Rows ?? 0;

            if (rowCount < 2)
                return BadRequest("Excel file has no data rows.");

            // Define column indexes (adjust these numbers based on your actual Excel structure)
            const int UserNameCol = 1;
            const int PhoneNumberCol = 2;
            const int EmailCol = 3;
            const int PasswordCol = 4;
            const int FirstNameCol = 5;
            const int SecondNameCol = 6;
            const int ThirdNameCol = 7;
            const int ForthNameCol = 8;
            const int DateOfBirthCol = 9;
            const int GenderCol = 10;
            const int HireDateCol = 11;
            const int NationalIDCol = 12;
            const int PositionCol = 13;
            const int DepartmentCol = 14;
            const int DisabilityCol = 15;
            const int StreetCol = 16;
            const int GovernorateCol = 17;
            const int StateCol = 18;
            const int NormalLeavesCol = 19;
            const int CasualLeavesCol = 20;
            const int SickLeavesCol = 21;
            const int Leaves47Col = 22;
            const int Leaves81_3Col = 23;
            const int Leaves81_2Col = 24;
            const int Leaves81_1Col = 25;
            const int TotalDaysCol = 26;
            const int RoleNameCol = 27; // Column for role name (not part of DTO)

            // 3. Process Each Row
            for (int row = 2; row <= rowCount; row++)
            {
                try
                {
                    // 4. Parse and Validate Data
                    if (!DateTime.TryParse(worksheet.Cells[row, DateOfBirthCol].Text, out var dateOfBirth))
                    {
                        results.Add($"Row {row}: تاريخ الميلاد غير صالح.");
                        continue;
                    }

                    if (!DateTime.TryParse(worksheet.Cells[row, HireDateCol].Text, out var hireDate))
                    {
                        results.Add($"Row {row}: تاريخ التعيين غير صالح.");
                        continue;
                    }
                    bool Disability = false;
                    var disabilityValue = worksheet.Cells[row, DisabilityCol].Value;

                    if (disabilityValue != null)
                    {
                        if (disabilityValue is string strValue)
                        {
                            Disability = strValue.Trim() == "1";
                        }
                        else if (disabilityValue is int intValue)
                        {
                            Disability = intValue == 1;
                        }
                        else if (double.TryParse(disabilityValue.ToString(), out double doubleValue))
                        {
                            Disability = doubleValue == 1;
                        }
                    }

                    // Get role name from Excel (not part of DTO validation)
                    var roleName = worksheet.Cells[row, RoleNameCol].Text?.Trim();

                    var dto = new CreateUserDTO
                    {
                        UserName = worksheet.Cells[row, UserNameCol].Text?.Trim(),
                        PhoneNumber = worksheet.Cells[row, PhoneNumberCol].Text?.Trim(),
                        Email = worksheet.Cells[row, EmailCol].Text?.Trim(),
                        Password = worksheet.Cells[row, PasswordCol].Text?.Trim(),
                        FirstName = worksheet.Cells[row, FirstNameCol].Text?.Trim(),
                        SecondName = worksheet.Cells[row, SecondNameCol].Text?.Trim(),
                        ThirdName = worksheet.Cells[row, ThirdNameCol].Text?.Trim(),
                        ForthName = worksheet.Cells[row, ForthNameCol].Text?.Trim(),
                        DateOfBirth = dateOfBirth,
                        Gender = worksheet.Cells[row, GenderCol].Text?.Trim(),
                        HireDate = hireDate,
                        NationalID = worksheet.Cells[row, NationalIDCol].Text?.Trim(),
                        position = int.TryParse(worksheet.Cells[row, PositionCol].Text, out var pos) ? pos : 0,
                        Departement_ID = string.IsNullOrWhiteSpace(worksheet.Cells[row, DepartmentCol].Text)
                            ? null
                            : int.TryParse(worksheet.Cells[row, DepartmentCol].Text, out var dept) ? dept : null,
                        Disability = Disability,
                        //Disability = worksheet.Cells[row, DisabilityCol].Text?.Trim() == "نعم",
                        Street = worksheet.Cells[row, StreetCol].Text?.Trim(),
                        governorate = worksheet.Cells[row, GovernorateCol].Text?.Trim(),
                        State = worksheet.Cells[row, StateCol].Text?.Trim(),
                        NormalLeavesCount = int.TryParse(worksheet.Cells[row, NormalLeavesCol].Text, out var normal) ? normal : 0,
                        CasualLeavesCount = int.TryParse(worksheet.Cells[row, CasualLeavesCol].Text, out var casual) ? casual : 0,
                        NonChronicSickLeavesCount = int.TryParse(worksheet.Cells[row, SickLeavesCol].Text, out var sick) ? sick : 0,
                        NormalLeavesCount_47 = int.TryParse(worksheet.Cells[row, Leaves47Col].Text, out var n47) ? n47 : 0,
                        NormalLeavesCount_81Before3Years = int.TryParse(worksheet.Cells[row, Leaves81_3Col].Text, out var n81_3) ? n81_3 : 0,
                        NormalLeavesCount_81Before2Years = int.TryParse(worksheet.Cells[row, Leaves81_2Col].Text, out var n81_2) ? n81_2 : 0,
                        NormalLeavesCount_81Before1Years = int.TryParse(worksheet.Cells[row, Leaves81_1Col].Text, out var n81_1) ? n81_1 : 0,
                        HowManyDaysFrom81And47 = int.TryParse(worksheet.Cells[row, TotalDaysCol].Text, out var totalDays) ? totalDays : 0
                    };

                    // 5. Validate DTO (without role name)
                    var validationContext = new System.ComponentModel.DataAnnotations.ValidationContext(dto);
                    var validationResults = new List<ValidationResult>();
                    bool isValid = Validator.TryValidateObject(dto, validationContext, validationResults, true);

                    if (!isValid)
                    {
                        var errorMessages = validationResults.Select(v => v.ErrorMessage);
                        results.Add($"Row {row}: خطأ في التحقق - {string.Join(" | ", errorMessages)}");
                        continue;
                    }

                    // 6. Map DTO to ApplicationUser
                    var user = new ApplicationUser
                    {
                        UserName = dto.UserName,
                        Email = dto.Email,
                        PhoneNumber = dto.PhoneNumber,
                        FirstName = dto.FirstName,
                        SecondName = dto.SecondName,
                        ThirdName = dto.ThirdName,
                        ForthName = dto.ForthName,
                        DateOfBirth = dto.DateOfBirth,
                        Gender = dto.Gender,
                        HireDate = dto.HireDate,
                        NationalID = dto.NationalID,
                        position = dto.position,
                        Departement_ID = dto.Departement_ID,
                        Disability = dto.Disability,
                        Street = dto.Street,
                        Governorate = dto.governorate,
                        State = dto.State,
                        NormalLeavesCount = dto.NormalLeavesCount,
                        CasualLeavesCount = dto.CasualLeavesCount,
                        NonChronicSickLeavesCount = dto.NonChronicSickLeavesCount,
                        NormalLeavesCount_47 = dto.NormalLeavesCount_47,
                        NormalLeavesCount_81Before3Years = dto.NormalLeavesCount_81Before3Years,
                        NormalLeavesCount_81Before2Years = dto.NormalLeavesCount_81Before2Years,
                        NormalLeavesCount_81Before1Years = dto.NormalLeavesCount_81Before1Years,
                        HowManyDaysFrom81And47 = dto.HowManyDaysFrom81And47,
                    };

                    // 7. Create User
                    user.Active = true;
                    var init = user.HireDate;
                    var today = DateTime.UtcNow;
                    user.YearsOfWork = today.Year - init.Year;
                    if (init.Month < 7)
                    {
                        user.YearsOfWork++;
                    }
                    if (today.Month < 7)
                    {
                        user.YearsOfWork--;
                    }
                    if (user.Disability == true)
                    {
                        user.LeaveSection = NormalLeaveSection.DisabilityEmployee;
                    }
                    else
                    {
                        //var hireDuration = (DateTime.UtcNow.Date - user.HireDate).TotalDays;
                        // validation on leaves count from front
                        var ageInYears = DateTime.UtcNow.Year - user.DateOfBirth.Year;
                        if (DateTime.UtcNow.Month < user.DateOfBirth.Month ||
                           (DateTime.UtcNow.Month == user.DateOfBirth.Month && DateTime.UtcNow.Day < user.DateOfBirth.Day))
                        {
                            ageInYears--;
                        }
                        if (ageInYears >= 50) user.LeaveSection = NormalLeaveSection.FiftyAge;
                        else
                        {
                            if (user.YearsOfWork == 0) user.LeaveSection = NormalLeaveSection.NoSection;
                            if (user.YearsOfWork >= 1) user.LeaveSection = NormalLeaveSection.OneYear;
                            if (user.YearsOfWork >= 10) user.LeaveSection = NormalLeaveSection.TenYears;
                        }
                    }
                    var createResult = await _userManager.CreateAsync(user, dto.Password);


                    if (!createResult.Succeeded)
                    {
                        var errors = createResult.Errors.Select(e => e.Description);
                        results.Add($"Row {row}: فشل إنشاء المستخدم - {string.Join(" | ", errors)}");
                        continue;
                    }

                    // 8. Assign Role if specified (handled separately from DTO)
                    if (!string.IsNullOrWhiteSpace(roleName))
                    {
                        try
                        {
                            var roleExists = await _roleService.IsRoleExisted(roleName);
                            if (!roleExists)
                            {
                                results.Add($"Row {row}: تم إنشاء المستخدم بنجاح ولكن الدور '{roleName}' غير موجود.");
                                continue;
                            }

                            var addToRoleResult = await _userManager.AddToRoleAsync(user, roleName);
                            if (!addToRoleResult.Succeeded)
                            {
                                var roleErrors = addToRoleResult.Errors.Select(e => e.Description);
                                results.Add($"Row {row}: تم إنشاء المستخدم بنجاح ولكن فشل تعيين الدور - {string.Join(" | ", roleErrors)}");
                                continue;
                            }

                            results.Add($"Row {row}: تم إنشاء المستخدم وتعيين الدور '{roleName}' بنجاح.");
                        }
                        catch (Exception roleEx)
                        {
                            results.Add($"Row {row}: تم إنشاء المستخدم بنجاح ولكن حدث خطأ أثناء تعيين الدور - {roleEx.Message}");
                        }
                    }
                    else
                    {
                        results.Add($"Row {row}: تم إنشاء المستخدم بنجاح (بدون تعيين دور).");
                    }
                }
                catch (Exception ex)
                {
                    results.Add($"Row {row}: خطأ غير متوقع - {ex.Message}");
                }
            }

            // 9. Return Structured Response
            var successCount = results.Count(r => r.Contains("تم إنشاء المستخدم بنجاح"));
            var errorCount = results.Count - successCount;

            return Ok(new
            {
                TotalRowsProcessed = rowCount - 1,
                SuccessCount = successCount,
                ErrorCount = errorCount,
                Results = results
            });
        }

        //[Authorize(Roles = "مدير الموارد البشرية")]
        [HttpPost("CreateUser/{RoleName}")]
        public async Task<IActionResult> CreateUser([FromRoute]string RoleName, [FromBody] CreateUserDTO model) 
        {
            try
            {
                if (ModelState.IsValid)
                {
                    if (RoleName == "عميد الكلية" || RoleName == "أمين الكلية" || RoleName == "مدير الموارد البشرية") // ??????????
                    {
                        var list = await _accountService.GetAllUsersInRole(RoleName);
                        if (list.Any()) return BadRequest(new { Message = $"يوجد بالفعل مستخدم لديه دور {RoleName}، يجب أن يُخصص هذا الدور لمستخدم واحد فقط." });
                    }
                    if (await _accountService.FindByEmail(model.Email) is not null)
                        return BadRequest(new { Message = "هذا البريد الإلكتروني مسجل بالفعل!" });

                    if (await _accountService.FindByNationalId(model.NationalID) is not null)
                        return BadRequest(new { Message = "هذا الرقم القومي مسجل بالفعل!" });

                    if (await _accountService.FindByName(model.UserName) is not null)
                        return BadRequest(new { Message = "هذا اسم المستخدم مسجل بالفعل!" });

                    if (!await _roleService.IsRoleExisted(RoleName))
                        return BadRequest(new { Message = "معرف المستخدم أو المنصب غير صالح!" });

                    if (model.Departement_ID != null)
                    {
                        if (await _deptBase.Get(d => d.Id == model.Departement_ID) is null)
                            return BadRequest(new { Message = "القسم غير صالح!" });
                    }
                    var res = await _accountService.Create(RoleName,model);
                    if (res.Succeeded)
                    {
                        var user = await _accountService.FindByNationalId(model.NationalID);
                        var ress = await _accountService.AddUserToRole(user, RoleName);
                        if (ress.Succeeded)
                        {
                            var ReturnedUser = _mapper.Map<UserDTO>(user);
                            ReturnedUser.FullName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                            if(user.Departement_ID != null)
                            {
                                var dpt = await _deptBase.Get(d => d.Id == user.Departement_ID);
                                ReturnedUser.DepartmentName = dpt.Name;
                            }
                            var DepartmentManager = await _deptBase.Get(d => d.ManagerId == ReturnedUser.Id);
                            if (DepartmentManager != null) ReturnedUser.IsDirectManager = true;
                            else ReturnedUser.IsDirectManager = false;
                            return CreatedAtAction(nameof(GetUserById), new { userID = user.Id }, ReturnedUser);
                        }
                        return BadRequest(ress.Errors);
                    }
                    return BadRequest(res.Errors);
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء معالجة طلبك.", error = ex.Message });
            }
        }
        //[AllowAnonymous]
        [HttpPost("UserLogin")]
        public async Task<IActionResult> Login([FromBody] LogInUserDTO model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _accountService.FindByName(model.UserName);
                    bool checkPassword = await _accountService.CheckPassword(user, model.Password);
                    if (user is null || !checkPassword)
                    {
                        return Unauthorized(new { Message = "أسم المستخدم أو كلمة المرور غير صحيحة!" });
                    }

                    var res = await _accountService.GetTokenAsync(user);
                    if (res.IsAuthenticated)
                    {
                        var DepartmentManager = await _deptBase.Get(d => d.ManagerId == res.Id);
                        if (DepartmentManager != null) res.IsDirectManager = true;
                        else res.IsDirectManager = false;
                        await _accountService.TransferingUserNormalLeaveCountToNewSection(user);
                        return Ok(res);
                    }
                    return Unauthorized(new {Message = res.Message});
                }
                return BadRequest(ModelState);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء معالجة طلبك.", error = ex.Message });
            }
        }
        //[Authorize(Roles = "مدير الموارد البشرية")]
        [HttpPut("UpdateUser/{userid}")]
        public async Task<IActionResult> UdpateUser(string userid,[FromBody]UpdateUserDTO model)
        {
            if (string.IsNullOrWhiteSpace(userid) || string.IsNullOrWhiteSpace(model.RoleName))
                return BadRequest(new { message = "معرف المستخدم أو المنصب غير صحيح." });


            try
            {
                if (ModelState.IsValid)
                {
                    var user = await _accountService.FindById(userid);
                    if (user == null) return NotFound(new { Message = "لم يتم العثور على المستخدم." });

                    if (model.RoleName == "عميد الكلية" || model.RoleName == "أمين الكلية" || model.RoleName == "مدير الموارد البشرية") // ??????????
                    {
                        var list = await _accountService.GetAllUsersInRole(model.RoleName);
                        if (list.Any())
                        {
                            var manager = list.FirstOrDefault();
                            if (manager != null)
                            {
                                if (manager.Id != userid)
                                    return BadRequest(new { Message = $"يوجد بالفعل مستخدم لديه دور {model.RoleName}، يجب أن يكون هذا الدور مخصصًا لمستخدم واحد فقط." });
                            }
                        }
                    }
                    var u = await _accountService.FindByEmail(model.Email);
                    if (u != null)
                    {
                        if (u.Id != userid)
                            return BadRequest(new { Message = "هذا البريد الإلكتروني مسجل بالفعل!" });
                    }

                    u = await _accountService.FindByNationalId(model.NationalID);
                    if (u != null)
                    {
                        if (u.Id != userid)
                            return BadRequest(new { Message = "هذا الرقم القومي مسجل بالفعل!" });
                    }

                    u = await _accountService.FindByName(model.UserName);
                    if (u != null)
                    {
                        if (u.Id != userid)
                            return BadRequest(new { Message = "هذا اسم المستخدم مسجل بالفعل!" });
                    }

                    if (!await _roleService.IsRoleExisted(model.RoleName))
                        return BadRequest(new { Message = "منصب غير صالح!" });

                    if (model.Departement_ID != null)
                    {
                        if (await _deptBase.Get(d => d.Id == model.Departement_ID) is null)
                            return BadRequest(new { Message = "القسم غير صالح!" });
                    }

                    _mapper.Map(model, user);
                    if (model.Disability == true)
                    {
                        user.LeaveSection = NormalLeaveSection.DisabilityEmployee;
                    }
                    else
                    {
                        var ageInYears = DateTime.UtcNow.Year - user.DateOfBirth.Year;
                        if (DateTime.UtcNow.Month < user.DateOfBirth.Month ||
                           (DateTime.UtcNow.Month == user.DateOfBirth.Month && DateTime.UtcNow.Day < user.DateOfBirth.Day))
                        {
                            ageInYears--;
                        }
                        if (ageInYears >= 50) user.LeaveSection = NormalLeaveSection.FiftyAge;
                        else
                        {
                            if (user.YearsOfWork == 0) user.LeaveSection = NormalLeaveSection.NoSection;
                            if (user.YearsOfWork >= 1) user.LeaveSection = NormalLeaveSection.OneYear;
                            if (user.YearsOfWork >= 10) user.LeaveSection = NormalLeaveSection.TenYears;
                        }
                    }

                    var res = await _accountService.Update(user);
                    if (res.Succeeded)
                    {
                        var roles = await _accountService.GetAllRolesOfUser(user);
                        foreach (var role in roles)
                        {
                            await _accountService.RemoveUserFromRole(user, role);
                        }
                        await _accountService.AddUserToRole(user, model.RoleName);

                        var ReturnedUser = _mapper.Map<UserDTO>(user);
                        ReturnedUser.FullName = $"{user.FirstName} {user.SecondName} {user.ThirdName} {user.ForthName}";
                        ReturnedUser.RoleName = await _accountService.GetFirstRole(user);
                        if (user.Departement_ID != null)
                        {
                            var dpt = await _deptBase.Get(d => d.Id == user.Departement_ID);
                            if(dpt!=null) ReturnedUser.DepartmentName = dpt.Name;
                        }
                        var DepartmentManager = await _deptBase.Get(d => d.ManagerId == ReturnedUser.Id);
                        if (DepartmentManager != null) ReturnedUser.IsDirectManager = true;
                        else ReturnedUser.IsDirectManager = false;
                        return Ok(new { Message = "تم التحديث بنجاح.", User = ReturnedUser });
                    }
                    return BadRequest(res.Errors);
                }
                return BadRequest(ModelState);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء معالجة طلبك.", error = ex.Message });
            }
        }
        //[Authorize]
        [HttpPut("UdpateUserForUser/{userid}")]
        public async Task<IActionResult> UdpateUserForUser(string userid, [FromBody] UpdateUserDTOforuser model)
        {
            if (string.IsNullOrWhiteSpace(userid))
                return BadRequest(new { message = "معرف المستخدم غير صالح." });

            try
            {
                if (ModelState.IsValid)
                {
                    //var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                    var user = await _accountService.FindById(userid);
                    if (user == null) return NotFound(new { Message = "لم يتم العثور على المستخدم." });

                    var u = await _accountService.FindByEmail(model.Email);
                    if (u != null)
                    {
                        if (u.Id != userid)
                            return BadRequest(new { Message = "هذا البريد الإلكتروني مسجل بالفعل!" });
                    }

                    user.Email = model.Email;
                    user.PhoneNumber = model.PhoneNumber;
                    user.Street = model.Street;
                    user.Governorate= model.Governorate;
                    user.State= model.State;

                    var res = await _accountService.Update(user);
                    if (res.Succeeded)
                    {
                        return Ok(new { Message = "تم التحديث بنجاح." });
                    }
                    return BadRequest(res.Errors);
                }
                return BadRequest(ModelState);

            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء معالجة طلبك.", error = ex.Message });
            }
        }
        //[Authorize(Roles = "مدير الموارد البشرية")]
        [HttpPut("SoftDeleteUser/{userid}")]
        public async Task<IActionResult> SoftDeleteUser(string userid)
        {
            if (string.IsNullOrWhiteSpace(userid))
                return BadRequest(new { message = "معرف المستخدم غير صالح." });
            try
            {
                var user = await _accountService.FindById(userid);
                if (user != null)
                {
                    var roleName = await _accountService.GetFirstRole(user);
                    if (roleName == "عميد الكلية" || roleName == "أمين الكلية" || roleName == "مدير الموارد البشرية") // ??????????
                    {
                        return BadRequest(new { Message = $"هذا المستخدم لديه منصب {roleName}. قبل حذف هذا المستخدم، يجب تعيين منصب {roleName} لمستخدم جديد." });
                        //return BadRequest(new { Message = $"This user has {roleName} role. Before deleting this user, you should assign {roleName} role to new user." });
                    }
                    var IsDeptHead = await _deptBase.Get(d => d.ManagerId == user.Id);
                    if (IsDeptHead != null)
                    {
                        return BadRequest(new { Message = $"هذا المستخدم هو مدير قسم {IsDeptHead.Name}، يجب تعيين مدير جديد لهذا القسم قبل حذف هذا المستخدم." });
                        // return BadRequest(new { Message = $"This user is {IsDeptHead.Name} department manager, you should assign a new manager to this department before deleting this user." });
                    }
                    if (user.Active)
                    {
                        user.Active = false;
                        await _accountService.Update(user);
                        return Ok(new { Message = "تم الحذف بنجاح." });
                    }
                    else
                    {
                        return BadRequest(new { Message = "المستخدم غير نشط بالفعل." });
                    }


                    //var res = await _accountService.Delete(user);
                    //if (res.Succeeded)
                    //{
                    //    return Ok(new { Message = "Deletion is succeeded" });
                    //}
                    //return BadRequest(res.Errors);
                }
                return NotFound(new { Message = "المستخدم غير موجود." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء معالجة طلبك.", error = ex.Message });
            }
        }
        //[Authorize(Roles = "مدير الموارد البشرية")]
        [HttpPut("ReActiveUser/{userid}")]
        public async Task<IActionResult> ReActiveUser(string userid)
        {
            if (string.IsNullOrWhiteSpace(userid))
                return BadRequest(new { message = "معرف المستخدم غير صالح." });
            try
            {
                var user = await _accountService.FindById(userid);
                if (user != null)
                {
                    if (!user.Active)   
                    {
                        user.Active = true;
                        await _accountService.Update(user);
                        return Ok(new { Message = "تم التنشيط بنجاح." });
                    }
                    else
                    {
                        return BadRequest(new { Message = "المستخدم نشط بالفعل." });
                    }

                    //var res = await _accountService.Delete(user);
                    //if (res.Succeeded)
                    //{
                    //    return Ok(new { Message = "Deletion is succeeded" });
                    //}
                    //return BadRequest(res.Errors);
                }
                return NotFound(new { Message = "المستخدم غير موجود." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "حدث خطأ أثناء معالجة طلبك.", error = ex.Message });
            }
        }
    }
}

