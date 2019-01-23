using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleApi
{
    public class GoogleOptimalRouteResponce : IGoogleOptimalRouteResponce
    {
        public IEnumerable<int> WaypointOrder { get; set; }
        public List<Leg> Legs { get; set; }
        public string Polyline { get; set; }
    }
}
