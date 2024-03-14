using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Qr_Menu_API.Data;
using Qr_Menu_API.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Database;

namespace Qr_Menu_API;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.

        builder.Services.AddControllers();
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddDbContext<ApplicationContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("ApplicationDatabase")));
        builder.Services.AddIdentity<ApplicationUser, IdentityRole>()
            .AddEntityFrameworkStores<ApplicationContext>().AddDefaultTokenProviders();
        builder.Services.AddAuthentication();
        builder.Services.AddAuthorization(options => options.AddPolicy("CompAdmin", policy => policy.RequireClaim("CompanyId")));

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapControllers();

        {
            ApplicationContext? applicationContext = app.Services.CreateScope().ServiceProvider.GetService<ApplicationContext>();
            if (applicationContext != null)
            {
                applicationContext.Database.Migrate();
                if (!applicationContext.States.Any())
                {
                    State stateDeleted = new State(0, "Deleted");
                    applicationContext.States.Add(stateDeleted);
                    State stateActive = new State(1, "Active");
                    applicationContext.States.Add(stateActive);
                    State statePassive = new State(2, "Passive");
                    applicationContext.States.Add(statePassive);
                }
                if (!applicationContext.Companies.Any())
                {
                    Company company = new Company();
                    company.AddressDetails = "adres";
                    company.EMail = "abc@def.com";
                    company.Name = "Company";
                    company.Phone = "1112223344";
                    company.PostalCode = "12345";
                    company.RegisterDate = DateTime.Today;
                    company.StateId = 1;
                    company.TaxNumber = "11111111111";
                    applicationContext.Companies.Add(company);
                    applicationContext.SaveChanges();
                    RoleManager<IdentityRole>? roleManager = app.Services.CreateScope().ServiceProvider.GetService<RoleManager<IdentityRole>>();
                    if (roleManager != null)
                    {
                        if (!roleManager.Roles.Any())
                        {
                            IdentityRole administratorRole = new IdentityRole("Administrator");
                            roleManager.CreateAsync(administratorRole).Wait();
                            IdentityRole companyAdministratorRole = new IdentityRole("CompanyAdministrator");
                            roleManager.CreateAsync(companyAdministratorRole).Wait();
                        }
                    }
                    UserManager<ApplicationUser>? userManager = app.Services.CreateScope().ServiceProvider.GetService<UserManager<ApplicationUser>>();
                    if (userManager != null)
                    {
                        if (!userManager.Users.Any())
                        {
                            if (company != null)
                            {
                                ApplicationUser applicationUser = new ApplicationUser();
                                applicationUser.UserName = "Administrator";
                                applicationUser.CompanyId = company.Id;
                                applicationUser.Name = "Administrator";
                                applicationUser.Email = "abc@def.com";
                                applicationUser.PhoneNumber = "1112223344";
                                applicationUser.RegisterDate = DateTime.Today;
                                applicationUser.StateId = 1;

                                userManager.CreateAsync(applicationUser, "Admin123!").Wait();
                                userManager.AddToRoleAsync(applicationUser, "Administrator").Wait();
                            }
                        }
                    }
                }
            }
        }

        app.Run();
    }
}

