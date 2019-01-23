using Domain.Entities;
using GoogleApi;
using System;
using System.Collections.Generic;
using System.Text;

namespace Service.RequestModels
{
    public class MapOtimizedRouteResponce
    {

        public IEnumerable<Marker> Markers { get; set; }
        public IEnumerable<ILeg> Legs { get; set; }
        public string Polyline { get; set; }

        public MapOtimizedRouteResponce()
        {
            Markers = new List<Marker>();
            Legs = new List<Leg>();
        }

    }
}
