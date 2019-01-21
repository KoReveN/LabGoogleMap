using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.RequestModels
{
    public class MapOtimizedRouteResponce
    {

        public IEnumerable<Marker> Markers { get; set; }
        public RouteLeg Leg { get; set; }
        public MapOtimizedRouteResponce()
        {
            Markers = new List<Marker>();
        }

    }
}
