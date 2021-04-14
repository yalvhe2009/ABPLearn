using Abp.Authorization;
using Blog.Authorization.Roles;
using Blog.Authorization.Users;

namespace Blog.Authorization
{
    public class PermissionChecker : PermissionChecker<Role, User>
    {
        public PermissionChecker(UserManager userManager)
            : base(userManager)
        {
        }
    }
}
