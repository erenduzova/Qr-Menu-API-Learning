using System;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Qr_Menu_API.Models;

namespace Qr_Menu_API.Data
{
	public class ApplicationContext : IdentityDbContext<ApplicationUser>
	{
		public ApplicationContext(DbContextOptions<ApplicationContext> options) : base(options)
		{
        }

		public DbSet<Company>? Companies { get; set; }
        public DbSet<State>? States { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>().HasOne(u => u.State).WithMany().OnDelete(DeleteBehavior.NoAction);
            base.OnModelCreating(builder);
        }

    }
}

