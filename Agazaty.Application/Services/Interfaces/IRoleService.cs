﻿
using Agazaty.Application.Common.DTOs.RoleDTOs;

using Microsoft.AspNetCore.Identity;

namespace Agazaty.Application.Services.Interfaces
{
    public interface IRoleService
    {
        Task<IdentityRole> FindById(string id);
        Task<IdentityRole> FindByName(string RoleName);
        Task<IEnumerable<IdentityRole>> GetAllRoles();
        Task<bool> IsRoleExisted(string RoleName);
        Task<IdentityResult> CreateRole(CreateRoleDTO model);
        Task<IdentityResult> UpdateRole(IdentityRole role);
        Task<IdentityResult> DeleteRole(IdentityRole role);
    }
}
