﻿using System;
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
            builder.Entity<Company>().HasOne(c => c.State).WithMany().OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Restaurant>().HasOne(r => r.State).WithMany().OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Category>().HasOne(c => c.State).WithMany().OnDelete(DeleteBehavior.NoAction);
            builder.Entity<Food>().HasOne(f => f.State).WithMany().OnDelete(DeleteBehavior.NoAction);

            builder.Entity<RestaurantUser>().HasOne(ru => ru.Restaurant).WithMany().OnDelete(DeleteBehavior.NoAction);
            builder.Entity<RestaurantUser>().HasOne(ru => ru.ApplicationUser).WithMany().OnDelete(DeleteBehavior.NoAction);

            builder.Entity<RestaurantUser>().HasKey(ru => new { ru.UserId, ru.RestaurantId });

            base.OnModelCreating(builder);
        }

    }
}

