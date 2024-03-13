using System;
using Microsoft.AspNetCore.Identity;
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
        public DbSet<Restaurant>? Restaurants { get; set; }
        public DbSet<Category>? Categories { get; set; }
        public DbSet<Food>? Foods { get; set; }
        public DbSet<RestaurantUser>? RestaurantUsers { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<ApplicationUser>().HasOne(u => u.State).WithMany().OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Restaurant>().HasOne(r => r.State).WithMany().OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Category>().HasOne(c => c.State).WithMany().OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Food>().HasOne(f => f.State).WithMany().OnDelete(DeleteBehavior.NoAction);


            builder.Entity<Restaurant>().HasOne(r => r.Company).WithMany().OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Category>().HasOne(c => c.Restaurant).WithMany().OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Food>().HasOne(f => f.Category).WithMany().OnDelete(DeleteBehavior.NoAction);

            builder.Entity<RestaurantUser>().HasOne(ru => ru.Restaurant).WithMany().OnDelete(DeleteBehavior.NoAction);
            builder.Entity<RestaurantUser>().HasOne(ru => ru.ApplicationUser).WithMany().OnDelete(DeleteBehavior.NoAction);

            builder.Entity<RestaurantUser>().HasKey(ru => new { ru.UserId, ru.RestaurantId });


            builder.Entity<State>().HasData(
                new State { Id = 0, Name = "Delete" },
                new State { Id = 1, Name = "Active" },
                new State { Id = 2, Name = "Passive" });

            builder.Entity<IdentityRole>().HasData(
                new IdentityRole("Administrator"));


            base.OnModelCreating(builder);
        }

    }
}

