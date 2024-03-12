using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        private readonly ApplicationContext _context;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public UsersController(ApplicationContext context, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _signInManager = signInManager;
        }

        // GET: api/Users
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ApplicationUser>>> GetUsers()
        {
          if (_context.Users == null)
          {
              return NotFound();
          }
            return await _signInManager.UserManager.Users.ToListAsync();
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
        public ActionResult PutApplicationUser(string id, ApplicationUser applicationUser, string? password = null, string? currentPassword = null)
        {
            var existingApplicationUser =  _signInManager.UserManager.FindByIdAsync(id).Result;

            existingApplicationUser.UserName = applicationUser.UserName;
            existingApplicationUser.Email = applicationUser.Email;
            existingApplicationUser.Name = applicationUser.Name;
            existingApplicationUser.PhoneNumber = applicationUser.PhoneNumber;
            existingApplicationUser.StateId = applicationUser.StateId;

            _signInManager.UserManager.UpdateAsync(existingApplicationUser).Wait();

            if (password != null && currentPassword != null)
            {
                IdentityResult identityResult = _signInManager.UserManager.ChangePasswordAsync(existingApplicationUser, currentPassword, password).Result;
            }

            return NoContent();
        }

        // POST: api/Users
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<ApplicationUser>> PostApplicationUser(ApplicationUser applicationUser, string password)
        {
          
            await _signInManager.UserManager.CreateAsync(applicationUser,password);   
            
            return CreatedAtAction("GetApplicationUser", new { id = applicationUser.Id }, applicationUser);
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

            Microsoft.AspNetCore.Identity.SignInResult result = _signInManager.PasswordSignInAsync(applicationUser, password, true, false).Result;

            if (result.Succeeded)
            {
                return true;
            }

            return false;

        }

        private bool ApplicationUserExists(string id)
        {
            return (_context.Users?.Any(e => e.Id == id)).GetValueOrDefault();
        }
    }
}
