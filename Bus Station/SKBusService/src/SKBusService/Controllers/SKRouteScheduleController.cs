/*
 *  SKRouteScheduleController.cs
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
using System.Net;

namespace SKBusService.Controllers
{
    public class SKRouteScheduleController : Controller
    {
        private readonly BusServiceContext _context;

        public SKRouteScheduleController(BusServiceContext context)
        {
            _context = context;    
        }

        /// <summary>
        /// To display list of route schedule
        /// </summary>
        /// <param name="busRouteCode">specific bus route code for displaying</param>
        /// <param name="routeName">route name for the bus route</param> 
        /// <returns>To index view page</returns>
        public async Task<IActionResult> Index(string busRouteCode, string routeName)
        {
            if (busRouteCode == null)
            {
                if ((busRouteCode = HttpContext.Session.GetString(nameof(busRouteCode))) != null) { }
                else if ((busRouteCode = HttpContext.Request.Cookies[nameof(busRouteCode)]) != null) { }
                else
                {
                    TempData["message"] = "Please select route to see the schedule";
                    return RedirectToAction(actionName: "Index", controllerName: "SKBusRoute");
                }
            }

            if (routeName == null)
            {
                routeName = _context.BusRoute.SingleOrDefault(a => a.BusRouteCode == busRouteCode).RouteName;
            }

            HttpContext.Session.SetString(nameof(busRouteCode), busRouteCode);
            HttpContext.Session.SetString(nameof(routeName), routeName);
            HttpContext.Response.Cookies.Append(nameof(busRouteCode), busRouteCode);
            
            var busServiceContext = _context.RouteSchedule.Include(r => r.BusRouteCodeNavigation).
                Where(a => a.BusRouteCode == busRouteCode).OrderBy(a => a.IsWeekDay == false).ThenBy(a => a.StartTime);
            return View(await busServiceContext.ToListAsync());
        }

        
        /// <summary>
        /// To display details of the route schedule
        /// </summary>
        /// <param name="id">id of route schedule</param>
        /// <returns>To details view</returns>
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeSchedule = await _context.RouteSchedule.SingleOrDefaultAsync(m => m.RouteScheduleId == id);
            if (routeSchedule == null)
            {
                return NotFound();
            }

            return View(routeSchedule);
        }

        /// <summary>
        /// To display create page
        /// </summary>
        /// <returns>To create view page</returns>
        public IActionResult Create()
        {
            ViewData["BusRouteCode"] = HttpContext.Session.GetString("busRouteCode");
            return View();
        }

        /// <summary>
        /// To save new record
        /// </summary>
        /// <param name="routeSchedule">route schdule which will be created</param>
        /// <returns>To </returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RouteScheduleId,BusRouteCode,Comments,IsWeekDay,StartTime")] RouteSchedule routeSchedule)
        {
            if (ModelState.IsValid)
            {
                _context.Add(routeSchedule);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["BusRouteCode"] = new SelectList(_context.BusRoute, "BusRouteCode", "BusRouteCode", routeSchedule.BusRouteCode);
            return View(routeSchedule);
        }

        /// <summary>
        /// To display edit view page
        /// </summary>
        /// <param name="id">route schedule id</param>
        /// <returns>To index view</returns>
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeSchedule = await _context.RouteSchedule.SingleOrDefaultAsync(m => m.RouteScheduleId == id);
            if (routeSchedule == null)
            {
                return NotFound();
            }
            ViewData["BusRouteCode"] = HttpContext.Session.GetString("busRouteCode");
            return View(routeSchedule);
        }

        /// <summary>
        /// To save edited data
        /// </summary>
        /// <param name="id">id of routeschdule id</param>
        /// <param name="routeSchedule">route schdule object to be updated</param>
        /// <returns></returns>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RouteScheduleId,BusRouteCode,Comments,IsWeekDay,StartTime")] RouteSchedule routeSchedule)
        {
            if (id != routeSchedule.RouteScheduleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(routeSchedule);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!RouteScheduleExists(routeSchedule.RouteScheduleId))
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
            ViewData["BusRouteCode"] = new SelectList(_context.BusRoute, "BusRouteCode", "BusRouteCode", routeSchedule.BusRouteCode);
            return View(routeSchedule);
        }

        /// <summary>
        /// To display delete page
        /// </summary>
        /// <param name="id">route schedule id</param>
        /// <returns>To delete page</returns>
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var routeSchedule = await _context.RouteSchedule.SingleOrDefaultAsync(m => m.RouteScheduleId == id);
            if (routeSchedule == null)
            {
                return NotFound();
            }

            return View(routeSchedule);
        }

        /// <summary>
        /// To delete data
        /// </summary>
        /// <param name="id">schdule id</param>
        /// <returns>To index page</returns>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var routeSchedule = await _context.RouteSchedule.SingleOrDefaultAsync(m => m.RouteScheduleId == id);
            _context.RouteSchedule.Remove(routeSchedule);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        /// <summary>
        /// Check if route schdule is exists
        /// </summary>
        /// <param name="id">route schdule id</param>
        /// <returns>true if exists</returns>
        private bool RouteScheduleExists(int id)
        {
            return _context.RouteSchedule.Any(e => e.RouteScheduleId == id);
        }


        /// <summary>
        /// To diplay specific route stop schdule
        /// </summary>
        /// <param name="routeStopId">route stop id to be displayed</param>
        /// <returns>To route stop schedule page</returns>
        public async Task<IActionResult> RouteStopSchedule(string routeStopId)
        {
            if (routeStopId == null)
            {
                TempData["message"] = "Please select route stop id";
                return RedirectToAction(actionName: "Index", controllerName: "SKRouteStop");
            }

            try
            {
                string busRouteCode = _context.RouteStop.SingleOrDefault(a => a.RouteStopId == int.Parse(routeStopId)).BusRouteCode;
                var recordset = await _context.RouteSchedule.Where(a => a.BusRouteCode == busRouteCode).OrderBy(a => a.StartTime).ToListAsync();
                var rstRouteStop = _context.RouteStop.Include(a => a.BusStopNumberNavigation). Include(a => a.BusRouteCodeNavigation).SingleOrDefault(a => a.RouteStopId.ToString() == routeStopId);

                HttpContext.Session.SetString("busRouteCode", rstRouteStop.BusRouteCodeNavigation.BusRouteCode);
                HttpContext.Session.SetString("routeName", rstRouteStop.BusRouteCodeNavigation.RouteName);
                HttpContext.Session.SetString("BusStopNumber", rstRouteStop.BusStopNumber.Value.ToString());
                HttpContext.Session.SetString("Location", rstRouteStop.BusStopNumberNavigation.Location);

                if (recordset.Count == 0)
                {
                    TempData["message"] = "Sorry cannot find result";
                    return RedirectToAction(actionName: "Index", controllerName: "SKRouteStop");
                }

                int offSetMinutes = (int)rstRouteStop.OffsetMinutes;
                foreach (var item in recordset)
                {
                    TimeSpan ts = new TimeSpan(0, offSetMinutes, 0);
                    item.StartTime = item.StartTime.Add(ts);
                }

                return View(recordset);
            }
            catch(Exception)
            {
                TempData["message"] = "Please enter valid route stop id";
            }

            return RedirectToAction(actionName: "Index", controllerName: "SKRouteStop");
        }
    }
}
