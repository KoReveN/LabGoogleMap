using Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace Service.RequestModels
{
    public class MapOtimizedRouteRequest
    {
        public IEnumerable<Marker> Markers { get; set; }
        public TrafficModel OptimalRouteOption { get; set; }

        public MapOtimizedRouteRequest()
        {
            Markers = new List<Marker>();
        }
    }


    public enum TrafficModel
    {
        [Description("Best guess")]
        best_guess,

        [Description("Pissimistic")]
        pessimistic,

        [Description("Optimistic")]
        optimistic
    }



        //var routeOptionsDataSource = traffic_model ["Pissimistic","Best guess","Optimistic"];


    }
