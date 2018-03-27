using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Employee_Mg_Asp.NetCore.Models;

namespace Employee_Mg_Asp.NetCore.Data
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            // Customize the ASP.NET Identity model and override the defaults if needed.
            // For example, you can rename the ASP.NET Identity table names and more.
            // Add your customizations after calling base.OnModelCreating(builder);
        }

        public DbSet<Employee_Mg_Asp.NetCore.Models.DepartmentMaster> DepartmentMaster { get; set; }

        public DbSet<Employee_Mg_Asp.NetCore.Models.EmployeeMaster> EmployeeMaster { get; set; }
    }
}
