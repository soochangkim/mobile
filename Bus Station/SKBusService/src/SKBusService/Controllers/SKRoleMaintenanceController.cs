/*
 *  SKRoleMaintenanceController.cs
 *  Assignment 5
 *  Created By:
 *      Soochang Kim, 7227663
 */
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SKBusService.Data;
using Microsoft.AspNetCore.Http;
using SKBusService.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using SKBusService.Models.Db;
using SKBusService.Models.DB;
using Microsoft.AspNetCore.Authorization;

namespace SKBusService.Controllers
{
    [Authorize(Roles ="administrators")]
    public class SKRoleMaintenanceController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AspNetContext _context;

        public SKRoleMaintenanceController(
            RoleManager<IdentityRole> roleManager,
            UserManager<ApplicationUser> userManager,
            AspNetContext context)
        {
            _roleManager = roleManager;
            _userManager = userManager;
            _context = context;
        }

        /// <summary>
        /// Index view action
        /// </summary>
        /// <returns> Index view to control</returns>
        public async Task<IActionResult> Index()
        {
            return View(await _roleManager.Roles.OrderBy(a => a.Name).ToListAsync());
        }
        
        /// <summary>
        /// Delete view action
        /// </summary>
        /// <param name="id"> role id</param>
        /// <returns> Comfirm page if the role is assigned to any user </returns>
        public async Task<IActionResult> Delete(string id)
        {
            IdentityResult result;

            if (id == null)
            {
                return NotFound();
            }

            var role = await _roleManager.Roles.SingleOrDefaultAsync(m => m.Id == id);
            if (role == null)
            {
                return NotFound();
            }

            if (role.Name == "administrators")
            {
                TempData["message"] = "You cannot delete administrators role";
                return RedirectToAction("Index");
            }

            try
            {
                var users = getUserByRoleId(id);

                if (users.Count() == 0)
                {
                    result = await _roleManager.DeleteAsync(role);
                    TempData["message"] = $"Role name '{role.Name}' has been deleted";
                }
                else
                {
                    HttpContext.Session.SetString(nameof(id), id);
                    return View(users);
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = $"Faile to delete '{role.Name}'role: {ex.GetBaseException().Message}";
            }

            return RedirectToAction("Index");
        }

        
        /// <summary>
        /// Delete user role and role
        /// </summary>
        /// <param name="id">role id </param>
        /// <returns> Index page </returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            try
            {
                var userInRole = _context.AspNetUsers.Include(a => a.AspNetUserRoles);
                ApplicationUser tempUser;

                var role = await _roleManager.FindByIdAsync(id);
                foreach(var user in userInRole)
                {
                    tempUser = await _userManager.FindByIdAsync(user.Id);
                    await _userManager.RemoveFromRoleAsync(tempUser, role.Name);
                }

                IdentityResult result = await _roleManager.DeleteAsync(role);

                if(result.Succeeded)
                {
                    TempData["message"] = $"Role '{role.Name}' has been successfully removed";
                }
                else
                {
                    throw new Exception(result.Errors.SingleOrDefault().Description);
                }
            }
            catch(Exception ex)
            {
                TempData["message"] = $"Fail to remove role: {ex.GetBaseException().Message}";
                return View(id);
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// To manage users in role
        /// </summary>
        /// <param name="id"> role id </param>
        /// <returns> User manage view </returns>
        public async Task<IActionResult> UsersInRole(string id)
        {
            if (id == null)
            {
                id = HttpContext.Session.GetString(nameof(id));
                if(id == null)
                    return NotFound();
            }
            else
            {
                HttpContext.Session.SetString(nameof(id), id);
            }

            var role = await _roleManager.Roles.SingleOrDefaultAsync(m => m.Id == id);

            if (role == null)
            {
                return NotFound();
            }

            HttpContext.Session.SetString(nameof(id), id);
            var users = getUserByRoleId(id);
            ViewBag.otherUsers = new SelectList(_userManager.Users.Except(users), "Id", "UserName");
            return View(users);
        }

        /// <summary>
        /// This will create new role
        /// </summary>
        /// <param name="roleName">role name</param>
        /// <returns> Index </returns>
        [HttpPost]
        public async Task<IActionResult> CreateNewRole(string roleName)
        {
            try
            {
                if (roleName == null || roleName.Trim() == "")
                {
                    throw new Exception("No given role name to add");
                }

                var sameRoleOnFile = _context.AspNetUserRoles.SingleOrDefault(a => a.Role.Name == roleName);

                if (sameRoleOnFile != null)
                {
                    throw new Exception("Sorry, the role name is already in list");
                }


                roleName = roleName.Trim();
                IdentityResult result = await _roleManager.CreateAsync(new IdentityRole(roleName));
                if (result.Succeeded)
                {
                    TempData["message"] = $"A new role '{roleName}' is succesfuly added";
                }
                else
                {
                    throw new Exception(result.Errors.SingleOrDefault().Description);
                }
            }
            catch (Exception ex)
            {
                TempData["message"] = $"Fail to add a new role: {ex.GetBaseException().Message}";
            }
            return RedirectToAction("Index");
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        private IQueryable<ApplicationUser> getUserByRoleId(string id)
        {
            return _userManager.Users.Where(a => a.Roles.SingleOrDefault(b => b.RoleId == id) != null);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<IActionResult> RemoveRoleFromUser(string userId)
        {
            try
            {
                if (userId == null)
                {
                    throw new Exception("No given user Id");
                }

                var user = await _userManager.FindByIdAsync(userId);

                if (user == null)
                {
                    throw new Exception("Cannot find user");
                }

                var role = await _roleManager.FindByIdAsync(HttpContext.Session.GetString("id"));

                if(role == null)
                {
                    throw new Exception("Cannnot find the role");
                }


                AspNetUserRoles userRole = new AspNetUserRoles();
                userRole.RoleId = role.Id;
                userRole.UserId = user.Id;
                _context.AspNetUserRoles.Remove(userRole);
                await _context.SaveChangesAsync();
                TempData["message"] = $"Role '{role.Name}' has been removed from User '{user.UserName}'";

            }
            catch (Exception ex)
            {
                TempData["message"] = $"Fasl to remove role: {ex.GetBaseException().Message}";
            }
            return RedirectToAction("UsersInRole");
        }

        /// <summary>
        /// This will add a new role to user
        /// </summary>
        /// <param name="userId">User id</param>
        /// <returns> User manage page</returns>
        public async Task<IActionResult> AddNewUser(string userId)
        {
            if (userId == null)
                return NotFound();

            try
            {
                string roleId = HttpContext.Session.GetString("id");

                if (roleId == null)
                {
                    TempData["message"] = "Please select user role before to add";
                    return RedirectToAction("Index");
                }

                AspNetUsers user = await _context.AspNetUsers.Where(a => a.Id == userId).SingleOrDefaultAsync();
                if(user == null)
                {
                    throw new Exception("Can not find the user");
                }

                AspNetRoles role = await _context.AspNetRoles.Where(a => a.Id == roleId).SingleOrDefaultAsync();
                if(role == null)
                {
                    throw new Exception("Can not find the role");
                }

                AspNetUserRoles userRole = new AspNetUserRoles();
                userRole.Role = role;
                userRole.RoleId = role.Id;
                userRole.UserId = user.Id;
                _context.Add(userRole);
                await _context.SaveChangesAsync();
                TempData["message"] = $"User '{user.UserName}' has assigned to role '{role.Name}'";

            }
            catch (Exception ex)
            {
                TempData["message"] = $"Fail to add a new user: {ex.GetBaseException().Message}";
            }

            return RedirectToAction("UsersInRole");
        }
    }
}