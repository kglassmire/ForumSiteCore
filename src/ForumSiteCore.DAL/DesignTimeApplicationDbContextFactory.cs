using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace ForumSiteCore.DAL
{
    public class DesignTimeApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();

            builder.UseNpgsql("Server=localhost; Database=forumsitecore; UserId=postgres; Password=development", opts => opts.CommandTimeout((int)TimeSpan.FromMinutes(60).TotalSeconds));            
            return new ApplicationDbContext(builder.Options);
        }
    }
}
