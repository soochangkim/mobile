/*
 *  SKUserMaintenanceController.cs
 *  Assignment 5
 *  Created By:
 *      Soochang Kim, 7227663
 */
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using SKBusService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SKBusService.Models.AccountViewModels;
using System.Security.Claims;
using SKBusService.Models.Db;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Collections.Generic;

namespace SKBusService.Controllers
{
    [Authorize(Roles = "administrators")] 
    public class SKUserMaintenanceController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly AspNetContext _context;
        public SKUserMaintenanceController(
            UserManager<ApplicationUser> userManger,
            AspNetContext context,
            SignInManager<ApplicationUser> signInManager,
            RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManger;
            _roleManager = roleManager;
            _signInManager = signInManager;
            _context = context;
        }

        
        /// <summary>
        /// Display Index page
        /// </summary>
        /// <returns> Index view </returns>
        public IActionResult Index()
        {
            var allUsers = _context.AspNetUsers.Include(a => a.AspNetUserRoles);
            var sortedUsers = (allUsers.ToList()).OrderByDescending(a => a.LockoutEnd != null).ThenBy(a => a.UserName);
            return View(sortedUsers);
        }

        /// <summary>
        /// To delete user information
        /// </summary>
        /// <param name="id"> user id</param>
        /// <returns> Index page </returns>
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = _userManager.Users.Where(a => a.Id == id).SingleOrDefault();
            
            try
            {
                IdentityResult result;
                var userRoles = _context.AspNetUserRoles.Where(a => a.UserId == id);

                if(userRoles == null || userRoles.Count() == 0)
                    result = await _userManager.DeleteAsync(user);
                // if user role is in administrators
                else if (userRoles.SingleOrDefault(a => a.RoleId =="1") != null ) 
                {
                    throw new Exception("User is in administrators role");
                }
                else
                {
                    _context.AspNetUserRoles.RemoveRange(userRoles);
                    await _context.SaveChangesAsync();
                    result = await _userManager.DeleteAsync(user);
                }
                TempData["message"] = $"User '{user.UserName}' has been deleted.";
            }
            catch (Exception ex)
            {
                TempData["message"] = $"Fail to delete '{user.UserName}': {ex.GetBaseException().Message}.";
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Display reset page
        /// </summary>
        /// <param name="id"> User id</param>
        /// <returns> Register view model</returns>
        public async Task<IActionResult> ResetPassword(string id)
        {
            if (!User.IsInRole("administrators"))
            {
                TempData["message"] = "Sorry, your role doesn't has authority for it";
                return RedirectToAction("Index");
            }
            if (id == null)
            {
                TempData["message"] = "Sorry, the user Id was null";
                return RedirectToAction("Index");
            }

            ApplicationUser user = await _userManager.FindByIdAsync(id);

            var userRoles = _context.AspNetUserRoles.Where(a => a.UserId == id);
            if(userRoles.SingleOrDefault(a => a.RoleId == "1") != null)
            {
                TempData["message"] = "Sorry, the user Id was not found";
                return RedirectToAction("Index");
            }

            if (id == null)
            {
                TempData["message"] = "Sorry, the user Id was not found";
                return RedirectToAction("Index");
            }

            RegisterViewModel rvm = new RegisterViewModel();
            rvm.UserName = user.UserName;
            return View(rvm);
        }

        /// <summary>
        /// Display user reset password
        /// </summary>
        /// <param name="id"> User id</param>
        /// <param name="register"> Model to reset user password </param>
        /// <returns> Reset page</returns>
        [HttpPost]
        public async Task<IActionResult> ResetPassword(string id, RegisterViewModel register)
        {
            if(register == null)
            {
                return NotFound();
            }

            ApplicationUser user = await _userManager.FindByIdAsync(id);
            
            if (user.Id == null)
            {
                return NotFound();
            }

            try
            {
                IdentityResult result;

                await _userManager.RemovePasswordAsync(user);
                var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                result = await _userManager.ResetPasswordAsync(user, code, register.Password);
                if (result.Succeeded)
                {
                    TempData["message"] = $"{user.UserName}'s password was successfully changed";
                    return RedirectToAction("Index");
                }
                else
                {
                    throw new Exception(result.Errors.FirstOrDefault().Description);
                }
            }
            catch(Exception ex)
            {
                TempData["message"] = $"Failed to change password: {ex.GetBaseException().Message}";
            }

            RegisterViewModel rvm = new RegisterViewModel();
            rvm.UserName = user.UserName;
            return View(rvm);
        }

        /// <summary>
        /// Lock user account
        /// </summary>
        /// <param name="user"> user </param>
        /// <returns> success if lock process success</returns>
        private async Task<IdentityResult> lockAccountAsync(ApplicationUser user)
        {
            user.AccessFailedCount = 5;
            user.LockoutEnd = new DateTimeOffset(DateTime.MaxValue, TimeSpan.Zero);
            return await _userManager.UpdateAsync(user);
        }

        /// <summary>
        /// Unlock user account
        /// </summary>
        /// <param name="user"> User </param>
        /// <returns> Result </returns>
        private async Task<IdentityResult> unLockAccountAsync(ApplicationUser user)
        {
            IdentityResult result = await _userManager.ResetAccessFailedCountAsync(user);
            if (result.Succeeded)
                return await _userManager.SetLockoutEndDateAsync(user, null);
            return result;
        }


        /// <summary>
        /// To change user's lock status
        /// </summary>
        /// <param name="id"> user </param>
        /// <returns> index view </returns>
        public async Task<IActionResult> ChangeLockState(string id)
        {           
            try
            {
                ApplicationUser user;
                IdentityResult result;
                user = await _userManager.FindByIdAsync(id);

                result = await ((user.LockoutEnd > DateTime.Now) ? unLockAccountAsync(user) : lockAccountAsync(user));

                if (result.Succeeded)
                {
                    TempData["message"] = $"{((user.LockoutEnd > DateTime.Now) ? "Locked" : "Unlocked")} account for : {user.UserName}";
                }
                else
                {
                    throw new Exception(result.Errors.FirstOrDefault().Description);
                }

            }
            catch(Exception ex)
            {
                TempData["message"] = $"Fail to change state: {ex.GetBaseException().Message}";
            }

            return RedirectToAction("Index");
        }
    }
}