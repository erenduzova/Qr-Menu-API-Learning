using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Qr_Menu_API.Data;
using Qr_Menu_API.Models;

namespace Qr_Menu_API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(SignInManager<ApplicationUser> signInManager, RoleManager<IdentityRole> roleManager)
        {
            _signInManager = signInManager;
            _roleManager = roleManager;
        }

        // GET: api/Users
        [HttpGet]
        public ActionResult<List<ApplicationUser>> GetUsers()
        {
            return _signInManager.UserManager.Users.ToList();
        }

        // GET: api/Users/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApplicationUser>> GetApplicationUser(string id)
        {
            var applicationUser = await _signInManager.UserManager.FindByIdAsync(id);

            if (applicationUser == null)
            {
                return NotFound();
            }

            return applicationUser;
        }

        // PUT: api/Users/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public ActionResult PutApplicationUser(ApplicationUser applicationUser)
        {
            ApplicationUser existingApplicationUser =  _signInManager.UserManager.FindByIdAsync(applicationUser.Id).Result;

            existingApplicationUser.UserName = applicationUser.UserName;
            existingApplicationUser.Email = applicationUser.Email;
            existingApplicationUser.Name = applicationUser.Name;
            existingApplicationUser.PhoneNumber = applicationUser.PhoneNumber;
            existingApplicationUser.StateId = applicationUser.StateId;

            _signInManager.UserManager.UpdateAsync(existingApplicationUser).Wait();

            return Ok();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [Authorize(Roles = "CompanyAdministrator")]
        [HttpPost]
        public string PostApplicationUser(ApplicationUser applicationUser, string passWord)
        {
            _signInManager.UserManager.CreateAsync(applicationUser, passWord).Wait();
            return applicationUser.Id;
        }

        // DELETE: api/Users/5
        [HttpDelete("{id}")]
        public ActionResult DeleteApplicationUser(string id)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByIdAsync(id).Result;
            if (applicationUser == null)
            {
                return NotFound();
            }
            applicationUser.StateId = 0;
            _signInManager.UserManager.UpdateAsync(applicationUser);

            return Ok();
        }

        // api/Users/LogIn
        [HttpPost("LogIn")]
        public bool LogIn(string userName, string password)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByNameAsync(userName).Result;

            if (applicationUser == null)
            {
                // User not found
                return false;
            }

            Microsoft.AspNetCore.Identity.SignInResult signInResult = _signInManager.PasswordSignInAsync(applicationUser, password, false, false).Result;

            return (signInResult.Succeeded);
        }

        // api/Users/ResetPassword
        [HttpPost("ResetPassword")]
        public void ResetPassword(string userName, string password)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByNameAsync(userName).Result;

            if (applicationUser == null)
            {
                // User not found
                return;
            }

            _signInManager.UserManager.RemovePasswordAsync(applicationUser).Wait();
            _signInManager.UserManager.AddPasswordAsync(applicationUser, password).Wait();

            return;
        }

        // api/Users/ResetPasswordGenerateToken
        [HttpPost("ResetPasswordGenerateToken")]
        public string? ResetPasswordGenerateToken(string userName)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByNameAsync(userName).Result;
            if (applicationUser == null)
            {
                // User not found
                return null;
            }

            return _signInManager.UserManager.GeneratePasswordResetTokenAsync(applicationUser).Result;
        }

        // api/Users/ResetPasswordValidateToken
        [HttpPost("ResetPasswordValidateToken")]
        public ActionResult<String> ResetPasswordValidateToken(string userName, string token, string newPassword)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByNameAsync(userName).Result;
            if (applicationUser == null)
            {
                // User not found
                return NotFound();
            }
            IdentityResult identityResult = _signInManager.UserManager.ResetPasswordAsync(applicationUser, token, newPassword).Result;
            if (identityResult.Succeeded == false)
            {
                return identityResult.Errors.First().Description;
            }
            return Ok();
        }

        [HttpPost("AssignRole")]
        public void AssignRole(string userId, string roleId)
        {
            ApplicationUser applicationUser = _signInManager.UserManager.FindByIdAsync(userId).Result;
            IdentityRole identityRole = _roleManager.FindByIdAsync(roleId).Result;

            _signInManager.UserManager.AddToRoleAsync(applicationUser, identityRole.Name).Wait();
        }

    }
}
