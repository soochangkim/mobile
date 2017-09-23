/*
 *  SKBusStopController.cs
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
    public class SKBusStopController : Controller
    {
        private readonly BusServiceContext _context;

        // SKBusStopController constructor
        public SKBusStopController(BusServiceContext context)
        {
            _context = context;    
        }


        /// <summary>
        /// Album index page will be displayed
        /// </summary>
        /// <param name="orderby">order condition</param>
        /// <param name="number">list number to display on one page</param>
        /// <param name="page">present page number</param>
        /// <returns></returns>
        public async Task<IActionResult> Index(string orderby = "", int number = 0, int page = 0)
        {
            var recordset = (orderby == "location") ?
                await _context.BusStop.OrderBy(a => a.Location).ToListAsync() :
                await _context.BusStop.OrderBy(a => a.BusStopNumber).ToListAsync();

            if (number == 0)
            {
                number = 10;
            }
            else
            {
                HttpContext.Session.SetInt32(nameof(number), number);
            }

            if(HttpContext.Session.GetInt32(nameof(number)) != null)
            {
                number = int.Parse(HttpContext.Session.GetInt32(nameof(number)).ToString());
            }

            int total = recordset.Count / number;
            ViewData["orderby"] = orderby;
            ViewData["page"] = page;
            ViewData["total"] = total;

            if(page == -1)
            {
                return View(recordset.Skip(number * total).Take(number));
            }

            return View(recordset.Skip(number * page).Take(number));
        }

        // GET SKBusStops Detail page with id parameter
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var busStop = await _context.BusStop.SingleOrDefaultAsync(m => m.BusStopNumber == id);

            if (busStop == null)
            {
                return NotFound();
            }

            return View(busStop);
        }

        // GET SKBusStops Create page
        public IActionResult Create()
        {
            return View();
        }

        // POST SKBusStops Create to insert new data into database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("BusStopNumber,GoingDowntown,Location,LocationHash")] BusStop busStop)
        {
            if (ModelState.IsValid)
            {
                busStop.LocationHash = HashLocation(busStop.Location);
                _context.Add(busStop);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(busStop);
        }

        /// <summary>
        /// string value is converted into integer by adding there ascii code number
        /// </summary>
        /// <param name="l">string to convert into integer number</param>
        /// <returns>integer number is returned </returns>
        private int HashLocation(string l)
        {
            int sum = 0;
            foreach(char c in l)
            {
                sum += (int)c;
            }
            return sum;
        }
       
        // GET SKBusStops Edit page
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var busStop = await _context.BusStop.SingleOrDefaultAsync(m => m.BusStopNumber == id);
            if (busStop == null)
            {
                return NotFound();
            }
            return View(busStop);
        }

        // POST SKBusStops Edit to update edited data 
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("BusStopNumber,GoingDowntown,Location,LocationHash")] BusStop busStop)
        {
            if (id != busStop.BusStopNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    busStop.LocationHash = HashLocation(busStop.Location);
                    _context.Update(busStop);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BusStopExists(busStop.BusStopNumber))
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
            return View(busStop);
        }

        // GET SKBusStops Delete page with one parameter
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var busStop = await _context.BusStop.SingleOrDefaultAsync(m => m.BusStopNumber == id);
            if (busStop == null)
            {
                return NotFound();
            }

            return View(busStop);
        }

        // POST to delete data from database 
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var busStop = await _context.BusStop.SingleOrDefaultAsync(m => m.BusStopNumber == id);
            _context.BusStop.Remove(busStop);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // check if busstop number is exist
        private bool BusStopExists(int id)
        {
            return _context.BusStop.Any(e => e.BusStopNumber == id);
        }


        /// <summary>
        /// Route selector page to be displayed
        /// </summary>
        /// <param name="busStopNumber">number of bus stop</param>
        /// <returns></returns>
        public async Task<IActionResult> RouteSelector(int? busStopNumber)
        {
            if (busStopNumber == null)
            {
                TempData["message"] = "Please Select Bus Stop Number";
                return RedirectToAction("Index");
            }

            try
            {
                var recordset = await _context.RouteStop.Where(a => a.BusStopNumber == busStopNumber).Include(a => a.BusRouteCodeNavigation).ToListAsync();
                if (recordset.Count > 0)
                {
                    if (recordset.Count == 1)
                    {
                        var routeStopId = _context.RouteStop.SingleOrDefault(a => a.BusStopNumber == busStopNumber).RouteStopId;    
                        return RedirectToAction(actionName: "RouteStopSchedule", controllerName: "SKRouteSchedule", routeValues: new { routeStopId = routeStopId });
                    }

                    TempData["number"] = busStopNumber;
                    TempData["location"] = _context.BusStop.SingleOrDefault(a => a.BusStopNumber == busStopNumber).Location;
                    return View(recordset);
                }    
            }
            catch (Exception)
            {
                TempData["message"] = "Error occured, please try another route stop";
                return RedirectToAction("Index");
            }

            TempData["message"] = "Sorry there is no result of the bus stop " + busStopNumber.ToString();
            return RedirectToAction("Index");
            
        }
    }
}
