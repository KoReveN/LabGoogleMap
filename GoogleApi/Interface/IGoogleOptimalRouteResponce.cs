using System;
using System.Collections.Generic;
using System.Text;

namespace GoogleApi
{
    public interface IGoogleOptimalRouteResponce
    {
        IEnumerable<int> WaypointOrder { get; set; }
        List<Leg> Legs { get; set; }
        string Polyline { get; set; }
    }
}
