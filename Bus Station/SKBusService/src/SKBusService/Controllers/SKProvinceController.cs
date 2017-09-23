/*
 *  SKProvinceController.cs
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

namespace SKBusService.Controllers
{
    public class SKProvinceController : Controller
    {
        private readonly BusServiceContext _context;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="context"></param>
        public SKProvinceController(BusServiceContext context)
        {
            _context = context;    
        }

        /// <summary>
        /// To display index page
        /// </summary>
        /// <returns>View of index page</returns>
        public async Task<IActionResult> Index()
        {
            var busServiceContext = _context.Province.Include(p => p.CountryCodeNavigation);
            return View(await busServiceContext.ToListAsync());
        }

        /// <summary>
        /// To display details
        /// </summary>
        /// <param name="id">Province Id</param>
        /// <returns>View of details page</returns>
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var province = await _context.Province.SingleOrDefaultAsync(m => m.ProvinceCode == id);
            if (province == null)
            {
                return NotFound();
            }

            return View(province);
        }

        /// <summary>
        /// To display create page
        /// </summary>
        /// <returns>View of create page</returns>
        public IActionResult Create()
        {
            ViewData["CountryCode"] = new SelectList(_context.Country, "CountryCode", "CountryCode");
            return View();
        }

        /// <summary>
        /// To Save new province record
        /// </summary>
        /// <param name="province">Province object</param>
        /// <returns>To index action only when the creation is success</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ProvinceCode,Capital,CountryCode,Name,TaxCode,TaxRate")] Province province)
        {
            if (ModelState.IsValid)
            {
                _context.Add(province);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["CountryCode"] = new SelectList(_context.Country, "CountryCode", "CountryCode", province.CountryCode);
            return View(province);
        }

        /// <summary>
        /// To display edit page
        /// </summary>
        /// <param name="id">Province id</param>
        /// <returns>View of edit page</returns>
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var province = await _context.Province.SingleOrDefaultAsync(m => m.ProvinceCode == id);
            if (province == null)
            {
                return NotFound();
            }
            ViewData["CountryCode"] = new SelectList(_context.Country, "CountryCode", "CountryCode", province.CountryCode);
            return View(province);
        }

        /// <summary>
        /// To Update province record
        /// </summary>
        /// <param name="id"></param>
        /// <param name="province"></param>
        /// <returns>To index action only when edition is success</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("ProvinceCode,Capital,CountryCode,Name,TaxCode,TaxRate")] Province province)
        {
            if (id != province.ProvinceCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(province);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProvinceExists(province.ProvinceCode))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["CountryCode"] = new SelectList(_context.Country, "CountryCode", "CountryCode", province.CountryCode);
            return View(province);
        }

        /// <summary>
        /// To display delete page
        /// </summary>
        /// <param name="id">province id</param>
        /// <returns>View of delete page</returns>
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var province = await _context.Province.SingleOrDefaultAsync(m => m.ProvinceCode == id);
            if (province == null)
            {
                return NotFound();
            }

            return View(province);
        }

        /// <summary>
        /// To Delete province record
        /// </summary>
        /// <param name="id">Province id</param>
        /// <returns>To index action only when deletion is success</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var province = await _context.Province.SingleOrDefaultAsync(m => m.ProvinceCode == id);
            _context.Province.Remove(province);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// To check if the province id is exist
        /// </summary>
        /// <param name="id">Province Id</param>
        /// <returns>True only when the province record is exist</returns>
        private bool ProvinceExists(string id)
        {
            return _context.Province.Any(e => e.ProvinceCode == id);
        }
    }
}
