/*
 *  SKRouteStopController.cs
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
    public class SKRouteStopController : Controller
    {
        private readonly BusServiceContext _context;

        public SKRouteStopController(BusServiceContext context)
        {
            _context = context;    
        }

        
        /// <summary>
        /// To display index page of route stop controller
        /// </summary>
        /// <param name="busRouteCode">bus route code</param>
        /// <param name="routeName">rouet name</param>
        /// <returns>to index view</returns>
        public async Task<IActionResult> Index(string busRouteCode ="", string routeName ="")
        {
            // if there is bus route code in query string
            if (busRouteCode != "")
            {
                // if there is no route name, search the name and save it to routeName
                if (routeName == "")
                {
                    try
                    {
                        var temp = await _context.BusRoute.SingleAsync(a => a.BusRouteCode == busRouteCode);
                        routeName = temp.RouteName.ToString();
                    }
                    catch
                    {
                        TempData["message"] = "Cannot find the result";
                        return RedirectToAction(actionName: "Index", controllerName: "SKBusRoute");
                    }
                }

                HttpContext.Session.SetString(nameof(busRouteCode), busRouteCode);
                HttpContext.Session.SetString(nameof(routeName), routeName);
            }
            // if there is not bus route code in query string, but session keep the information
            else if(HttpContext.Session.GetString(nameof(busRouteCode)) != null )
            {
                busRouteCode = HttpContext.Session.GetString(nameof(busRouteCode));
                routeName = HttpContext.Session.GetString(nameof(routeName));
            }
            // if there is no information of bus route code
            else
            {
                TempData["message"] = "Please Select bus route name";
                return RedirectToAction(actionName: "Index", controllerName: "SKBusRoute");
            }

            ViewData["routeName"] = routeName;
            ViewData["busRouteCode"] = busRouteCode;

            var recordset = _context.RouteStop.Where(a => a.BusRouteCode == busRouteCode).OrderBy(a => a.OffsetMinutes)
                .Include(a => a.BusRouteCodeNavigation).Include(a => a.BusStopNumberNavigation);

            if (recordset.Count() == 0)
            {
                TempData["message"] = "Cannot find the route";
                return RedirectToAction(actionName: "Index", controllerName: "SKBusRoute");
            }

            return View(await recordset.ToListAsync());
        }


        
        /// <summary>
        /// To display details page
        /// </summary>
        /// <param name="id">id of chosen route stop </param>
        /// <returns></returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeStop = await _context.RouteStop.SingleOrDefaultAsync(m => m.RouteStopId == id);

            if (routeStop == null)
            {
                return NotFound();
            }

            return View(routeStop);
        }

        /// <summary>
        /// To display create page
        /// </summary>
        /// <returns></returns>
        public IActionResult Create()
        {
            var recordsetBusStopNumber = _context.BusStop.OrderBy(a => a.Location).ThenBy(a => a.GoingDowntown);

            foreach (var item in recordsetBusStopNumber)
            {
                item.Location += (item.GoingDowntown) ? " down town" : " up town";
            }
            ViewData["BusStopNumber"] = new SelectList(recordsetBusStopNumber, "BusStopNumber", "Location");
            ViewData["BusRoutecode"] = int.Parse(HttpContext.Session.GetString("busRouteCode"));
            return View();
        }

        /// <summary>
        /// To save new record into roue stop table
        /// </summary>
        /// <param name="routeStop">created route stop</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RouteStopId,BusRouteCode,BusStopNumber,OffsetMinutes")] RouteStop routeStop)
        {
            if (ModelState.IsValid)
            {
                _context.Add(routeStop);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            return View(routeStop);
        }

        /// <summary>
        /// To display edit page
        /// </summary>
        /// <param name="id">bus route stop id</param>
        /// <returns></returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeStop = await _context.RouteStop.SingleOrDefaultAsync(m => m.RouteStopId == id);

            if (routeStop == null)
            {
                return NotFound();
            }

            var recordsetBusStopNumber = _context.BusStop.OrderBy(a => a.Location).ThenBy(a => a.GoingDowntown);
            
            foreach(var item in recordsetBusStopNumber)
            {
                item.Location += (item.GoingDowntown) ? " down town" : " up town";
            }

            ViewData["BusRoutecode"] = int.Parse(HttpContext.Session.GetString("busRouteCode"));
            ViewData["BusStopNumber"] = new SelectList(recordsetBusStopNumber, "BusStopNumber", "Location", routeStop.BusStopNumber);

            return View(routeStop);
        }

        /// <summary>
        /// To set drop down list
        /// </summary>
        /// <param name="id">id of route stop to display</param>
        private void setDropDownList(int? id)
        {
            var routeStop = _context.RouteStop.SingleOrDefault(m => m.RouteStopId == id);
            ViewData["BusRoutecode"] = int.Parse(HttpContext.Session.GetString("busRouteCode"));
            ViewData["BusStopNumber"] = new SelectList(_context.BusStop.OrderBy(a => a.Location), "BusStopNumber", "Location", routeStop.BusStopNumber);
        }


        /// <summary>
        /// To save changed data
        /// </summary>
        /// <param name="id">bus route stop id</param>
        /// <param name="routeStop">changed value</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RouteStopId,BusRouteCode,BusStopNumber,OffsetMinutes")] RouteStop routeStop)
        {
            if (id != routeStop.RouteStopId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(routeStop);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RouteStopExists(routeStop.RouteStopId))
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
            ViewData["BusRouteCode"] = new SelectList(_context.BusRoute, "BusRouteCode", "BusRouteCode", routeStop.BusRouteCode);
            ViewData["BusStopNumber"] = new SelectList(_context.BusStop, "BusStopNumber", "BusStopNumber", routeStop.BusStopNumber);
            return View(routeStop);
        }

        /// <summary>
        /// To display delete page
        /// </summary>
        /// <param name="id">bus route stop id</param>
        /// <returns></returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeStop = await _context.RouteStop.SingleOrDefaultAsync(m => m.RouteStopId == id);
            if (routeStop == null)
            {
                return NotFound();
            }

            return View(routeStop);
        }

        /// <summary>
        /// To delete the bus route stop permenantly
        /// </summary>
        /// <param name="id">bus route stop id</param>
        /// <returns></returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var routeStop = await _context.RouteStop.SingleOrDefaultAsync(m => m.RouteStopId == id);
            _context.RouteStop.Remove(routeStop);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }


        /// <summary>
        /// To check if given id is exists in current context
        /// </summary>
        /// <param name="id">id number to be searched</param>
        /// <returns>true if there is data</returns>
        private bool RouteStopExists(int id)
        {
            return _context.RouteStop.Any(e => e.RouteStopId == id);
        }
    }
}
