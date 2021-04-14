using System.Collections.Generic;
using Blog.Roles.Dto;

namespace Blog.Web.Models.Users
{
    public class UserListViewModel
    {
        public IReadOnlyList<RoleDto> Roles { get; set; }
    }
}
