using System.Collections.Generic;
using Blog.Roles.Dto;

namespace Blog.Web.Models.Roles
{
    public class RoleListViewModel
    {
        public IReadOnlyList<PermissionDto> Permissions { get; set; }
    }
}
