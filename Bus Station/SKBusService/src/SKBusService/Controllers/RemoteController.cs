/*
 *  RemoteController.cs
 *  Assignment 5
 *  Created By:
 *      Soochang Kim, 7227663
 */
using SKClassLibrary;
using Microsoft.AspNetCore.Mvc;
using SKBusService.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SKBusService.Controllers
{
    /// <summary>
    /// To validate remote action of the controller
    /// </summary>
    public class RemoteController : Controller
    {
        private readonly BusServiceContext _context;
        private SKValidations custom = new SKValidations();

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="context">bus service database</param>
        public RemoteController(BusServiceContext context)
        {
            this._context = context;
        }

        /// <summary>
        /// To check province code
        /// </summary>
        /// <param name="provinceCode">province code to be validated</param>
        /// <returns>Result of validation</returns>
        public JsonResult provinceCodeCheck(string provinceCode)
        {
            if (!custom.isEmpty(provinceCode))
            {
                provinceCode = provinceCode.Trim();
                if (provinceCode.Length != 2)
                {
                    return Json($"Province Code '{provinceCode}' is not a valid (must 2 letters)");
                }

                try
                {
                    var province = _context.Province.SingleOrDefault(a => a.ProvinceCode == provinceCode);
                    if (province == null)
                    {
                        return Json($"Province Code '{provinceCode}' is not on the file");
                    }
                }
                catch (Exception ex)
                {
                    return Json($"Error validation province code {ex.GetBaseException().Message}");
                }

                return Json(true);
            }
            return Json("Povince code cannot be white space");
        }
    }
}
