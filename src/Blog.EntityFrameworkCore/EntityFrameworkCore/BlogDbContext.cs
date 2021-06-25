using Microsoft.EntityFrameworkCore;
using Abp.Zero.EntityFrameworkCore;
using Blog.Authorization.Roles;
using Blog.Authorization.Users;
using Blog.Contracts;
using Blog.MultiTenancy;

namespace Blog.EntityFrameworkCore
{
    public class BlogDbContext : AbpZeroDbContext<Tenant, Role, User, BlogDbContext>
    {
        /* Define a DbSet for each entity of the application */

        /// <summary>
        /// 合同
        /// </summary>
        public DbSet<Contract> Contracts { get; set; }
        
        public BlogDbContext(DbContextOptions<BlogDbContext> options)
            : base(options)
        {
        }
    }
}
