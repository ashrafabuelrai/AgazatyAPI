
using Agazaty.Application.Common.DTOs.AccountDTOs;
using Agazaty.Application.Common.DTOs.CasualLeaveDTOs;
using Agazaty.Application.Common.DTOs.DepartmentDTOs;
using Agazaty.Application.Common.DTOs.HolidayDTOs;
using Agazaty.Application.Common.DTOs.NormalLeaveDTOs;
using Agazaty.Application.Common.DTOs.PermitLeavesDTOs;
using Agazaty.Application.Common.DTOs.RoleDTOs;
using Agazaty.Application.Common.DTOs.SickLeaveDTOs;
using Agazaty.Domain.Entities;
using AutoMapper;
using Microsoft.AspNetCore.Identity;

namespace Agazaty
{
    public class MappingConfig : Profile
    {
        public MappingConfig()
        {
            CreateMap<PermitLeave, CreatePermitLeaveDTO>().ReverseMap();
            CreateMap<PermitLeave, UpdatePermitLeaveDTO>().ReverseMap();
            CreateMap<PermitLeave, PermitLeaveDTO>().ReverseMap();

            CreateMap<PermitLeaveImage, PermitLeaveImageDTO>().ReverseMap();

            CreateMap<CasualLeave, CreateCasualLeaveDTO>().ReverseMap();
            CreateMap<CasualLeave, UpdateCasualLeaveDTO>().ReverseMap();
            CreateMap<CasualLeave, CasualLeaveDTO>().ReverseMap();

            CreateMap<SickLeave, UpdateSickLeaveDTO>().ReverseMap(); 
            CreateMap<SickLeave, CreateSickLeaveDTO>().ReverseMap();
            CreateMap<SickLeave, SickLeaveDTO>().ReverseMap();

            CreateMap<NormalLeave, CreateNormalLeaveDTO>().ReverseMap(); 
            CreateMap<NormalLeave, UpdateNormalLeaveDTO>().ReverseMap();
            CreateMap<NormalLeave, NormalLeaveDTO>().ReverseMap();

            CreateMap<Department, CreateDepartmentDTO>().ReverseMap();
            CreateMap<Department, UpdateDepartmentDTO>().ReverseMap();
            CreateMap<Department, DepartmentDTO>().ReverseMap();

            CreateMap<ApplicationUser, CreateUserDTO>().ReverseMap();
            CreateMap<ApplicationUser, UpdateUserDTO>().ReverseMap();
            CreateMap<ApplicationUser, UserDTO>().ReverseMap();
            CreateMap<ApplicationUser, CoworkerDTO>().ReverseMap();

            CreateMap<IdentityRole, CreateRoleDTO>().ReverseMap();
            CreateMap<IdentityRole, UpdateRoleDTO>().ReverseMap();
            CreateMap<IdentityRole, RoleDTO>().ReverseMap();

            CreateMap<Holiday, CreateHolidayDTO>().ReverseMap();
            CreateMap<Holiday, UpdateHolidayDTO>().ReverseMap();
        }
    }
}
