using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using Blog.Authorization.Roles;
using Blog.Authorization.Users;
using Blog.MultiTenancy;

namespace Blog.EntityFrameworkCore
{
    public class BlogDbContext : AbpZeroDbContext<Tenant, Role, User, BlogDbContext>
    {
        /* Define a DbSet for each entity of the application */
        
        public BlogDbContext(DbContextOptions<BlogDbContext> options)
            : base(options)
        {
        }
    }
}
