/*
 *  SKCountryController.cs
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
    public class SKCountryController : Controller
    {
        private readonly BusServiceContext _context;

        /// <summary>
        /// Class Constructor
        /// </summary>
        /// <param name="context">Database</param>
        public SKCountryController(BusServiceContext context)
        {
            _context = context;    
        }

        /// <summary>
        /// To display index page
        /// </summary>
        /// <returns>View of index page</returns>
        public async Task<IActionResult> Index()
        {
            return View(await _context.Country.ToListAsync());
        }

        /// <summary>
        /// To display details page
        /// </summary>
        /// <param name="id">Id of driver</param>
        /// <returns>View of details page</returns>
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Country.SingleOrDefaultAsync(m => m.CountryCode == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        /// <summary>
        /// To display create page
        /// </summary>
        /// <returns>View of create page</returns>
        public IActionResult Create()
        {
            return View();
        }

        /// <summary>
        /// To Create new record of country
        /// </summary>
        /// <param name="country">Object of country</param>
        /// <returns>To index action only when the creation is success</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CountryCode,Name,PhonePattern,PostalPattern")] Country country)
        {
            if (ModelState.IsValid)
            {
                _context.Add(country);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(country);
        }

        /// <summary>
        /// To display edit page for country record
        /// </summary>
        /// <param name="id">country id</param>
        /// <returns>View of edit page</returns>
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Country.SingleOrDefaultAsync(m => m.CountryCode == id);
            if (country == null)
            {
                return NotFound();
            }
            return View(country);
        }

        /// <summary>
        /// To Update country record
        /// </summary>
        /// <param name="id">Id of country</param>
        /// <param name="country">Objec of country</param>
        /// <returns>To index action only when the update is success</returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("CountryCode,Name,PhonePattern,PostalPattern")] Country country)
        {
            if (id != country.CountryCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(country);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CountryExists(country.CountryCode))
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
            return View(country);
        }

        /// <summary>
        /// To display delete page
        /// </summary>
        /// <param name="id"></param>
        /// <returns>View of delete page</returns>
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var country = await _context.Country.SingleOrDefaultAsync(m => m.CountryCode == id);
            if (country == null)
            {
                return NotFound();
            }

            return View(country);
        }

        /// <summary>
        /// To Delete country record
        /// </summary>
        /// <param name="id">Country Id to be deleted</param>
        /// <returns>To index action</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var country = await _context.Country.SingleOrDefaultAsync(m => m.CountryCode == id);
            _context.Country.Remove(country);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// To check if the country is exists
        /// </summary>
        /// <param name="id">Country Id</param>
        /// <returns>True only when country id is exist</returns>
        private bool CountryExists(string id)
        {
            return _context.Country.Any(e => e.CountryCode == id);
        }
    }
}
