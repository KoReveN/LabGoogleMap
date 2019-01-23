using System;
using System.Collections.Generic;
using System.Text;
using GoogleApi;

namespace Domain.Entities
{
    public class RouteLeg : ILeg
    {
        public int RouteLegId { get; set; }
        public int StartPoint { get; set; }
        public int EndPoint { get; set; }
        public int Distance { get; set; }
        public int Duration { get; set; }
        public string Polyline { get; set; } 


        static public implicit operator RouteLeg (Leg leg)
        {
            return new RouteLeg()
            {
                RouteLegId = leg.RouteLegId,
                StartPoint = leg.StartPoint,
                EndPoint = leg.EndPoint,
                Distance = leg.Distance,
                Duration = leg.Duration,
                Polyline = leg.Polyline
            };
        }




    }
}
