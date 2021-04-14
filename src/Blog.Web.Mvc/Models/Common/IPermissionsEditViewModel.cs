using System.Collections.Generic;
using Blog.Roles.Dto;

namespace Blog.Web.Models.Common
{
    public interface IPermissionsEditViewModel
    {
        List<FlatPermissionDto> Permissions { get; set; }
    }
}