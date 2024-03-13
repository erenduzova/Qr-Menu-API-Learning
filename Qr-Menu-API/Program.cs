using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Qr_Menu_API.Data;
using Qr_Menu_API.Models;

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
        builder.Services.AddAuthorization();

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
                if (applicationContext.States.Count() == 0)
                {
                    State stateDeleted = new State(0, "Deleted");
                    applicationContext.States.Add(stateDeleted);
                    State stateActive = new State(1, "Active");
                    applicationContext.States.Add(stateActive);
                    State statePassive = new State(2, "Passive");
                    applicationContext.States.Add(statePassive);
                }
                applicationContext.SaveChanges();
                RoleManager<IdentityRole>? roleManager = app.Services.CreateScope().ServiceProvider.GetService<RoleManager<IdentityRole>>();
                if (roleManager != null)
                {
                    if (roleManager.Roles.Count() == 0)
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
                    if (roleManager.Roles.Count() == 0)
                    {
                        ApplicationUser applicationUser = new ApplicationUser();
                        applicationUser.UserName = "Administrator";
                        userManager.CreateAsync(applicationUser, "Admin123!").Wait();
                        userManager.AddToRoleAsync(applicationUser, "Administrator").Wait();
                    }
                }
            }
        }

        app.Run();
    }
}

