/*
 *  SKBusRouteController.cs
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
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.AspNetCore.Authorization;

namespace SKBusService.Controllers
{
    [Authorize]
    public class SKBusRouteController : Controller
    {
        private readonly BusServiceContext _context;

        // SKBusRouteController constructor
        public SKBusRouteController(BusServiceContext context)
        {
            
            _context = context;    
        }

        /// <summary>
        /// To display index page of bus route
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var records = await _context.BusRoute.OrderBy(a => a.BusRouteCode).ToListAsync();
            //To make sure that all list is ordered by numeric value of BusRouteCode
            records.Sort();
            return View(records);
        }

        // GET SKBusRoute Details page id parameter is passed
        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var busRoute = await _context.BusRoute.SingleOrDefaultAsync(m => m.BusRouteCode == id);
            if (busRoute == null)
            {
                return NotFound();
            }

            return View(busRoute);
        }

        // GET SKBusRoute Create page
        public IActionResult Create()
        {
            
            return View();
        }

        // POST data which is sent by SKBusRoute Create page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BusRouteCode,RouteName")] BusRoute busRoute)
        {
            if (ModelState.IsValid)
            {
                _context.Add(busRoute);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(busRoute);
        }

        // Get SKBusRoute Edit page
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var busRoute = await _context.BusRoute.SingleOrDefaultAsync(m => m.BusRouteCode == id);
            if (busRoute == null)
            {
                return NotFound();
            }
            return View(busRoute);
        }

        // Post updated data which is sent by SKBusRoute Edit page
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("BusRouteCode,RouteName")] BusRoute busRoute)
        {
            if (id != busRoute.BusRouteCode)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(busRoute);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BusRouteExists(busRoute.BusRouteCode))
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
            return View(busRoute);
        }

        // Get SKBusRoute Delete page
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var busRoute = await _context.BusRoute.SingleOrDefaultAsync(m => m.BusRouteCode == id);
            if (busRoute == null)
            {
                return NotFound();
            }

            return View(busRoute);
        }

        // Delete data from database
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var busRoute = await _context.BusRoute.SingleOrDefaultAsync(m => m.BusRouteCode == id);
            _context.BusRoute.Remove(busRoute);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // check if the BusRoute is exist
        private bool BusRouteExists(string id)
        {
            return _context.BusRoute.Any(e => e.BusRouteCode == id);
        }
    }
}
