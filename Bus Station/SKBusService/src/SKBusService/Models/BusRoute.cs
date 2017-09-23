using System;
using System.Collections.Generic;

namespace SKBusService.Models
{
    public partial class BusRoute : IComparable<BusRoute>
    {
        public BusRoute()
        {
            RouteSchedule = new HashSet<RouteSchedule>();
            RouteStop = new HashSet<RouteStop>();
        }

        public string BusRouteCode { get; set; }
        public string RouteName { get; set; }

        public virtual ICollection<RouteSchedule> RouteSchedule { get; set; }
        public virtual ICollection<RouteStop> RouteStop { get; set; }

        public int CompareTo(BusRoute other)
        {
            return (int.Parse(BusRouteCode) > int.Parse(other.BusRouteCode)) ? 1 : -1;
        }
    }
}
