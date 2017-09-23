/*
 *  BusSeriviceContext_Singleton.cs
 *  Assignment 04
 *  Created By:
 *      Soochang Kim, 11/12/2016
 */

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SKBusService.Models
{
    public class BusServiceContext_Singleton
    {
        private static BusServiceContext _context;
        private static object locker = new object();

        public static BusServiceContext Context()
        {
            if(_context == null)
            {
                lock(locker)
                {
                    if(_context == null)
                    {
                        var optionBuilder = new DbContextOptionsBuilder<BusServiceContext>();
                        optionBuilder.UseSqlServer(@"server=.\sqlexpress; database=BusService; Trusted_Connection=true");
                        _context = new BusServiceContext(optionBuilder.Options);

                    }
                }
            }
            return _context;
        }
    }

    public class BusServiceContext_UsersSingleton
    {
        private static BusServiceContext _context;
        private static object locker = new object();

        public static BusServiceContext Context()
        {
            if (_context == null)
            {
                lock (locker)
                {
                    if (_context == null)
                    {
                        var optionBuilder = new DbContextOptionsBuilder<BusServiceContext>();
                        optionBuilder.UseSqlServer(@"server=.\sqlexpress; database=aspnet; Trusted_Connection=true");
                        _context = new BusServiceContext(optionBuilder.Options);

                    }
                }
            }
            return _context;
        }
    }
}
