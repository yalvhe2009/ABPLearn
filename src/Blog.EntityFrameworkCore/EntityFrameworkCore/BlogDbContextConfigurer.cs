using System.Data.Common;
using Microsoft.EntityFrameworkCore;

namespace Blog.EntityFrameworkCore
{
    public static class BlogDbContextConfigurer
    {
        public static void Configure(DbContextOptionsBuilder<BlogDbContext> builder, string connectionString)
        {
            builder.UseMySql(connectionString);
        }

        public static void Configure(DbContextOptionsBuilder<BlogDbContext> builder, DbConnection connection)
        {
            builder.UseMySql(connection);
        }
    }
}
