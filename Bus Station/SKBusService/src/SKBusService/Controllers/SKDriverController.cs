/*
 *  SKDriverController.cs
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
using SKBusService.Models;
using Microsoft.AspNetCore.Http;

namespace SKBusService.Controllers
{
    public class SKDriverController : Controller
    {
        private readonly BusServiceContext _context;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="context"></param>
        public SKDriverController(BusServiceContext context)
        {
            _context = context;    
        }

        /// <summary>
        /// To display index page
        /// </summary>
        /// <returns>View of index page</returns>
        public async Task<IActionResult> Index()
        {
            var busServiceContext = _context.Driver.Include(d => d.ProvinceCodeNavigation).OrderBy(a => a.FullName);
            return View(await busServiceContext.ToListAsync());
        }

        /// <summary>
        /// To display details page
        /// </summary>
        /// <param name="id">Driver id</param>
        /// <returns>View of details page</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["message"] = "No driver id given";
                return RedirectToAction("Index");
            }

            var driver = await _context.Driver.SingleOrDefaultAsync(m => m.DriverId == id);
            if (driver == null)
            {
                TempData["message"] = "No record for given driver id";
                return RedirectToAction("Index");
            }
            HttpContext.Session.SetString("Driver", driver.FirstName + " " + driver.LastName);
            return View(driver);
        }

        /// <summary>
        /// To display create page
        /// </summary>
        /// <returns>Vieow of create page</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// To Save new driver record
        /// </summary>
        /// <param name="driver">Driver object</param>
        /// <returns>To index action only when addtion is success</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DriverId,City,DateHired,FirstName,FullName,HomePhone,LastName,PostalCode,ProvinceCode,Street,WorkPhone")] Driver driver)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Add(driver);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "New Driver Record is inserted: " + driver.FirstName + " " + driver.LastName;
                    return RedirectToAction("Index");
                }
                catch(Exception ex)
                {
                    ModelState.AddModelError("", $"Exception thrown on Create: {ex.GetBaseException().Message}");
                }
            }
            return View(driver);
        }

        /// <summary>
        /// To display edit page
        /// </summary>
        /// <param name="id">Driver id</param>
        /// <returns>View of edit page</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["message"] = "No driver id given";
                return RedirectToAction("Index");
            }

            var driver = await _context.Driver.SingleOrDefaultAsync(m => m.DriverId == id);
            if (driver == null)
            {
                TempData["message"] = "No record for given driver id";
                return RedirectToAction("Index");
            }

            HttpContext.Session.SetString("Driver",driver.FirstName + " " + driver.LastName);
            ViewData["Driver"] = driver.FirstName + " " + driver.LastName;
            ViewData["ProvinceCode"] = new SelectList(_context.Province.OrderBy(a => a.Name) , "ProvinceCode", "Name", driver.ProvinceCode);
            return View(driver);
        }

        /// <summary>
        /// To Update driver record
        /// </summary>
        /// <param name="id">Driver id</param>
        /// <param name="driver">Driver object</param>
        /// <returns>To index action only when update is success</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DriverId,City,DateHired,FirstName,FullName,HomePhone,LastName,PostalCode,ProvinceCode,Street,WorkPhone")] Driver driver)
        {
            if (id != driver.DriverId)
            {
                ModelState.AddModelError("", "Invalid driver ID");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(driver);
                    await _context.SaveChangesAsync();
                    TempData["message"] = "Driver record is updated: " + driver.FirstName + " " + driver.LastName;
                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DriverExists(driver.DriverId))
                    {
                        ModelState.AddModelError("", $"Driver ID '{driver.DriverId}' is not registered");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Someone else modified the original record .. re-retrieve it and re-apply your updates");
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", ex.GetBaseException().Message);
                }
            }
            ViewData["ProvinceCode"] = new SelectList(_context.Province, "ProvinceCode", "Name", driver.ProvinceCode);
            return View(driver);
        }

        /// <summary>
        /// To display delete page
        /// </summary>
        /// <param name="id">Driver Id</param>
        /// <returns>View of delete page</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["message"] = "No driver id given";
                return RedirectToAction("Index");
            }

            var driver = await _context.Driver.SingleOrDefaultAsync(m => m.DriverId == id);
            if (driver == null)
            {
                TempData["message"] = "No record for given driver id";
                return RedirectToAction("Index");
            }
            HttpContext.Session.SetString("Driver", driver.FirstName + " " + driver.LastName);
            return View(driver);
        }

        /// <summary>
        /// To Delete driver record
        /// </summary>
        /// <param name="id">Driver Id</param>
        /// <returns>To index action when deletion is success</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var driver = await _context.Driver.SingleOrDefaultAsync(m => m.DriverId == id);
            try
            {
                _context.Driver.Remove(driver);
                await _context.SaveChangesAsync();
            }
            catch(Exception ex)
            {
                ViewData["message"]= "Deletion was failed: " + ex.GetBaseException().Message;
                return View(id);
            }
            TempData["message"] =  "Driver Record is deleted: " + driver.FirstName + " " + driver.LastName;
            return RedirectToAction("Index");
        }

        /// <summary>
        /// To check if the given id is exists
        /// </summary>
        /// <param name="id">Driver id</param>
        /// <returns>True if driver id is exist</returns>
        private bool DriverExists(int id)
        {
            return _context.Driver.Any(e => e.DriverId == id);
        }
    }
}
