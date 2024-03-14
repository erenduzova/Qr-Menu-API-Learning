using System;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Qr_Menu_API.Models;

namespace Qr_Menu_API.Data
{
	public class DbInitializer
	{
		private readonly ApplicationContext _applicationContext;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        public DbInitializer(ApplicationContext applicationContext, UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
		{
			_applicationContext = applicationContext;
            _userManager = userManager;
            _roleManager = roleManager;
        }


		public void Initialize()
        {
            if (_applicationContext != null)
            {
                _applicationContext.Database.Migrate();
                if (!_applicationContext.States.Any())
                {
                    State stateDeleted = new State(0, "Deleted");
                    _applicationContext.States.Add(stateDeleted);
                    State stateActive = new State(1, "Active");
                    _applicationContext.States.Add(stateActive);
                    State statePassive = new State(2, "Passive");
                    _applicationContext.States.Add(statePassive);
                }
                if (!_applicationContext.Companies.Any())
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
                    _applicationContext.Companies.Add(company);
                    _applicationContext.SaveChanges();
                    if (_roleManager != null)
                    {
                        if (!_roleManager.Roles.Any())
                        {
                            IdentityRole administratorRole = new IdentityRole("Administrator");
                            _roleManager.CreateAsync(administratorRole).Wait();
                            IdentityRole companyAdministratorRole = new IdentityRole("CompanyAdministrator");
                            _roleManager.CreateAsync(companyAdministratorRole).Wait();
                        }
                    }
                    if (_userManager != null)
                    {
                        if (!_userManager.Users.Any())
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

                                _userManager.CreateAsync(applicationUser, "Admin123!").Wait();
                                _userManager.AddToRoleAsync(applicationUser, "Administrator").Wait();
                            }
                        }
                    }
                }
            }
        }


    }
}

